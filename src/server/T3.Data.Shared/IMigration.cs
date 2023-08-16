namespace T3.Data.Shared;

public interface IMigration
{
    public Task Up();
}