# LinkedIn Post

I recently published QueryContracts, an open-source .NET 10 library for defining public query contracts over `IQueryable<T>`.

The idea is straightforward: instead of scattering filtering, sorting, and pagination logic across controllers or relying on dynamic LINQ strings, you define an explicit, type-safe contract that maps an application input object to an `IQueryable<T>`.

```csharp
QueryContract.For<User, UserQuery>()
    .Filter(q => q.Name, u => u.Name).Contains()
    .Filter(q => q.Active, u => u.IsActive).Equals()
    .Sort(q => q.Sort)
        .Allow("name", u => u.Name)
        .Default("created", descending: true)
    .Page(q => q.Page, q => q.PageSize, maxSize: 100)
    .Build();
```

Key design decisions:

- The core library composes expression trees only. It never executes queries.
- No dependency on ASP.NET Core or EF Core in the core package.
- No OData, GraphQL, or Dynamic LINQ.
- Sort aliases are explicit strings, not entity property names.
- Validation errors are structured with error codes, member names, and attempted values.
- The API requires two expressions per filter: one reads from the input, the other maps to the entity property. No convention-based guessing.

Built with .NET 10, generics, expression trees, a fluent builder API, xUnit tests, and a Minimal API sample.

This is an early preview (0.1.0-preview.1). Not production-ready yet.

Repo: https://github.com/LucasFernandes0101/querycontracts
NuGet: https://www.nuget.org/packages/QueryContracts

#dotnet #opensource #csharp #linq #expressiontrees #nuget
