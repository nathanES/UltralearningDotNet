using Microsoft.Extensions.Diagnostics.HealthChecks;
using TaskManagement.Common.Interfaces;

namespace TaskManagement.Api.Health;

public class DatabaseHealthCheck(ILogger<DatabaseHealthCheck> logger, IEnumerable<IDbInitializer> dbInitializers) : IHealthCheck
{
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var dbInitializer in dbInitializers)
        {
            var canConnect = await dbInitializer.HealthCheckAsync(cancellationToken);
            if (!canConnect)
            {
                return HealthCheckResult.Unhealthy("Unable to connect to the database.");
            }
        }

        return HealthCheckResult.Healthy("Database connection is healthy");
    }
}