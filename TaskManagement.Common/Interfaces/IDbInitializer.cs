namespace TaskManagement.Common.Interfaces;

public interface IDbInitializer
{
    Task InitializeAsync(CancellationToken token = default);
    Task<bool> HealthCheckAsync(CancellationToken token = default);
}