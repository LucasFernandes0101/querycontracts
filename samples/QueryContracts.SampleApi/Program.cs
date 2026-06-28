using QueryContracts;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var products = new List<Product>
{
    new() { Id = Guid.NewGuid(), Name = "Mechanical Keyboard", Category = "Hardware", Price = 120m, IsAvailable = true, CreatedAt = new DateTime(2026, 1, 15) },
    new() { Id = Guid.NewGuid(), Name = "Wireless Mouse", Category = "Hardware", Price = 45m, IsAvailable = true, CreatedAt = new DateTime(2026, 2, 10) },
    new() { Id = Guid.NewGuid(), Name = "USB-C Cable", Category = "Accessories", Price = 12m, IsAvailable = false, CreatedAt = new DateTime(2026, 3, 5) },
    new() { Id = Guid.NewGuid(), Name = "Monitor Stand", Category = "Accessories", Price = 85m, IsAvailable = true, CreatedAt = new DateTime(2026, 4, 20) },
    new() { Id = Guid.NewGuid(), Name = "Laptop Sleeve", Category = "Accessories", Price = 30m, IsAvailable = true, CreatedAt = new DateTime(2026, 5, 12) },
    new() { Id = Guid.NewGuid(), Name = "Webcam HD", Category = "Hardware", Price = 65m, IsAvailable = false, CreatedAt = new DateTime(2026, 6, 8) },
};

app.MapGet("/products", (ProductQuery query) =>
{
    var result = products.AsQueryable().Apply(ProductContract.Instance, query);

    if (!result.IsValid)
        return Results.BadRequest(result.Errors);

    var items = result.Query.Select(p => new ProductResponse(
        p.Id, p.Name, p.Category, p.Price, p.IsAvailable, p.CreatedAt)).ToList();

    return Results.Ok(items);
});

app.Run();

public sealed class Product
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string Category { get; init; } = "";
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

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string Category,
    decimal Price,
    bool IsAvailable,
    DateTime CreatedAt);

public static class ProductContract
{
    public static readonly QueryContract<Product, ProductQuery> Instance =
        QueryContract.For<Product, ProductQuery>()
            .Filter(q => q.Search, p => p.Name).Contains()
            .Filter(q => q.Category, p => p.Category).Equals()
            .Filter(q => q.MinPrice, p => p.Price).GreaterThanOrEqual()
            .Filter(q => q.MaxPrice, p => p.Price).LessThanOrEqual()
            .Filter(q => q.Available, p => p.IsAvailable).Equals()
            .Sort(q => q.Sort)
                .Allow("name", p => p.Name)
                .Allow("price", p => p.Price)
                .Allow("created", p => p.CreatedAt)
                .Default("created", descending: true)
            .Page(q => q.Page, q => q.PageSize, maxSize: 100)
            .Build();
}
