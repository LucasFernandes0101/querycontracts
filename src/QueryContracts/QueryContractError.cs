namespace QueryContracts;

/// <summary>
/// Represents a structured validation error produced when applying a query contract.
/// </summary>
/// <param name="Code">The error code categorising the failure.</param>
/// <param name="Message">A human-readable description of the error.</param>
/// <param name="MemberName">The name of the input member that caused the error, if applicable.</param>
/// <param name="AttemptedValue">The value that was attempted, if applicable.</param>
public sealed record QueryContractError(
    QueryContractErrorCode Code,
    string Message,
    string? MemberName = null,
    object? AttemptedValue = null);
