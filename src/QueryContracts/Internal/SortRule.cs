using System.Linq.Expressions;

namespace QueryContracts.Internal;

/// <summary>
/// Represents a single sort alias mapped to an entity property selector.
/// </summary>
internal sealed class SortAlias<TEntity>
{
    public string Name { get; }
    public LambdaExpression Selector { get; }
    public Type PropertyType { get; }

    internal SortAlias(string name, LambdaExpression selector, Type propertyType)
    {
        Name = name;
        Selector = selector;
        PropertyType = propertyType;
    }
}

/// <summary>
/// Represents a resolved sort operation ready to be applied to a query.
/// </summary>
internal sealed class SortApplication<TEntity>
{
    private readonly SortAlias<TEntity> _alias;
    private readonly bool _descending;

    internal SortApplication(SortAlias<TEntity> alias, bool descending)
    {
        _alias = alias;
        _descending = descending;
    }

    public IQueryable<TEntity> Apply(IQueryable<TEntity> source)
    {
        return ExpressionHelpers.ApplyOrderBy(source, _alias.Selector, _alias.PropertyType, _descending);
    }
}

/// <summary>
/// Evaluates the sort input against the configured aliases and default sort.
/// </summary>
internal sealed class SortRule<TEntity, TInput>
{
    private readonly Func<TInput, string?> _sortAccessor;
    private readonly IReadOnlyDictionary<string, SortAlias<TEntity>> _aliases;
    private readonly (string Alias, bool Descending)? _default;

    internal SortRule(
        Func<TInput, string?> sortAccessor,
        IReadOnlyDictionary<string, SortAlias<TEntity>> aliases,
        (string Alias, bool Descending)? defaultSort)
    {
        _sortAccessor = sortAccessor;
        _aliases = aliases;
        _default = defaultSort;
    }

    public (SortApplication<TEntity>? Sort, QueryContractError? Error) Evaluate(TInput input)
    {
        string? sortInput = _sortAccessor(input);
        string trimmed = sortInput?.Trim() ?? string.Empty;

        string alias;
        bool descending;

        if (trimmed.Length == 0)
        {
            if (_default is not null)
            {
                alias = _default.Value.Alias;
                descending = _default.Value.Descending;
            }
            else
            {
                return (null, null);
            }
        }
        else
        {
            descending = trimmed.StartsWith('-');
            alias = descending ? trimmed[1..] : trimmed;
        }

        if (!_aliases.TryGetValue(alias, out var sortAlias))
        {
            return (null, new QueryContractError(
                QueryContractErrorCode.UnknownSort,
                $"Unknown sort alias '{alias}'.",
                "Sort",
                sortInput));
        }

        return (new SortApplication<TEntity>(sortAlias, descending), null);
    }
}
