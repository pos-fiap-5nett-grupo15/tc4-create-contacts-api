using CreateContact.Infrastructure.Crypto;
using CreateContact.Infrastructure.UnitOfWork;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateContact.Infrastructure.HealthCheck
{
    public class SqlConnectionHealthCheck : BaseConnection, IHealthCheck
    {
        private string _healthQuery;

        public SqlConnectionHealthCheck(IConfiguration configuration, ICryptoService cryptoService, string heathQuery = "SELECT 1") : base(configuration, cryptoService)
        {
            this._healthQuery = heathQuery;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            try
            {
                using (var command = this.InitializeConnection().ExecuteScalarAsync(new CommandDefinition(_healthQuery, cancellationToken)))
                {
                    command.Wait();
                }
                return HealthCheckResult.Healthy("SQL Server is healthy.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("SQL Server is unhealthy.", ex);
            }
        }
    }
}
