namespace TaskManagement.Common.ResultPattern.Errors;

public class UnauthorizedError(string message, string? target = null, string? details = null)
    : Error(ErrorCodes.Unauthorized, message, target, details)
{
    public override string ToString()
    {
        return $"Unauthorized Error: Message={Message}, Target={Target}, Details={Details}, Timestamp={Timestamp}";
    }
}