using T3.Web.Services.Set.Models;

namespace T3.Web.Services.Set.Entities;

public record ServerTimestampEntity : ServerTimestamp
{
    public required Guid Id { get; set; }
}