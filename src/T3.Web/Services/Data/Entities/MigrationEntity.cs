namespace T3.Web.Services.Data.Entities;

public class MigrationEntity
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateTime ExecutedAt { get; set; }
}