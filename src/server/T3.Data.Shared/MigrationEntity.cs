namespace T3.Data.Shared;

public class MigrationEntity
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateTime ExecutedAt { get; set; }
}