using MassTransit;

namespace TestScheduler.HostedServices
{
    public class Worker : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<Worker> _logger;

        public Worker(IServiceProvider provider, ILogger<Worker> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var messageSent = false;
                while (!stoppingToken.IsCancellationRequested)
                {
                    // Send a single message
                    if (!messageSent)
                    {
                        var messageId = Guid.NewGuid();
                        var scheduledMessage = new ScheduleNotification()
                        {
                            DeliveryTime = DateTime.UtcNow + TimeSpan.FromSeconds(5),
                            EmailAddress = "frank@nul.org",
                            Body = "Thank you for signing up for our awesome newsletter!"
                        };

                        // Can't inject this service in the constructor because it's scoped
                        await using var scope = _provider.CreateAsyncScope();
                        var scheduler = scope.ServiceProvider.GetRequiredService<IMessageScheduler>();
                        await scheduler.SchedulePublish<ScheduleNotification>(scheduledMessage.DeliveryTime, scheduledMessage);

                        Console.WriteLine($"Publishing ScheduleNotification message with DeliveryTime: {scheduledMessage.DeliveryTime.ToLongTimeString()} and Body: {scheduledMessage.Body}");
                        messageSent = true;
                    }
                    await Task.Delay(10 * 1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Worker.ExecuteAsync: {ex.Message}");
            }
        }
    }
}
