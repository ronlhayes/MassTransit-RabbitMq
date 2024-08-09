using Contracts;
using MassTransit;
using static MassTransit.Monitoring.Performance.BuiltInCounters;
using static MassTransit.ValidationResultExtensions;

namespace Publisher
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
            var messagesSent = false;
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    if(!messagesSent)
                    {
                        for(int count = 0; count < 5; ++count)
                        {
                            var messageId = Guid.NewGuid();
                            var uniqueName = $"UniqueName-{messageId.ToString().Substring(0, 8)}";
                            var helloMessage = new HelloWorld()
                            {
                                Name = uniqueName
                            };
                            await _bus.Publish<HelloWorld>(helloMessage, x =>
                            {
                                x.MessageId = messageId;
                            }, stoppingToken);

                            Console.WriteLine($"Publishing HelloWorld message with Name: {uniqueName} and MessageId: {messageId}");
                            await Task.Delay(500, stoppingToken);
                        }
                        messagesSent = true;

                        await Task.Delay(10 * 1000, stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Worker.ExecuteAsync: {ex.Message}");
            }
        }
    }
}
