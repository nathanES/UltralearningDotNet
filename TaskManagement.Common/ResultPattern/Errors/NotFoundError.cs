namespace TaskManagement.Common.ResultPattern.Errors;

public class NotFoundError(string entity, string? identifier = null, string? details = null)
    : Error(ErrorCodes.NotFound, $"{entity} was not found.", identifier, details)
{
    public string Entity { get; } = entity ?? throw new ArgumentNullException(nameof(entity));
    public string? Identifier { get; } = identifier;

    public override string ToString()
    {
        return $"Not Found Error: Entity={Entity}, Identifier={Identifier}, Details={Details}, Timestamp={Timestamp}";
    }
}