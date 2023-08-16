namespace T3.Web.Services.Timestamp;

public record ServerTimestampEntity : ServerTimestamp
{
    public required Guid Id { get; set; }
}