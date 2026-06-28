namespace QueryContracts;

/// <summary>
/// Represents the kind of error produced during query contract validation.
/// </summary>
public enum QueryContractErrorCode
{
    /// <summary>
    /// The requested sort alias is not declared in the contract.
    /// </summary>
    UnknownSort,

    /// <summary>
    /// The page number is invalid (must be greater than 0).
    /// </summary>
    InvalidPage,

    /// <summary>
    /// The page size is invalid (must be greater than 0).
    /// </summary>
    InvalidPageSize,

    /// <summary>
    /// The page size exceeds the maximum allowed by the contract.
    /// </summary>
    PageSizeTooLarge,
}
