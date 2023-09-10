namespace T3.Common.Models;

/// <summary>
/// Contains information about the client application.
/// </summary>
public record ClientApp
{
    /// <summary>
    /// The name of the application.
    /// </summary>
    public required string Name { get; init; }
    
    /// <summary>
    /// The version number of the application
    /// </summary>
    public string? Version { get; init; }
    
    /// <summary>
    /// The url of the application, where it can be downloaded.
    /// </summary>
    public string? Url { get; init; }
}