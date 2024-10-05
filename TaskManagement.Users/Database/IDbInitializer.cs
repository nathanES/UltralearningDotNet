namespace TaskManagement.Users.Database;

public interface IDbInitializer
{
    Task InitializeAsync();
}