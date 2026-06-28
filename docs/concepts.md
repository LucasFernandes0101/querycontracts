# Concepts

## What is a public query contract?

A **public query contract** is an explicit, type-safe definition of how an API endpoint accepts filtering, sorting, and pagination from clients.

Instead of scattering query logic across controllers, attributes, or dynamic LINQ strings, a query contract declares:

- Which filters are accepted.
- Which sort aliases are public.
- What the maximum page size is.
- How validation errors are structured.

## Why QueryContracts is intentionally small

QueryContracts does one thing: it maps an input object to an `IQueryable<T>` using expression tree composition.

It does **not**:

- Execute queries (no `ToList`, `Count`, `First`, `Any`, etc.).
- Depend on ASP.NET Core.
- Depend on EF Core.
- Parse a query language.
- Expose entity properties automatically.
- Support OData, GraphQL, or Dynamic LINQ.

This keeps the library focused, testable, and safe. The public API surface is small and explicit.

## The golden formula

```
IQueryable<TEntity> + TInput + QueryContract<TEntity, TInput> = QueryContractResult<TEntity>
```

The contract is built once, reused across requests, and applied to any `IQueryable<TEntity>`.

## Why two expressions?

```csharp
.Filter(q => q.Active, u => u.IsActive).Equals()
```

- The first expression reads from the **public input** (`TInput`).
- The second expression maps to the **entity property** (`TEntity`).

The library never guesses mappings by convention. This prevents accidental exposure of internal entity properties.

## Design principles

1. **Explicit over implicit** — every filter, sort, and pagination rule is declared.
2. **Composition over execution** — the library only composes `IQueryable`, never runs it.
3. **Validation is structured** — errors include a code, message, member name, and attempted value.
4. **No hidden dependencies** — the core package has no reference to ASP.NET Core, EF Core, or any database provider.
