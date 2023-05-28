namespace T3.Web;

public abstract record TypedGuid
{
    public Guid Value { get; init; }
}