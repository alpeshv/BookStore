using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BookStore.Api
{
    public class DbHealthCheck: IHealthCheck
    {
        private readonly IDbConnection _dbConnection;

        public DbHealthCheck(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                _dbConnection.Open();
                _dbConnection.Close();

                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception exception)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(exception.Message));
            }
        }
    }
}
