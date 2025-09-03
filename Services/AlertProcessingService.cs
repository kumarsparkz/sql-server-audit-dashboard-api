using AuditDashboard.Services;

namespace AuditDashboard.Services
{
    public class AlertProcessingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AlertProcessingService> _logger;

        public AlertProcessingService(IServiceProvider serviceProvider, ILogger<AlertProcessingService> logger)
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
                    var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();
                    
                    await alertService.ProcessAlertsAsync();
                    
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in alert processing service");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }
    }
}