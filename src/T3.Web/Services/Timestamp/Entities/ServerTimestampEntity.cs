using T3.Web.Services.Timestamp.Models;

namespace T3.Web.Services.Timestamp.Entities;

public record ServerTimestampEntity : ServerTimestamp
{
    public required Guid Id { get; set; }
}