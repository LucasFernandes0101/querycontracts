namespace QueryContracts.Internal;

/// <summary>
/// Evaluates pagination input against configured defaults and limits.
/// </summary>
internal sealed class PageRule<TInput>
{
    private readonly Func<TInput, int?> _pageAccessor;
    private readonly Func<TInput, int?> _pageSizeAccessor;
    private readonly int _maxSize;
    private readonly string _pageMemberName;
    private readonly string _pageSizeMemberName;

    internal PageRule(
        Func<TInput, int?> pageAccessor,
        Func<TInput, int?> pageSizeAccessor,
        int maxSize,
        string pageMemberName,
        string pageSizeMemberName)
    {
        _pageAccessor = pageAccessor;
        _pageSizeAccessor = pageSizeAccessor;
        _maxSize = maxSize;
        _pageMemberName = pageMemberName;
        _pageSizeMemberName = pageSizeMemberName;
    }

    public (int Page, int PageSize, IReadOnlyList<QueryContractError> Errors) Evaluate(TInput input)
    {
        int page = _pageAccessor(input) ?? 1;
        int pageSize = _pageSizeAccessor(input) ?? 20;
        var errors = new List<QueryContractError>();

        if (page <= 0)
        {
            errors.Add(new(
                QueryContractErrorCode.InvalidPage,
                "Page must be greater than 0.",
                _pageMemberName,
                page));
        }

        if (pageSize <= 0)
        {
            errors.Add(new(
                QueryContractErrorCode.InvalidPageSize,
                "Page size must be greater than 0.",
                _pageSizeMemberName,
                pageSize));
        }
        else if (pageSize > _maxSize)
        {
            errors.Add(new(
                QueryContractErrorCode.PageSizeTooLarge,
                $"Page size must not exceed {_maxSize}.",
                _pageSizeMemberName,
                pageSize));
        }

        return (page, pageSize, [.. errors]);
    }
}
