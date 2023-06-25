using T3.Web.Services.Commit.ValueObjects;

namespace T3.Web.Services.Commit.Models;

/// <summary>
/// A view of a watch
/// </summary>
/// <param name="Id">The unique id of the watch</param>
/// <param name="Key">The technical name of the watch (for example timeout). The client can decide upon the name.</param>
/// <param name="Changes">The changes in the watch</param>
/// <param name="MaxMilliseconds">The maximum milliseconds allowed if any. This is mostly so that clients can visualize it.</param>
public record WatchView(WatchId Id, string Key, WatchChange[] Changes, int? MaxMilliseconds);

/// <summary>
/// How the state of a watch changed.
/// </summary>
/// <param name="State">The state of the watch when the timestamp happens</param>
/// <param name="Timestamp">When the change occurred</param>
public record WatchChange(WatchState State, Timestamp.Models.Timestamp Timestamp);

/// <summary>
/// The state of a watch
/// </summary>
public enum WatchState
{
    /// <summary>
    /// The watch is running (either started or resumed).
    /// </summary>
    Ticking,
    
    /// <summary>
    /// The watch is paused or halted.
    /// </summary>
    Pausing
}