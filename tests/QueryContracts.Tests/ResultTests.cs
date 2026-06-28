using QueryContracts;

namespace QueryContracts.Tests;

public class ResultTests
{
    private static readonly User[] Users =
    [
        new() { Id = Guid.NewGuid(), Name = "Alice", IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
        new() { Id = Guid.NewGuid(), Name = "Bob", IsActive = false, CreatedAt = new DateTime(2026, 2, 1) },
    ];

    private static readonly QueryContract<User, UserQuery> Contract =
        QueryContract.For<User, UserQuery>()
            .Filter(q => q.Name, u => u.Name).Contains()
            .Sort(q => q.Sort)
                .Allow("name", u => u.Name)
                .Default("name")
            .Page(q => q.Page, q => q.PageSize, maxSize: 100)
            .Build();

    [Fact]
    public void Valid_Result_Has_IsValid_True()
    {
        var result = Users.AsQueryable().Apply(Contract, new UserQuery("Ali", null, "name", 1, 10));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Valid_Result_Has_No_Errors()
    {
        var result = Users.AsQueryable().Apply(Contract, new UserQuery("Ali", null, "name", 1, 10));

        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Invalid_Result_Has_IsValid_False()
    {
        var result = Users.AsQueryable().Apply(Contract, new UserQuery(null, null, "bad", 1, 10));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Invalid_Result_Has_Structured_Errors()
    {
        var result = Users.AsQueryable().Apply(Contract, new UserQuery(null, null, "bad", 1, 10));

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.All(result.Errors, e => Assert.NotEmpty(e.Message));
    }

    [Fact]
    public void Result_Query_Remains_IQueryable()
    {
        var result = Users.AsQueryable().Apply(Contract, new UserQuery(null, null, null, null, null));

        Assert.IsAssignableFrom<IQueryable<User>>(result.Query);
    }
}
