using System.Text;

namespace T3.Web.Services.Identity;

public class PasswordValue
{
    public string Version { get; set; }
    public int Iterations { get; set; } // How many iterations the password was hashed with
    public byte[] Salt { get; set; } // The salt that was used to hash the password
    public byte[] Hash { get; set; } // The hash of the password (can also be used to determine the length of the hash).

    /// <summary>
    /// Additional options that can be used to store additional information about the password.
    /// </summary>
    public string[]? Options { get; set; }

    public string ToSerializedString()
    {
        var sb = new StringBuilder();

        sb.Append(SafeOption(Version, "Version")); // Required
        sb.Append('$');
        sb.Append(Iterations); // Required
        sb.Append('$');
        sb.Append(Convert.ToBase64String(Salt)); // Required

        // Additional options are 
        for (var index = 0; index < (Options ?? Array.Empty<string>()).Length; index++)
        {
            var option = (Options ?? Array.Empty<string>())[index];
            sb.Append('$');
            sb.Append(SafeOption(option, "options[" + index + "]"));
        }

        sb.Append('$');
        sb.Append(Convert.ToBase64String(Hash));
        return sb.ToString();
    }

    private string SafeOption(string value, string parameter)
    {
        if (!value.Contains('$')) return value;

        throw new Exception($"The value '{value}' contains a '$' which is not allowed in the {parameter} parameter.");
    }

    public static PasswordValue Parse(string input)
    {
        var values = input.Split('$');
        if (values.Length < 4)
            throw new Exception("The input string is not valid. It must contain at least 4 values separated by '$'.");

        return new PasswordValue()
        {
            Version = values[0],
            Iterations = int.Parse(values[1]),
            Salt = Convert.FromBase64String(values[2]),
            Options = values.Length > 4 ? values[3..^1] : null,
            Hash = Convert.FromBase64String(values[^1]),
        };
    }
}