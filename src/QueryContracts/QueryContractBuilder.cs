using System.Linq.Expressions;
using QueryContracts.Internal;

namespace QueryContracts;

/// <summary>
/// A mutable, fluent builder for constructing a <see cref="QueryContract{TEntity, TInput}"/>.
/// </summary>
/// <typeparam name="TEntity">The entity type being queried.</typeparam>
/// <typeparam name="TInput">The application input type.</typeparam>
public sealed class QueryContractBuilder<TEntity, TInput>
{
    private readonly List<FilterRule<TEntity, TInput>> _filters = [];
    private SortRule<TEntity, TInput>? _sort;
    private PageRule<TInput>? _page;

    internal QueryContractBuilder() { }

    /// <summary>
    /// Begins configuring a filter that reads from <paramref name="inputSelector"/>
    /// and applies a comparison against the entity property selected by <paramref name="entitySelector"/>.
    /// </summary>
    /// <typeparam name="TInputProperty">The property type on the input object.</typeparam>
    /// <typeparam name="TEntityProperty">The property type on the entity.</typeparam>
    /// <param name="inputSelector">An expression that reads the filter value from the input.</param>
    /// <param name="entitySelector">An expression that selects the entity property to filter on.</param>
    /// <returns>A <see cref="FilterBuilder{TEntity, TInput, TInputProperty, TEntityProperty}"/>
    /// for choosing the comparison kind.</returns>
    public FilterBuilder<TEntity, TInput, TInputProperty, TEntityProperty> Filter<TInputProperty, TEntityProperty>(
        Expression<Func<TInput, TInputProperty>> inputSelector,
        Expression<Func<TEntity, TEntityProperty>> entitySelector)
        => new(this, inputSelector, entitySelector);

    /// <summary>
    /// Begins configuring sorting rules for the contract.
    /// </summary>
    /// <param name="sortSelector">An expression that reads the sort string from the input.</param>
    /// <returns>A <see cref="SortBuilder{TEntity, TInput}"/> for declaring sort aliases.</returns>
    public SortBuilder<TEntity, TInput> Sort(Expression<Func<TInput, string?>> sortSelector)
    {
        string sortMemberName = ExpressionHelpers.GetMemberName(sortSelector);
        return new(
            this,
            sortSelector,
            sortMemberName);
    }

    /// <summary>
    /// Configures pagination rules for the contract.
    /// </summary>
    /// <param name="pageSelector">An expression that reads the page number from the input.</param>
    /// <param name="pageSizeSelector">An expression that reads the page size from the input.</param>
    /// <param name="maxSize">The maximum allowed page size.</param>
    /// <returns>This builder for chaining.</returns>
    public QueryContractBuilder<TEntity, TInput> Page(
        Expression<Func<TInput, int?>> pageSelector,
        Expression<Func<TInput, int?>> pageSizeSelector,
        int maxSize)
    {
        string pageMemberName = ExpressionHelpers.GetMemberName(pageSelector);
        string pageSizeMemberName = ExpressionHelpers.GetMemberName(pageSizeSelector);

        _page = new PageRule<TInput>(
            pageSelector.Compile(),
            pageSizeSelector.Compile(),
            maxSize,
            pageMemberName,
            pageSizeMemberName);

        return this;
    }

    /// <summary>
    /// Builds an immutable <see cref="QueryContract{TEntity, TInput}"/> from the current configuration.
    /// </summary>
    /// <returns>A new, immutable contract instance.</returns>
    public QueryContract<TEntity, TInput> Build()
        => new([.. _filters], _sort, _page);

    internal void AddFilter(FilterRule<TEntity, TInput> filter) => _filters.Add(filter);

    internal void SetSort(SortRule<TEntity, TInput> sort) => _sort = sort;
}

/// <summary>
/// Intermediate builder returned by <see cref="QueryContractBuilder{TEntity, TInput}.Filter{TInputProperty, TEntityProperty}"/>
/// for selecting the filter comparison kind.
/// </summary>
public sealed class FilterBuilder<TEntity, TInput, TInputProperty, TEntityProperty>
{
    private readonly QueryContractBuilder<TEntity, TInput> _builder;
    private readonly Expression<Func<TInput, TInputProperty>> _inputSelector;
    private readonly Expression<Func<TEntity, TEntityProperty>> _entitySelector;

    internal FilterBuilder(
        QueryContractBuilder<TEntity, TInput> builder,
        Expression<Func<TInput, TInputProperty>> inputSelector,
        Expression<Func<TEntity, TEntityProperty>> entitySelector)
    {
        _builder = builder;
        _inputSelector = inputSelector;
        _entitySelector = entitySelector;
    }

    /// <summary>
    /// Adds an equality filter. When the input value is not <c>null</c>,
    /// the entity property is compared using <c>==</c>.
    /// </summary>
    /// <returns>The parent builder for chaining.</returns>
    public QueryContractBuilder<TEntity, TInput> Equals()
    {
        var compiled = _inputSelector.Compile();
        _builder.AddFilter(new FilterRule<TEntity, TInput, TEntityProperty>(
            input => compiled(input),
            _entitySelector,
            FilterKind.Equals));
        return _builder;
    }

    /// <summary>
    /// Adds a <c>string.Contains</c> filter. When the input value is not
    /// <c>null</c>, empty, or whitespace, the entity property is tested with
    /// <c>Contains</c>.
    /// </summary>
    /// <returns>The parent builder for chaining.</returns>
    /// <exception cref="InvalidOperationException">
    /// The entity property is not a <see cref="string"/>.
    /// </exception>
    public QueryContractBuilder<TEntity, TInput> Contains()
    {
        FilterBuilder<TEntity, TInput, TInputProperty, TEntityProperty>.EnsureStringEntityProperty(nameof(Contains));

        var compiled = _inputSelector.Compile();
        _builder.AddFilter(new FilterRule<TEntity, TInput, TEntityProperty>(
            input => compiled(input),
            _entitySelector,
            FilterKind.Contains));
        return _builder;
    }

    /// <summary>
    /// Adds a <c>string.StartsWith</c> filter. When the input value is not
    /// <c>null</c>, empty, or whitespace, the entity property is tested with
    /// <c>StartsWith</c>.
    /// </summary>
    /// <returns>The parent builder for chaining.</returns>
    /// <exception cref="InvalidOperationException">
    /// The entity property is not a <see cref="string"/>.
    /// </exception>
    public QueryContractBuilder<TEntity, TInput> StartsWith()
    {
        FilterBuilder<TEntity, TInput, TInputProperty, TEntityProperty>.EnsureStringEntityProperty(nameof(StartsWith));

        var compiled = _inputSelector.Compile();
        _builder.AddFilter(new FilterRule<TEntity, TInput, TEntityProperty>(
            input => compiled(input),
            _entitySelector,
            FilterKind.StartsWith));
        return _builder;
    }

    private static void EnsureStringEntityProperty(string methodName)
    {
        if (typeof(TEntityProperty) != typeof(string))
        {
            throw new InvalidOperationException(
                $"{methodName} can only be used with string entity properties.");
        }
    }

    /// <summary>
    /// Adds a greater-than-or-equal filter. When the input value is not <c>null</c>,
    /// the entity property is compared using <c>&gt;=</c>.
    /// </summary>
    /// <returns>The parent builder for chaining.</returns>
    public QueryContractBuilder<TEntity, TInput> GreaterThanOrEqual()
    {
        var compiled = _inputSelector.Compile();
        _builder.AddFilter(new FilterRule<TEntity, TInput, TEntityProperty>(
            input => compiled(input),
            _entitySelector,
            FilterKind.GreaterThanOrEqual));
        return _builder;
    }

    /// <summary>
    /// Adds a less-than-or-equal filter. When the input value is not <c>null</c>,
    /// the entity property is compared using <c>&lt;=</c>.
    /// </summary>
    /// <returns>The parent builder for chaining.</returns>
    public QueryContractBuilder<TEntity, TInput> LessThanOrEqual()
    {
        var compiled = _inputSelector.Compile();
        _builder.AddFilter(new FilterRule<TEntity, TInput, TEntityProperty>(
            input => compiled(input),
            _entitySelector,
            FilterKind.LessThanOrEqual));
        return _builder;
    }
}

/// <summary>
/// Intermediate builder returned by <see cref="QueryContractBuilder{TEntity, TInput}.Sort"/>
/// for declaring public sort aliases.
/// </summary>
public sealed class SortBuilder<TEntity, TInput>
{
    private readonly QueryContractBuilder<TEntity, TInput> _builder;
    private readonly Func<TInput, string?> _sortAccessor;
    private readonly string _sortMemberName;
    private readonly Dictionary<string, SortAlias<TEntity>> _aliases = [];
    private (string Alias, bool Descending)? _default;

    internal SortBuilder(
        QueryContractBuilder<TEntity, TInput> builder,
        Expression<Func<TInput, string?>> sortSelector,
        string sortMemberName)
    {
        _builder = builder;
        _sortAccessor = sortSelector.Compile();
        _sortMemberName = sortMemberName;
    }

    /// <summary>
    /// Declares a public sort alias mapped to an entity property.
    /// </summary>
    /// <typeparam name="TProperty">The entity property type.</typeparam>
    /// <param name="alias">The public alias name clients may use in the sort string.</param>
    /// <param name="selector">An expression that selects the entity property to sort by.</param>
    /// <returns>This builder for chaining additional aliases.</returns>
    public SortBuilder<TEntity, TInput> Allow<TProperty>(
        string alias,
        Expression<Func<TEntity, TProperty>> selector)
    {
        _aliases[alias] = new(alias, selector, typeof(TProperty));
        return this;
    }

    /// <summary>
    /// Sets the default sort applied when the sort input is <c>null</c>, empty, or whitespace.
    /// </summary>
    /// <param name="alias">The alias to use as the default. Must be previously declared with <see cref="Allow{TProperty}"/>.</param>
    /// <param name="descending">Whether the default sort should be descending.</param>
    /// <returns>The parent <see cref="QueryContractBuilder{TEntity, TInput}"/> for chaining.</returns>
    public QueryContractBuilder<TEntity, TInput> Default(string alias, bool descending = false)
    {
        _default = (alias, descending);

        _builder.SetSort(new(
            _sortAccessor,
            _aliases,
            _default,
            _sortMemberName));

        return _builder;
    }

    /// <summary>
    /// Configures pagination rules for the contract.
    /// </summary>
    /// <param name="pageSelector">An expression that reads the page number from the input.</param>
    /// <param name="pageSizeSelector">An expression that reads the page size from the input.</param>
    /// <param name="maxSize">The maximum allowed page size.</param>
    /// <returns>The parent <see cref="QueryContractBuilder{TEntity, TInput}"/> for chaining.</returns>
    public QueryContractBuilder<TEntity, TInput> Page(
        Expression<Func<TInput, int?>> pageSelector,
        Expression<Func<TInput, int?>> pageSizeSelector,
        int maxSize)
    {
        _builder.SetSort(new(
            _sortAccessor,
            _aliases,
            _default,
            _sortMemberName));

        return _builder.Page(pageSelector, pageSizeSelector, maxSize);
    }

    /// <summary>
    /// Builds an immutable <see cref="QueryContract{TEntity, TInput}"/> from the current configuration.
    /// </summary>
    /// <returns>A new, immutable contract instance.</returns>
    public QueryContract<TEntity, TInput> Build()
    {
        _builder.SetSort(new(
            _sortAccessor,
            _aliases,
            _default,
            _sortMemberName));

        return _builder.Build();
    }
}
