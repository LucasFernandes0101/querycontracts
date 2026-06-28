# Examples

## Users

```csharp
public sealed class User
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed record UserQuery(
    string? Name,
    bool? Active,
    string? Sort,
    int? Page,
    int? PageSize);

public static readonly QueryContract<User, UserQuery> Contract =
    QueryContract.For<User, UserQuery>()
        .Filter(q => q.Name, u => u.Name).Contains()
        .Filter(q => q.Active, u => u.IsActive).Equals()
        .Sort(q => q.Sort)
            .Allow("name", u => u.Name)
            .Allow("created", u => u.CreatedAt)
            .Default("created", descending: true)
        .Page(q => q.Page, q => q.PageSize, maxSize: 100)
        .Build();
```

Usage:

```csharp
var result = users.Apply(Contract, new UserQuery("ali", true, "-name", 1, 20));
```

## Products

```csharp
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

public static readonly QueryContract<Product, ProductQuery> Contract =
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
```

Usage:

```csharp
var result = products.Apply(Contract,
    new ProductQuery("keyboard", "hardware", 50m, null, true, "-price", 1, 20));
```

## Orders

```csharp
public sealed class Order
{
    public Guid Id { get; init; }
    public string CustomerEmail { get; init; } = "";
    public decimal Total { get; init; }
    public string Status { get; init; } = "";
    public DateTime PlacedAt { get; init; }
}

public sealed record OrderQuery(
    string? Email,
    string? Status,
    decimal? MinTotal,
    decimal? MaxTotal,
    string? Sort,
    int? Page,
    int? PageSize);

public static readonly QueryContract<Order, OrderQuery> Contract =
    QueryContract.For<Order, OrderQuery>()
        .Filter(q => q.Email, o => o.CustomerEmail).Contains()
        .Filter(q => q.Status, o => o.Status).Equals()
        .Filter(q => q.MinTotal, o => o.Total).GreaterThanOrEqual()
        .Filter(q => q.MaxTotal, o => o.Total).LessThanOrEqual()
        .Sort(q => q.Sort)
            .Allow("email", o => o.CustomerEmail)
            .Allow("total", o => o.Total)
            .Allow("placed", o => o.PlacedAt)
            .Default("placed", descending: true)
        .Page(q => q.Page, q => q.PageSize, maxSize: 50)
        .Build();
```

## Audit logs

```csharp
public sealed class AuditEntry
{
    public Guid Id { get; init; }
    public string Actor { get; init; } = "";
    public string Action { get; init; } = "";
    public DateTime OccurredAt { get; init; }
}

public sealed record AuditQuery(
    string? Actor,
    string? Action,
    DateTime? From,
    DateTime? To,
    string? Sort,
    int? Page,
    int? PageSize);

public static readonly QueryContract<AuditEntry, AuditQuery> Contract =
    QueryContract.For<AuditEntry, AuditQuery>()
        .Filter(q => q.Actor, a => a.Actor).Contains()
        .Filter(q => q.Action, a => a.Action).Equals()
        .Filter(q => q.From, a => a.OccurredAt).GreaterThanOrEqual()
        .Filter(q => q.To, a => a.OccurredAt).LessThanOrEqual()
        .Sort(q => q.Sort)
            .Allow("actor", a => a.Actor)
            .Allow("occurred", a => a.OccurredAt)
            .Default("occurred", descending: true)
        .Page(q => q.Page, q => q.PageSize, maxSize: 200)
        .Build();
```
