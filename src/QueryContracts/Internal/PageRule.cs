namespace QueryContracts.Internal;

/// <summary>
/// Evaluates pagination input against configured defaults and limits.
/// </summary>
internal sealed class PageRule<TInput>
{
    private readonly Func<TInput, int?> _pageAccessor;
    private readonly Func<TInput, int?> _pageSizeAccessor;
    private readonly int _maxSize;

    internal PageRule(
        Func<TInput, int?> pageAccessor,
        Func<TInput, int?> pageSizeAccessor,
        int maxSize)
    {
        _pageAccessor = pageAccessor;
        _pageSizeAccessor = pageSizeAccessor;
        _maxSize = maxSize;
    }

    public (int Page, int PageSize, QueryContractError? Error) Evaluate(TInput input)
    {
        int page = _pageAccessor(input) ?? 1;
        int pageSize = _pageSizeAccessor(input) ?? 20;

        if (page <= 0)
        {
            return (page, pageSize, new QueryContractError(
                QueryContractErrorCode.InvalidPage,
                "Page must be greater than 0.",
                "Page",
                page));
        }

        if (pageSize <= 0)
        {
            return (page, pageSize, new QueryContractError(
                QueryContractErrorCode.InvalidPageSize,
                "Page size must be greater than 0.",
                "PageSize",
                pageSize));
        }

        if (pageSize > _maxSize)
        {
            return (page, pageSize, new QueryContractError(
                QueryContractErrorCode.PageSizeTooLarge,
                $"Page size must not exceed {_maxSize}.",
                "PageSize",
                pageSize));
        }

        return (page, pageSize, null);
    }
}
