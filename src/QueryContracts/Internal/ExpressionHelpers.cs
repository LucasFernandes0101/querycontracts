using System.Linq.Expressions;
using System.Reflection;

namespace QueryContracts.Internal;

/// <summary>
/// Helper methods for building and applying expression trees over <see cref="System.Linq.IQueryable{T}"/>.
/// </summary>
internal static class ExpressionHelpers
{
    /// <summary>
    /// Applies an OrderBy or OrderByDescending operation
    /// using a non-typed <see cref="LambdaExpression"/>.
    /// </summary>
    public static IQueryable<TEntity> ApplyOrderBy<TEntity>(
        IQueryable<TEntity> source,
        LambdaExpression selector,
        Type propertyType,
        bool descending)
    {
        string methodName = descending
            ? nameof(Queryable.OrderByDescending)
            : nameof(Queryable.OrderBy);

        MethodInfo? targetMethod = null;

        foreach (MethodInfo m in typeof(Queryable).GetMethods())
        {
            if (m.Name == methodName && m.GetParameters().Length == 2)
            {
                targetMethod = m;
                break;
            }
        }

        MethodInfo method = (targetMethod ?? throw new MissingMethodException(
            nameof(Queryable),
            methodName))
            .MakeGenericMethod(typeof(TEntity), propertyType);

        return (IQueryable<TEntity>)method.Invoke(
            null,
            [source, selector])!;
    }
}
