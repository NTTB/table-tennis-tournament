namespace T3.Web.Services.Set.Models;

public record SetCommitAuthor
{
    /// <summary>
    ///     The user who created the commit, can be direct or indirect.
    /// </summary>
    public required UserId UserId { get; init; }

    /// <summary>
    ///     The session that created the commit. People can follow a session and see all the changes that happen in it.
    /// </summary>
    public required SessionId SessionId { get; init; }

    /// <summary>
    ///     How should the author be displayed in the UI? This can be a username or a display name.
    ///     Examples: "Wouter", "Table 3"
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    ///     The device name that created the commit. This is mostly used to find out which device created a commit.
    ///     Examples: "Wouter's iPhone", "Digital Scoreboard A"
    /// </summary>
    public required string? DeviceName { get; init; }

    /// <summary>
    ///     The client app that created the commit. This can be used to market other apps, but also by app creators to find out
    ///     which apps are used and if there app has an issue.
    /// </summary>
    public required ClientApp? ClientApp { get; init; }
}