using QueryContracts;
using QueryContracts.SampleApi;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var products = new List<Product>
{
    new()
    {
        Id = Guid.NewGuid(),
        Name = "Mechanical Keyboard",
        Category = "Hardware",
        Price = 120m,
        IsAvailable = true,
        CreatedAt = new(2026, 1, 15)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Name = "Wireless Mouse",
        Category = "Hardware",
        Price = 45m,
        IsAvailable = true,
        CreatedAt = new(2026, 2, 10)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Name = "USB-C Cable",
        Category = "Accessories",
        Price = 12m,
        IsAvailable = false,
        CreatedAt = new(2026, 3, 5)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Name = "Monitor Stand",
        Category = "Accessories",
        Price = 85m,
        IsAvailable = true,
        CreatedAt = new(2026, 4, 20)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Name = "Laptop Sleeve",
        Category = "Accessories",
        Price = 30m,
        IsAvailable = true,
        CreatedAt = new(2026, 5, 12)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Name = "Webcam HD",
        Category = "Hardware",
        Price = 65m,
        IsAvailable = false,
        CreatedAt = new(2026, 6, 8)
    },
};

app.MapGet("/products", ([AsParameters] ProductQuery query) =>
{
    var result = products.AsQueryable().Apply(
        ProductContract.Instance,
        query);

    if (!result.IsValid)
    {
        return Results.BadRequest(result.Errors);
    }

    var items = result.Query.Select(p =>
        new ProductResponse(
            p.Id,
            p.Name,
            p.Category,
            p.Price,
            p.IsAvailable,
            p.CreatedAt))
        .ToList();

    return Results.Ok(items);
});

app.Run();
