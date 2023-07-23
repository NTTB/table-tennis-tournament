using T3.Web.Hubs;
using T3.Web.Services.Commit.Models;

namespace T3.Web.Services.Commit.Entities;

public class SetCommitEntity
{
    public MongoDB.Bson.ObjectId Id { get; set; }
    public SetCommit Content { get; set; }
    public MessageSignature Signature { get; set; }
}