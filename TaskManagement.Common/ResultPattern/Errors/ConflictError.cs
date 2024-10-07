namespace TaskManagement.Common.ResultPattern.Errors;

public class ConflictError(string message, string entity, string? target = null, string? details = null)
    : Error(ErrorCodes.Conflict, message, target, details)
{
    public string Entity { get; } = entity;

    public override string ToString()
    {
        return $"Conflict Error: Entity={Entity}, Message={Message}, Target={Target}, Details={Details}, Timestamp={Timestamp}";
    }
}