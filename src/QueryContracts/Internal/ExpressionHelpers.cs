using System.Linq.Expressions;

namespace QueryContracts.Internal;

/// <summary>
/// Helper methods for building and applying expression trees over <see cref="System.Linq.IQueryable{T}"/>.
/// </summary>
internal static class ExpressionHelpers
{
    /// <summary>
    /// Applies an <see cref="System.Linq.Queryable.OrderBy"/> or
    /// <see cref="System.Linq.Queryable.OrderByDescending"/> operation
    /// using a non-typed <see cref="LambdaExpression"/>.
    /// </summary>
    public static IQueryable<TEntity> ApplyOrderBy<TEntity>(
        IQueryable<TEntity> source,
        LambdaExpression selector,
        Type propertyType,
        bool descending)
    {
        var methodName = descending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);
        var method = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(TEntity), propertyType);

        return (IQueryable<TEntity>)method.Invoke(null, [source, selector])!;
    }
}
