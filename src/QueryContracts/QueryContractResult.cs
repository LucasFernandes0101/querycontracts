namespace QueryContracts;

/// <summary>
/// Holds the result of applying a <see cref="QueryContract{TEntity, TInput}"/>
/// to an <see cref="System.Linq.IQueryable{T}"/>.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <param name="Query">The composed <see cref="System.Linq.IQueryable{TEntity}"/> with filters, sorting and pagination applied when valid.</param>
/// <param name="Errors">A read-only list of validation errors, if any.</param>
public sealed record QueryContractResult<TEntity>(
    IQueryable<TEntity> Query,
    IReadOnlyList<QueryContractError> Errors)
{
    /// <summary>
    /// Gets a value indicating whether the result is valid (no errors).
    /// </summary>
    public bool IsValid => Errors.Count == 0;
}
