namespace BazaR.Backend.Api.Contracts.Attributes;

public sealed record AddOptionItem(string Value, int SortOrder = 0);

public sealed class AddOptionsRequest
{
    public string? Value { get; init; }
    public int SortOrder { get; init; } = 0;

    public IReadOnlyList<AddOptionItem>? Options { get; init; }
}
