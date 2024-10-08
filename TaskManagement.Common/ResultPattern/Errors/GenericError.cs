namespace TaskManagement.Common.ResultPattern.Errors;

public class GenericError(string message, string? details = null) : Error(ErrorCodes.GenericError, message, null, details)
{
    public override string ToString()
    {
        return $"Generic Error: Message={Message}, Details={Details}, Timestamp={Timestamp}";
    }
}