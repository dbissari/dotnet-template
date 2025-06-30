namespace Infrastructure.Results;

public sealed record Fault(string Code, string Description)
{
    public static readonly Fault None = new(string.Empty, string.Empty);
}
