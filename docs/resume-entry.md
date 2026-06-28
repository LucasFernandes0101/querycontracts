# Resume Entry

## Short resume bullet

- Built QueryContracts, an open-source .NET 10 NuGet library for defining type-safe public query contracts over `IQueryable<T>` using expression trees and a fluent API.

## Detailed resume version

**QueryContracts** — Open-source .NET 10 NuGet package (`0.1.0-preview.1`)

Designed and implemented a small, intentional library that maps application input objects to `IQueryable<T>` using explicit, type-safe query contracts.

- **Core library** uses generics and expression trees to compose `Where`, `OrderBy`, `Skip`, and `Take` operations without executing queries.
- **Fluent builder API** with filter rules (Equals, Contains, StartsWith, GreaterThanOrEqual, LessThanOrEqual), sort aliases with ascending/descending support and defaults, and pagination with configurable maximum page size.
- **Structured validation errors** with error codes, member names, and attempted values.
- **No dependency on ASP.NET Core, EF Core, or any database provider** in the core package.
- **xUnit test suite** covering filtering, sorting, pagination, and result validation.
- **Minimal API sample** demonstrating the library with an in-memory product list.
- **Professional documentation**: README, concepts guide, examples, LinkedIn post, and resume materials.
- **Semantic conventional commits** throughout development.
- Published as a preview NuGet package with manual `dotnet nuget push`.

Tech: .NET 10, C#, LINQ expression trees, generics, xUnit, Minimal API, NuGet.

Repo: https://github.com/LucasFernandes0101/querycontracts

## GitHub profile README version

### QueryContracts

Define public query contracts for .NET APIs.

A small .NET 10 library that maps an application input object to `IQueryable<T>` using an explicit, type-safe, fluent contract. Composes expression trees only — never executes queries. No ASP.NET Core or EF Core dependency in the core package.

- Filters: Equals, Contains, StartsWith, GreaterThanOrEqual, LessThanOrEqual
- Sorting: explicit aliases with `-` prefix for descending, configurable defaults
- Pagination: 1-based with configurable maximum page size
- Structured validation errors
- xUnit tests + Minimal API sample

NuGet: `dotnet add package QueryContracts --version 0.1.0-preview.1`

[Source](https://github.com/LucasFernandes0101/querycontracts)

## LinkedIn featured project description

**QueryContracts** — .NET 10 open-source NuGet library

Defines public query contracts over `IQueryable<T>` using a fluent, type-safe API. The core library composes expression trees (Where, OrderBy, Skip, Take) without executing queries and has no dependency on ASP.NET Core, EF Core, or any database provider.

Features include filter rules (Equals, Contains, StartsWith, comparison), sort aliases with descending prefix and defaults, pagination with maximum page size validation, and structured error reporting. Ships with xUnit tests and a Minimal API sample.

Early preview: `0.1.0-preview.1`

Links:
- GitHub: https://github.com/LucasFernandes0101/querycontracts
- NuGet: https://www.nuget.org/packages/QueryContracts
