namespace QueryContracts.SampleApi;

public sealed record ProductQuery(
    string? Search,
    string? Category,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? Available,
    string? Sort,
    int? Page,
    int? PageSize);
