namespace QueryContracts.SampleApi;

public sealed class Product
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public bool IsAvailable { get; init; }
    public DateTime CreatedAt { get; init; }
}
