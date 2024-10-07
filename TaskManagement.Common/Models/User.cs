using Ardalis.GuardClauses;

namespace TaskManagement.Common.Models;

public class User(Guid id, string username, string email)
{
    public Guid Id { get; init; } = Guard.Against.Default(id);
    public string Username { get; private set; } = Guard.Against.NullOrWhiteSpace(username);
    public string Email { get; private set; } = Guard.Against.NullOrWhiteSpace(email);
    public ICollection<Task> Tasks { get; private set; }

    public void UpdateUsername(string username) => Username = Guard.Against.NullOrWhiteSpace(username);
    public void UpdateEmail(string email) => Email = Guard.Against.NullOrWhiteSpace(email);

}