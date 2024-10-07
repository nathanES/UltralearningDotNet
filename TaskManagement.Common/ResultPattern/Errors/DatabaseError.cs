namespace TaskManagement.Common.ResultPattern.Errors;

public class DatabaseError(string message, string entity, string? target, string? details) 
    : Error(ErrorCodes.DatabaseError, message, target, details)
{
    public string Entity { get; } = entity ?? throw new ArgumentNullException(nameof(entity));
    public override string ToString()
    {
        return $"Database Error: Entity={Entity}, Message={Message}, Target={Target}, Details={Details}, Timestamp={Timestamp}";
    }
}