using Microsoft.EntityFrameworkCore;
using Project1.Controllers;

namespace Project1.Services
{
    public class StockMonitoringService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<StockMonitoringService> _logger;
        private readonly IEmailService _emailService;

        public StockMonitoringService(IServiceProvider provider,
                                      ILogger<StockMonitoringService> logger,
                                      IEmailService emailService)
        {
            _provider = provider;
            _logger = logger;
            _emailService = emailService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("StockMonitoringService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _provider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectDbContext>();
                        var products = await dbContext.Products.ToListAsync();

                        foreach (var product in products)
                        {
                            if (product.Stock < 10)
                            {
                                var emailSubject = "Stock Alert";
                                var emailBody = $"The stock of {product.Name} is running low. Current stock: {product.Stock}";

                                // Assuming you have an email address to send the alert to.
                                var adminEmail = "mwangiderrick27@gmail.com";

                                await _emailService.SendEmailAsync(adminEmail, emailSubject, emailBody);
                            }
                        }
                    }

                    // For demonstration purposes, let's log a message every 5 seconds.
                    _logger.LogInformation("Checking stock levels...");

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while checking stock levels.");
                }
            }

            _logger.LogInformation("StockMonitoringService is stopping.");
        }
    }
}
