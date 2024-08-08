using MassTransit;

namespace TestScheduler.HostedServices
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBus _bus;

        public Worker(ILogger<Worker> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
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
                        await _bus.Publish<ScheduleNotification>(scheduledMessage, stoppingToken);

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
