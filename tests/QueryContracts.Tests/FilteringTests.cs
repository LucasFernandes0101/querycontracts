namespace QueryContracts.Tests;

public class FilteringTests
{
    private static readonly User[] _users =
    [
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Alice",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1)
        },
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Bob",
            IsActive = false,
            CreatedAt = new DateTime(2026, 2, 1)
        },
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Charlie",
            IsActive = true,
            CreatedAt = new DateTime(2026, 3, 1)
        },
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Alice Cooper",
            IsActive = true,
            CreatedAt = new DateTime(2026, 4, 1)
        },
    ];

    private static readonly Product[] _products =
    [
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Mechanical Keyboard",
            Category = "Hardware",
            Price = 120m,
            IsAvailable = true,
            CreatedAt = new DateTime(2026, 1, 10)
        },
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Wireless Mouse",
            Category = "Hardware",
            Price = 45m,
            IsAvailable = false,
            CreatedAt = new DateTime(2026, 2, 15)
        },
        new()
        {
            Id = Guid.NewGuid(),
            Name = "USB Cable",
            Category = "Accessories",
            Price = 12m,
            IsAvailable = true,
            CreatedAt = new DateTime(2026, 3, 20)
        },
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Monitor Stand",
            Category = "Accessories",
            Price = 85m,
            IsAvailable = true,
            CreatedAt = new DateTime(2026, 4, 5)
        },
    ];

    [Fact]
    public void Equals_Filter_Applies_When_Input_Value_Is_Not_Null()
    {
        var contract = QueryContract.For<User, UserQuery>()
            .Filter(q => q.Active, u => u.IsActive).Equals()
            .Build();

        var result = _users.AsQueryable().Apply(
            contract,
            new UserQuery(
                null,
                true,
                null,
                null,
                null));

        Assert.True(result.IsValid);
        var items = result.Query.ToList();
        Assert.Equal(3, items.Count);
        Assert.All(items, u => Assert.True(u.IsActive));
    }

    [Fact]
    public void Equals_Filter_Is_Ignored_When_Input_Value_Is_Null()
    {
        var contract = QueryContract.For<User, UserQuery>()
            .Filter(q => q.Active, u => u.IsActive).Equals()
            .Build();

        var result = _users.AsQueryable().Apply(
            contract,
            new UserQuery(
                null,
                null,
                null,
                null,
                null));

        Assert.True(result.IsValid);
        Assert.Equal(4, result.Query.ToList().Count);
    }

    [Fact]
    public void Contains_Filter_Works_For_Strings()
    {
        var contract = QueryContract.For<User, UserQuery>()
            .Filter(q => q.Name, u => u.Name).Contains()
            .Build();

        var result = _users.AsQueryable().Apply(
            contract,
            new UserQuery(
                "Alice",
                null,
                null,
                null,
                null));

        Assert.True(result.IsValid);
        var items = result.Query.ToList();
        Assert.Equal(2, items.Count);
        Assert.Contains(items, u => u.Name == "Alice");
        Assert.Contains(items, u => u.Name == "Alice Cooper");
    }

    [Fact]
    public void Contains_Filter_Ignores_Null()
    {
        var contract = QueryContract.For<User, UserQuery>()
            .Filter(q => q.Name, u => u.Name).Contains()
            .Build();

        var result = _users.AsQueryable().Apply(
            contract,
            new UserQuery(
                null,
                null,
                null,
                null,
                null));

        Assert.True(result.IsValid);
        Assert.Equal(4, result.Query.ToList().Count);
    }

    [Fact]
    public void Contains_Filter_Ignores_Empty_String()
    {
        var contract = QueryContract.For<User, UserQuery>()
            .Filter(q => q.Name, u => u.Name).Contains()
            .Build();

        var result = _users.AsQueryable().Apply(
            contract,
            new UserQuery(
                "",
                null,
                null,
                null,
                null));

        Assert.True(result.IsValid);
        Assert.Equal(4, result.Query.ToList().Count);
    }

    [Fact]
    public void Contains_Filter_Ignores_Whitespace()
    {
        var contract = QueryContract.For<User, UserQuery>()
            .Filter(q => q.Name, u => u.Name).Contains()
            .Build();

        var result = _users.AsQueryable().Apply(
            contract,
            new UserQuery(
                "   ",
                null,
                null,
                null,
                null));

        Assert.True(result.IsValid);
        Assert.Equal(4, result.Query.ToList().Count);
    }

    [Fact]
    public void StartsWith_Filter_Works_For_Strings()
    {
        var contract = QueryContract.For<User, UserQuery>()
            .Filter(q => q.Name, u => u.Name).StartsWith()
            .Build();

        var result = _users.AsQueryable().Apply(
            contract,
            new UserQuery(
                "Ali",
                null,
                null,
                null,
                null));

        Assert.True(result.IsValid);
        var items = result.Query.ToList();
        Assert.Equal(2, items.Count);
        Assert.All(items, u => Assert.StartsWith("Ali", u.Name));
    }

    [Fact]
    public void GreaterThanOrEqual_Works_For_Decimal()
    {
        var contract = QueryContract.For<Product, ProductQuery>()
            .Filter(q => q.MinPrice, p => p.Price).GreaterThanOrEqual()
            .Build();

        var result = _products.AsQueryable().Apply(
            contract,
            new ProductQuery(
                null,
                null,
                50m,
                null,
                null,
                null,
                null,
                null));

        Assert.True(result.IsValid);
        var items = result.Query.ToList();
        Assert.Equal(2, items.Count);
        Assert.All(items, p => Assert.True(p.Price >= 50m));
    }

    [Fact]
    public void LessThanOrEqual_Works_For_Decimal()
    {
        var contract = QueryContract.For<Product, ProductQuery>()
            .Filter(q => q.MaxPrice, p => p.Price).LessThanOrEqual()
            .Build();

        var result = _products.AsQueryable().Apply(
            contract,
            new ProductQuery(
                null,
                null,
                null,
                50m,
                null,
                null,
                null,
                null));

        Assert.True(result.IsValid);
        var items = result.Query.ToList();
        Assert.Equal(2, items.Count);
        Assert.All(items, p => Assert.True(p.Price <= 50m));
    }

    [Fact]
    public void Multiple_Filters_Compose_Correctly()
    {
        var contract = QueryContract.For<Product, ProductQuery>()
            .Filter(q => q.Search, p => p.Name).Contains()
            .Filter(q => q.Category, p => p.Category).Equals()
            .Filter(q => q.MinPrice, p => p.Price).GreaterThanOrEqual()
            .Filter(q => q.MaxPrice, p => p.Price).LessThanOrEqual()
            .Filter(q => q.Available, p => p.IsAvailable).Equals()
            .Build();

        var result = _products.AsQueryable().Apply(
            contract,
            new ProductQuery(
                "o",
                "Hardware",
                40m,
                200m,
                true,
                null,
                null,
                null));

        Assert.True(result.IsValid);
        var items = result.Query.ToList();
        Assert.Single(items);
        Assert.Equal("Mechanical Keyboard", items[0].Name);
    }
}
