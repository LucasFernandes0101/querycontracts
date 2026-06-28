namespace QueryContracts.Tests;

public class PaginationTests
{
    private static readonly User[] _users = [.. Enumerable.Range(1, 25)
        .Select(i => new User
        {
            Id = Guid.NewGuid(),
            Name = $"User{i}",
            IsActive = true,
            CreatedAt = new(2026, 1, i),
        })];

    private static readonly QueryContract<User, UserQuery> _contract =
        QueryContract.For<User, UserQuery>()
            .Page(q => q.Page, q => q.PageSize, maxSize: 100)
            .Build();

    [Fact]
    public void Applies_Skip_And_Take_Correctly()
    {
        var result = _users.AsQueryable().Apply(
            _contract,
            new UserQuery(
                null,
                null,
                null,
                2,
                10));

        Assert.True(result.IsValid);
        var items = result.Query.ToList();
        Assert.Equal(10, items.Count);
        Assert.Equal("User11", items[0].Name);
        Assert.Equal("User20", items[9].Name);
    }

    [Fact]
    public void Defaults_Page_To_1_When_Null()
    {
        var result = _users.AsQueryable().Apply(
            _contract,
            new UserQuery(
                null,
                null,
                null,
                null,
                5));

        Assert.True(result.IsValid);
        var items = result.Query.ToList();
        Assert.Equal(5, items.Count);
        Assert.Equal("User1", items[0].Name);
    }

    [Fact]
    public void Defaults_PageSize_To_20_When_Null()
    {
        var result = _users.AsQueryable().Apply(
            _contract,
            new UserQuery(
                null,
                null,
                null,
                1,
                null));

        Assert.True(result.IsValid);
        var items = result.Query.ToList();
        Assert.Equal(20, items.Count);
    }

    [Fact]
    public void Rejects_Page_Less_Than_Or_Equal_To_Zero()
    {
        var result = _users.AsQueryable().Apply(
            _contract,
            new UserQuery(
                null,
                null,
                null,
                0,
                10));

        Assert.False(result.IsValid);
        var error = Assert.Single(result.Errors);
        Assert.Equal(QueryContractErrorCode.InvalidPage, error.Code);
    }

    [Fact]
    public void Rejects_PageSize_Less_Than_Or_Equal_To_Zero()
    {
        var result = _users.AsQueryable().Apply(
            _contract,
            new UserQuery(
                null,
                null,
                null,
                1,
                0));

        Assert.False(result.IsValid);
        var error = Assert.Single(result.Errors);
        Assert.Equal(QueryContractErrorCode.InvalidPageSize, error.Code);
    }

    [Fact]
    public void Rejects_PageSize_Greater_Than_MaxSize()
    {
        var result = _users.AsQueryable().Apply(
            _contract,
            new UserQuery(
                null,
                null,
                null,
                1,
                101));

        Assert.False(result.IsValid);
        var error = Assert.Single(result.Errors);
        Assert.Equal(QueryContractErrorCode.PageSizeTooLarge, error.Code);
    }
}
