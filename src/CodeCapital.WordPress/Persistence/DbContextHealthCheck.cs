using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Persistence
{
    public class DbContextHealthCheck : IHealthCheck
    {
        private readonly WordPressDbContext _dbContext;
        private int _millisecondsDelay = 1000 * 5;

        public DbContextHealthCheck(WordPressDbContext dbContext) => _dbContext = dbContext;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                var task = _dbContext.Database.CanConnectAsync(cancellationToken);

                _millisecondsDelay = 1000 * 5;
                return await Task.WhenAny(task, Task.Delay(_millisecondsDelay, cancellationToken)) == task ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                // throw;
                return HealthCheckResult.Unhealthy("Error while connecting to database", e);
            }

            //using (var cancellationTokenSource = new CancellationTokenSource(1000))
            //{
            //    try
            //    {
            //        //cancellationTokenSource.CancelAfter(1000 * 60);
            //        var result = await Task.WhenAny(_dbContext.Database.CanConnectAsync(cancellationTokenSource.Token));

            //        cancellationTokenSource.Cancel();

            //        return result.Result ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e);
            //        throw;
            //    }
            //}

            //return await _dbContext.Database.CanConnectAsync(cancellationToken)
            //    ? HealthCheckResult.Healthy()
            //    : HealthCheckResult.Unhealthy();
        }
    }
}
