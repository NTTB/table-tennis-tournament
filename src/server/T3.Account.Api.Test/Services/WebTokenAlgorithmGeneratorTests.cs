using JWT.Algorithms;
using NSubstitute;
using T3.Account.Api.Errors;
using T3.Account.Api.Services;
using T3.Account.Api.Settings;

namespace T3.Account.Api.Test.Services;

public class WebTokenAlgorithmGeneratorTests
{
    private IWebTokenAlgorithmGenerator _sut = Substitute.For<IWebTokenAlgorithmGenerator>();
    private WebTokenSettings _settings = Substitute.For<WebTokenSettings>();
    private readonly List<string> _tempFiles = new();
    private string _publicKeyPath = string.Empty;
    private string _privateKeyPath = string.Empty;

    private string GetTempFile()
    {
        var f = Path.GetTempFileName();
        _tempFiles.Add(f);
        return f;
    }

    [SetUp]
    public void SetUp()
    {
        _publicKeyPath = GetTempFile();
        _privateKeyPath = GetTempFile();

        _settings = Substitute.For<WebTokenSettings>();
        _settings.PublicKeyPath.Returns(_publicKeyPath);
        _settings.PrivateKeyPath.Returns(_privateKeyPath);

        _sut = new WebTokenAlgorithmGenerator(_settings);
    }

    [TearDown]
    public void TearDown()
    {
        // Remove all temp files that we generated
        foreach (var file in _tempFiles)
        {
            File.Delete(file);
        }

        _tempFiles.Clear();
        _sut.Dispose();
    }

    [Test]
    public void Algorithm__is_NoneAlgorithm_if_no_keys_are_provided()
    {
        Assert.That(_sut.Algorithm, Is.TypeOf<NoneAlgorithm>());
    }

    [Test]
    public void Generate__throws_if_public_file_is_corrupt()
    {
        File.WriteAllText(_settings.PublicKeyPath, "corrupt");
        File.WriteAllText(_settings.PrivateKeyPath, RandomData.NextPrivateKeyPem());

        Assert.That(async () => await _sut.LoadFromDisk(), Throws.TypeOf<WebTokenGeneratorException>().With.Message.Contains("Failed to load public key"));
    }

    [Test]
    public void Generate__throws_if_private_file_is_corrupt()
    {
        File.WriteAllText(_settings.PublicKeyPath, RandomData.NextPublicKeyPem());
        File.WriteAllText(_settings.PrivateKeyPath, "corrupt");

        Assert.That(async () => await _sut.LoadFromDisk(), Throws.TypeOf<WebTokenGeneratorException>().With.Message.Contains("Failed to load private key"));
    }

    [Test]
    public void Generate__throws_if_private_is_missing_while_public_exists()
    {
        File.Delete(_settings.PublicKeyPath);
        Assert.That(async () => await _sut.LoadFromDisk(),
            Throws.TypeOf<WebTokenGeneratorException>().With.Message.Contains("Private key exists but public key is missing"));
    }

    [Test]
    public void Generate__throws_if_public_is_missing_while_public_exists()
    {
        File.Delete(_settings.PrivateKeyPath);
        Assert.That(async () => await _sut.LoadFromDisk(),
            Throws.TypeOf<WebTokenGeneratorException>().With.Message.Contains("Public key exists but private key is missing"));
    }

    [Test]
    public void Generate__Creates_new_keys_if_files_are_missing()
    {
        File.Delete(_settings.PublicKeyPath);
        File.Delete(_settings.PrivateKeyPath);

        Assume.That(File.Exists(_settings.PublicKeyPath) || File.Exists(_settings.PrivateKeyPath), Is.False, "since they were just deleted it");

        Assert.That(async () => await _sut.LoadFromDisk(), Throws.Nothing);

        Assume.That(File.Exists(_settings.PublicKeyPath) && File.Exists(_settings.PrivateKeyPath), Is.True, "since it was generated");
    }
}