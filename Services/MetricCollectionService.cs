using AuditDashboard.Services;

namespace AuditDashboard.Services
{
    public class MetricCollectionService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MetricCollectionService> _logger;

        public MetricCollectionService(IServiceProvider serviceProvider, ILogger<MetricCollectionService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var monitoringService = scope.ServiceProvider.GetRequiredService<IMonitoringService>();
                    
                    // Collect metrics for all active servers
                    await monitoringService.CollectServerMetricsAsync(0); // 0 for all servers
                    
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in metric collection service");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }
    }
}