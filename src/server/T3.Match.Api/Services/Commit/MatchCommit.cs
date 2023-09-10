namespace T3.Web.Services.Commit;

public record MatchCommit(CommitHeader Header, MatchCommitCommand[] Commands, MatchView View);