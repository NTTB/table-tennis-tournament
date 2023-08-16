using T3.Web.Hubs;

namespace T3.Web.Services.Commit;

public class MatchCommitEntity
{
    public MongoDB.Bson.ObjectId Id { get; set; }
    public MatchCommit Content { get; set; }
    public MessageSignature Signature { get; set; }
}