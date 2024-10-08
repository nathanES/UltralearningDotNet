using TaskManagement.Common.Utils;

namespace TaskManagement.Common.ResultPattern.Errors;

public abstract class Error(string code, string message, string? target, string? details) : Comparable
{
    public string Code { get; } = code;
    public string Message { get; } = message;
    public string? Target { get; } = target;
    public string? Details { get; } = details; 
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public override string ToString()
    {
        return $"Error: {Code}, Message: {Message}, Target: {Target}, Details: {Details}, Timestamp: {Timestamp}";
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }
}

public class ErrorCodes
{
    public const string DatabaseError = "DATABASE_ERROR";
    public const string NotFound = "NOT_FOUND";
    public const string ValidationError = "VALIDATION_ERROR";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Conflict = "CONFLICT";
    public const string GenericError = "GENERIC_ERROR";
}