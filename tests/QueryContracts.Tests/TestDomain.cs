namespace QueryContracts.Tests;

public sealed class User
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed record UserQuery(
    string? Name,
    bool? Active,
    string? Sort,
    int? Page,
    int? PageSize);

public sealed class Product
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public bool IsAvailable { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed record ProductQuery(
    string? Search,
    string? Category,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? Available,
    string? Sort,
    int? Page,
    int? PageSize);
