using System.Linq.Expressions;
using QueryContracts.Internal;

namespace QueryContracts;

/// <summary>
/// Extension methods for applying <see cref="QueryContract{TEntity, TInput}"/>
/// instances to <see cref="System.Linq.IQueryable{T}"/>.
/// </summary>
public static class QueryContractQueryableExtensions
{
    /// <summary>
    /// Applies the given <paramref name="contract"/> to <paramref name="source"/>
    /// using the values from <paramref name="input"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TInput">The application input type.</typeparam>
    /// <param name="source">The queryable to compose filters, sorting and pagination onto.</param>
    /// <param name="contract">The query contract defining the public query behavior.</param>
    /// <param name="input">The input object carrying filter, sort and pagination values.</param>
    /// <returns>
    /// A <see cref="QueryContractResult{TEntity}"/> containing the composed query
    /// and any validation errors.
    /// </returns>
    public static QueryContractResult<TEntity> Apply<TEntity, TInput>(
        this IQueryable<TEntity> source,
        QueryContract<TEntity, TInput> contract,
        TInput input)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(contract);
        ArgumentNullException.ThrowIfNull(input);

        var errors = new List<QueryContractError>();
        IQueryable<TEntity> query = source;
        bool hasSortError = false;

        foreach (FilterRule<TEntity, TInput> filter in contract.Filters)
        {
            Expression<Func<TEntity, bool>>? predicate = filter.GetPredicate(input);
            if (predicate is not null)
            {
                query = query.Where(predicate);
            }
        }

        if (contract.Sort is not null)
        {
            (SortApplication<TEntity>? sort, QueryContractError? error) = contract.Sort.Evaluate(input);
            if (error is not null)
            {
                errors.Add(error);
                hasSortError = true;
            }
            else if (sort is not null)
            {
                query = sort.Apply(query);
            }
        }

        if (contract.Page is not null)
        {
            (int page, int pageSize, IReadOnlyList<QueryContractError> pageErrors) = contract.Page.Evaluate(input);
            if (pageErrors.Count > 0)
            {
                errors.AddRange(pageErrors);
            }
            else if (!hasSortError)
            {
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }
        }

        return new(query, [.. errors]);
    }
}
