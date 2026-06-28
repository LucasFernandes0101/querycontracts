using System.Linq.Expressions;
using System.Reflection;

namespace QueryContracts.Internal;

/// <summary>
/// Helper methods for building and applying expression trees over <see cref="System.Linq.IQueryable{T}"/>.
/// </summary>
internal static class ExpressionHelpers
{
    /// <summary>
    /// Extracts the member name from a simple property access expression.
    /// </summary>
    public static string GetMemberName<TInput, TProperty>(
        Expression<Func<TInput, TProperty>> selector)
    {
        var body = selector.Body;

        if (body is UnaryExpression unary &&
            unary.Operand is MemberExpression operandMember)
        {
            body = operandMember;
        }

        if (body is not MemberExpression memberExpression)
        {
            throw new ArgumentException(
                "Selector must be a simple member access expression.",
                nameof(selector));
        }

        return memberExpression.Member.Name;
    }

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

        var method = (targetMethod ?? throw new MissingMethodException(
            nameof(Queryable),
            methodName))
            .MakeGenericMethod(typeof(TEntity), propertyType);

        return (IQueryable<TEntity>)method.Invoke(
            null,
            [source, selector])!;
    }
}
