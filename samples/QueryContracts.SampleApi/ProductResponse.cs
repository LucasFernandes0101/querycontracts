namespace QueryContracts.SampleApi;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string Category,
    decimal Price,
    bool IsAvailable,
    DateTime CreatedAt);
