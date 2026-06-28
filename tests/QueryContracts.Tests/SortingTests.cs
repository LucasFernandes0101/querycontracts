using QueryContracts;

namespace QueryContracts.Tests;

public class SortingTests
{
    private static readonly User[] Users =
    [
        new() { Id = Guid.NewGuid(), Name = "Charlie", IsActive = true, CreatedAt = new DateTime(2026, 3, 1) },
        new() { Id = Guid.NewGuid(), Name = "Alice", IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
        new() { Id = Guid.NewGuid(), Name = "Bob", IsActive = false, CreatedAt = new DateTime(2026, 2, 1) },
    ];

    private static readonly QueryContract<User, UserQuery> Contract =
        QueryContract.For<User, UserQuery>()
            .Sort(q => q.Sort)
                .Allow("name", u => u.Name)
                .Allow("created", u => u.CreatedAt)
                .Default("created", descending: true)
            .Build();

    [Fact]
    public void Sort_Ascending_By_Alias()
    {
        var result = Users.AsQueryable().Apply(Contract, new UserQuery(null, null, "name", null, null));

        Assert.True(result.IsValid);
        var items = result.Query.ToList();
        Assert.Equal("Alice", items[0].Name);
        Assert.Equal("Bob", items[1].Name);
        Assert.Equal("Charlie", items[2].Name);
    }

    [Fact]
    public void Sort_Descending_With_Dash_Prefix()
    {
        var result = Users.AsQueryable().Apply(Contract, new UserQuery(null, null, "-name", null, null));

        Assert.True(result.IsValid);
        var items = result.Query.ToList();
        Assert.Equal("Charlie", items[0].Name);
        Assert.Equal("Bob", items[1].Name);
        Assert.Equal("Alice", items[2].Name);
    }

    [Fact]
    public void Default_Sort_Is_Applied_When_Sort_Is_Null()
    {
        var result = Users.AsQueryable().Apply(Contract, new UserQuery(null, null, null, null, null));

        Assert.True(result.IsValid);
        var items = result.Query.ToList();
        Assert.Equal(new DateTime(2026, 3, 1), items[0].CreatedAt);
        Assert.Equal(new DateTime(2026, 2, 1), items[1].CreatedAt);
        Assert.Equal(new DateTime(2026, 1, 1), items[2].CreatedAt);
    }

    [Fact]
    public void Default_Sort_Is_Applied_When_Sort_Is_Empty()
    {
        var result = Users.AsQueryable().Apply(Contract, new UserQuery(null, null, "", null, null));

        Assert.True(result.IsValid);
        var items = result.Query.ToList();
        Assert.Equal(new DateTime(2026, 3, 1), items[0].CreatedAt);
    }

    [Fact]
    public void Unknown_Sort_Alias_Returns_IsValid_False()
    {
        var result = Users.AsQueryable().Apply(Contract, new UserQuery(null, null, "unknownfield", null, null));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Unknown_Sort_Alias_Returns_Structured_Error()
    {
        var result = Users.AsQueryable().Apply(Contract, new UserQuery(null, null, "unknownfield", null, null));

        Assert.False(result.IsValid);
        var error = Assert.Single(result.Errors);
        Assert.Equal(QueryContractErrorCode.UnknownSort, error.Code);
        Assert.Equal("Sort", error.MemberName);
        Assert.Equal("unknownfield", error.AttemptedValue);
    }

    [Fact]
    public void Unknown_Sort_Alias_Does_Not_Silently_Fall_Back_To_Default_Sort()
    {
        var result = Users.AsQueryable().Apply(Contract, new UserQuery(null, null, "unknownfield", null, null));

        Assert.False(result.IsValid);
        var error = Assert.Single(result.Errors);
        Assert.Equal(QueryContractErrorCode.UnknownSort, error.Code);
    }

    [Fact]
    public void Unknown_Sort_Alias_Does_Not_Execute_Query()
    {
        var result = Users.AsQueryable().Apply(Contract, new UserQuery(null, null, "unknownfield", 1, 10));

        Assert.False(result.IsValid);
        var error = Assert.Single(result.Errors);
        Assert.Equal(QueryContractErrorCode.UnknownSort, error.Code);
    }
}
