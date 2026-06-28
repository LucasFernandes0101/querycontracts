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
        var query = source;

        foreach (var filter in contract.Filters)
        {
            var predicate = filter.GetPredicate(input);
            if (predicate is not null)
                query = query.Where(predicate);
        }

        if (contract.Sort is not null)
        {
            var (sort, error) = contract.Sort.Evaluate(input);
            if (error is not null)
            {
                errors.Add(error);
            }
            else if (sort is not null)
            {
                query = sort.Apply(query);
            }
        }

        if (contract.Page is not null && errors.Count == 0)
        {
            var (page, pageSize, error) = contract.Page.Evaluate(input);
            if (error is not null)
            {
                errors.Add(error);
            }
            else
            {
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }
        }

        return new QueryContractResult<TEntity>(query, errors.AsReadOnly());
    }
}
