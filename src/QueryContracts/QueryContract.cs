using QueryContracts.Internal;

namespace QueryContracts;

/// <summary>
/// Provides the entry point for creating a query contract.
/// </summary>
public static class QueryContract
{
    /// <summary>
    /// Creates a new <see cref="QueryContractBuilder{TEntity, TInput}"/>
    /// for the specified entity and input types.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being queried.</typeparam>
    /// <typeparam name="TInput">The application input type that carries filter, sort and pagination values.</typeparam>
    /// <returns>A new builder instance.</returns>
    public static QueryContractBuilder<TEntity, TInput> For<TEntity, TInput>()
        => new();
}

/// <summary>
/// An immutable query contract that maps <typeparamref name="TInput"/> to
/// <see cref="System.Linq.IQueryable{TEntity}"/> using configured filters,
/// sorting and pagination rules.
/// </summary>
/// <typeparam name="TEntity">The entity type being queried.</typeparam>
/// <typeparam name="TInput">The application input type.</typeparam>
public sealed class QueryContract<TEntity, TInput>
{
    internal IReadOnlyList<FilterRule<TEntity, TInput>> Filters { get; }
    internal SortRule<TEntity, TInput>? Sort { get; }
    internal PageRule<TInput>? Page { get; }

    internal QueryContract(
        IReadOnlyList<FilterRule<TEntity, TInput>> filters,
        SortRule<TEntity, TInput>? sort,
        PageRule<TInput>? page)
    {
        Filters = filters;
        Sort = sort;
        Page = page;
    }
}
