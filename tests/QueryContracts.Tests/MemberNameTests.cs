namespace QueryContracts.Tests;

public class MemberNameTests
{
    private static readonly User[] _users =
    [
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Alice",
            IsActive = true,
            CreatedAt = new(2026, 1, 1)
        },
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Bob",
            IsActive = false,
            CreatedAt = new(2026, 2, 1)
        },
    ];

    [Fact]
    public void Unknown_Sort_Error_Uses_Input_Member_Name()
    {
        var contract = QueryContract.For<User, CustomUserQuery>()
            .Sort(q => q.OrderBy)
                .Allow("name", u => u.Name)
                .Default("name")
            .Build();

        var result = _users.AsQueryable().Apply(
            contract,
            new CustomUserQuery(
                null,
                null,
                "unknown"));

        Assert.False(result.IsValid);
        var error = Assert.Single(result.Errors);
        Assert.Equal("OrderBy", error.MemberName);
    }

    [Fact]
    public void Pagination_Errors_Uses_Input_Member_Names()
    {
        var contract = QueryContract.For<User, CustomUserQuery>()
            .Page(
                q => q.CurrentPage,
                q => q.Size,
                maxSize: 100)
            .Build();

        var result = _users.AsQueryable().Apply(
            contract,
            new CustomUserQuery(
                0,
                0,
                null));

        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains(result.Errors, e => e.MemberName == "CurrentPage");
        Assert.Contains(result.Errors, e => e.MemberName == "Size");
    }

    [Fact]
    public void Page_Size_Too_Large_Error_Uses_Input_Member_Name()
    {
        var contract = QueryContract.For<User, CustomUserQuery>()
            .Page(
                q => q.CurrentPage,
                q => q.Size,
                maxSize: 10)
            .Build();

        var result = _users.AsQueryable().Apply(
            contract,
            new CustomUserQuery(
                1,
                20,
                null));

        Assert.False(result.IsValid);
        var error = Assert.Single(result.Errors);
        Assert.Equal(QueryContractErrorCode.PageSizeTooLarge, error.Code);
        Assert.Equal("Size", error.MemberName);
    }

    private sealed record CustomUserQuery(
        int? CurrentPage,
        int? Size,
        string? OrderBy);
}
