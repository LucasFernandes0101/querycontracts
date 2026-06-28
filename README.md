# QueryContracts

Define public query contracts for .NET APIs.

QueryContracts maps an application input object to `IQueryable<T>` using an explicit, type-safe contract.

## The problem

Public API endpoints that accept filtering, sorting, and pagination need to answer:

- Which filters are accepted?
- Which sorts are public?
- What is the maximum page size?
- Where does query validation live?
- How do we avoid exposing internal entity properties by accident?

Without an explicit contract, these decisions are scattered across controllers, attributes, dynamic LINQ strings, or ad-hoc validation code.

## The solution

QueryContracts lets developers define public query behavior explicitly:

```csharp
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

## Installation

> This package is an early preview.

```bash
dotnet add package QueryContracts --version 0.1.0-preview.1
```

## Quickstart

Define your entity and input:

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
```

Define the contract:

```csharp
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

Apply it:

```csharp
var result = users.Apply(Contract, query);

if (!result.IsValid)
{
    return Results.BadRequest(result.Errors);
}

var finalQuery = result.Query;
```

## Why two expressions?

```csharp
.Filter(q => q.Active, u => u.IsActive).Equals()
```

The API requires two expressions per filter:

- The first expression reads from the **public/application input** (`TInput`).
- The second expression maps to the **entity property** (`TEntity`).

The library does not guess mappings by convention. Explicit mapping prevents accidental exposure of entity properties that should not be queryable.

## Sorting

Sort aliases are explicit strings declared in the contract:

| Sort string   | Behavior              |
|---------------|-----------------------|
| `"name"`      | Ascending by name     |
| `"-name"`     | Descending by name    |
| `"created"`   | Ascending by created  |
| `"-created"`  | Descending by created |

When the sort input is `null`, empty, or whitespace, the default sort is applied (if configured).

Unknown aliases return a structured error and do not fall back to the default.

## Pagination

- Page is **1-based**.
- Default page is **1** when `null`.
- Default page size is **20** when `null`.
- Maximum page size is configured by the contract.
- Invalid page or page size returns a structured error.

## Error handling

```csharp
var result = users.Apply(Contract, query);

if (!result.IsValid)
{
    return Results.BadRequest(result.Errors);
}
```

Errors are structured with:

- `Code` — a `QueryContractErrorCode` enum value.
- `Message` — a human-readable description.
- `MemberName` — the input member that caused the error.
- `AttemptedValue` — the value that was attempted.

## Design principles

- Does not execute queries.
- Does not depend on ASP.NET Core.
- Does not depend on EF Core.
- Does not expose entity properties automatically.
- Does not parse a query language.
- Keeps public query behavior explicit and testable.

## What QueryContracts is not

- Not OData
- Not GraphQL
- Not Dynamic LINQ
- Not a query language
- Not a repository abstraction
- Not an EF Core extension
- Not an ASP.NET Core extension

## Sample API

A Minimal API sample is available in [`samples/QueryContracts.SampleApi`](samples/QueryContracts.SampleApi).

It demonstrates the library with an in-memory product list and a `GET /products` endpoint.

## Roadmap

**v0.1 (current preview):**
- Filters (Equals, Contains, StartsWith, GreaterThanOrEqual, LessThanOrEqual)
- Sorting with aliases and defaults
- Pagination with maximum page size validation
- Structured validation errors
- Sample API

**Later:**
- More examples
- Optional ASP.NET Core helpers
- Better docs

## License

MIT
