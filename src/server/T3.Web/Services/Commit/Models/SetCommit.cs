namespace T3.Web.Services.Commit.Models;

public record SetCommit(SetCommitHeader Header, SetCommitCommand[] Commands, SetView View);