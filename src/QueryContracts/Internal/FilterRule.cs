using System.Linq.Expressions;

namespace QueryContracts.Internal;

/// <summary>
/// Defines the kind of comparison applied by a filter rule.
/// </summary>
internal enum FilterKind
{
    Equals,
    Contains,
    StartsWith,
    GreaterThanOrEqual,
    LessThanOrEqual,
}

/// <summary>
/// Abstract base for filter rules that compose <see cref="Expression{TDelegate}"/>
/// predicates over <typeparamref name="TEntity"/>.
/// </summary>
internal abstract class FilterRule<TEntity, TInput>
{
    /// <summary>
    /// Builds a predicate expression for the given input, or returns <c>null</c>
    /// when the filter should be skipped (e.g. the input value is <c>null</c>).
    /// </summary>
    public abstract Expression<Func<TEntity, bool>>? GetPredicate(TInput input);
}

/// <summary>
/// Concrete filter rule that reads a value from the input object and builds
/// a predicate expression over the entity.
/// </summary>
internal sealed class FilterRule<TEntity, TInput, TEntityProperty> : FilterRule<TEntity, TInput>
{
    private readonly Func<TInput, object?> _inputAccessor;
    private readonly Expression<Func<TEntity, TEntityProperty>> _entitySelector;
    private readonly FilterKind _kind;

    internal FilterRule(
        Func<TInput, object?> inputAccessor,
        Expression<Func<TEntity, TEntityProperty>> entitySelector,
        FilterKind kind)
    {
        _inputAccessor = inputAccessor;
        _entitySelector = entitySelector;
        _kind = kind;
    }

    public override Expression<Func<TEntity, bool>>? GetPredicate(TInput input)
    {
        object? inputValue = _inputAccessor(input);

        if (inputValue is null)
        {
            return null;
        }

        if (_kind is FilterKind.Contains or FilterKind.StartsWith)
        {
            if (inputValue is not string s || string.IsNullOrWhiteSpace(s))
            {
                return null;
            }

            inputValue = s;
        }

        ConstantExpression constant = Expression.Constant(inputValue, typeof(TEntityProperty));
        Expression body = _kind switch
        {
            FilterKind.Equals =>
                Expression.Equal(_entitySelector.Body, constant),
            FilterKind.GreaterThanOrEqual =>
                Expression.GreaterThanOrEqual(_entitySelector.Body, constant),
            FilterKind.LessThanOrEqual =>
                Expression.LessThanOrEqual(_entitySelector.Body, constant),
            FilterKind.Contains =>
                Expression.Call(
                    _entitySelector.Body,
                    typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!,
                    constant),
            FilterKind.StartsWith =>
                Expression.Call(
                    _entitySelector.Body,
                    typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)])!,
                    constant),
            _ => throw new InvalidOperationException($"Unsupported filter kind: {_kind}"),
        };

        return Expression.Lambda<Func<TEntity, bool>>(body, _entitySelector.Parameters[0]);
    }
}
