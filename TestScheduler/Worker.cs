using MassTransit;

namespace TestScheduler
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
                //while (!stoppingToken.IsCancellationRequested)
                //{
                    var messageId = Guid.NewGuid();

                    var scheduledMessage = new ScheduleNotification()
                    {
                        DeliveryTime = DateTime.UtcNow + TimeSpan.FromSeconds(5),
                        EmailAddress = "test@test.com",
                        Body = Guid.NewGuid().ToString()
                    };

                    await _bus.Publish<ScheduleNotification>(scheduledMessage, x =>
                    {
                        x.MessageId = messageId;
                    }, stoppingToken);
                    
                    Console.WriteLine($"Publishing message with Body: {scheduledMessage.Body} and MessageId: {messageId}");

                    await Task.Delay(1000, stoppingToken);
//                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Worker.ExecuteAsync: {ex.Message}");
            }
        }
    }
}
