namespace TaskManagement.Common.ResultPattern.Errors;

public class ValidationError(string message, string field, string? details = null)
    : Error(ErrorCodes.ValidationError, message, field, details)
{
    public string Field { get; } = field;

    public override string ToString()
    {
        return $"Validation Error: Field={Field}, Message={Message}, Details={Details}, Timestamp={Timestamp}";
    }
}