namespace QueryContracts.Tests;

public class ContractImmutabilityTests
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
    ];

    [Fact]
    public void Builder_Can_Be_Reuse_After_Build_And_Contract_Does_Not_Change()
    {
        var builder = QueryContract.For<User, UserQuery>()
            .Filter(q => q.Name, u => u.Name).Contains();

        var contract1 = builder.Build();

        builder.Filter(q => q.Active, u => u.IsActive).Equals();

        var contract2 = builder.Build();

        var result1 = _users.AsQueryable().Apply(
            contract1,
            new UserQuery(
                "Ali",
                null,
                null,
                null,
                null));
        var result2 = _users.AsQueryable().Apply(
            contract2,
            new UserQuery(
                "Ali",
                null,
                null,
                null,
                null));

        Assert.True(result1.IsValid);
        Assert.Single(result1.Query.ToList());
        Assert.Equal("Alice", result1.Query.ToList()[0].Name);

        Assert.True(result2.IsValid);
        Assert.Single(result2.Query.ToList());
    }

    [Fact]
    public void Sort_Aliases_Dictionary_Is_Immutable_After_Build()
    {
        var builder = QueryContract.For<User, UserQuery>()
            .Sort(q => q.Sort)
                .Allow("name", u => u.Name)
                .Default("name");

        var contract = builder.Build();

        builder.Sort(q => q.Sort)
            .Allow("created", u => u.CreatedAt)
            .Default("name");

        var rebuiltContract = builder.Build();

        var result = _users.AsQueryable().Apply(
            contract,
            new UserQuery(
                null,
                null,
                "created",
                null,
                null));
        Assert.False(result.IsValid);
        var error = Assert.Single(result.Errors);
        Assert.Equal(QueryContractErrorCode.UnknownSort, error.Code);

        var resultRebuilt = _users.AsQueryable().Apply(
            rebuiltContract,
            new UserQuery(
                null,
                null,
                "created",
                null,
                null));
        Assert.True(resultRebuilt.IsValid);
    }

    [Fact]
    public void Multiple_Validation_Errors_Returned_Together()
    {
        var contract = QueryContract.For<User, UserQuery>()
            .Sort(q => q.Sort)
                .Allow("name", u => u.Name)
                .Default("name")
            .Page(q => q.Page, q => q.PageSize, maxSize: 100)
            .Build();

        var result = _users.AsQueryable().Apply(
            contract,
            new UserQuery(
                null,
                null,
                "badalias",
                0,
                200));

        Assert.False(result.IsValid);
        Assert.Equal(3, result.Errors.Count);
        Assert.Contains(result.Errors, e => e.Code == QueryContractErrorCode.UnknownSort);
        Assert.Contains(result.Errors, e => e.Code == QueryContractErrorCode.InvalidPage);
        Assert.Contains(result.Errors, e => e.Code == QueryContractErrorCode.PageSizeTooLarge);
    }

    [Fact]
    public void Unknown_Sort_Alias_Does_Not_Fall_Back_To_Default()
    {
        var contract = QueryContract.For<User, UserQuery>()
            .Sort(q => q.Sort)
                .Allow("name", u => u.Name)
                .Default("name")
            .Build();

        var result = _users.AsQueryable().Apply(
            contract,
            new UserQuery(
                null,
                null,
                "unknown",
                null,
                null));

        Assert.False(result.IsValid);
        var error = Assert.Single(result.Errors);
        Assert.Equal(QueryContractErrorCode.UnknownSort, error.Code);
        Assert.Equal("unknown", error.AttemptedValue);
    }
}
