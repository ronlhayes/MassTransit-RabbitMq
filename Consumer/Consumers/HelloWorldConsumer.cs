using Contracts;
using MassTransit;

namespace Consumer.Consumers
{
    public class HelloWorldConsumer : IConsumer<HelloWorld>
    {
        private readonly IConfiguration _configuration;
        private readonly int _secondsToDelay;

        public HelloWorldConsumer(IConfiguration configuration)
        {
            _configuration = configuration;
            _secondsToDelay = _configuration.GetValue<int>("SecondsToDelay");
        }

        public async Task Consume(ConsumeContext<HelloWorld> context)
        {
            Console.WriteLine($"Received HelloWOrld message at {DateTime.UtcNow.ToLongTimeString()} for [{context.Message.Name}] with MessageId: {context.MessageId}");

            var deliveryTime = DateTime.UtcNow + TimeSpan.FromSeconds(5);
            await context.SchedulePublish<ScheduledHelloWorld>(deliveryTime, new ScheduledHelloWorld()
                                        {
                                            Name = context.Message.Name + " Scheduled"
                                        });
            Console.WriteLine($"Published ScheduledHelloWorld message for {deliveryTime.ToLongTimeString()} for [{context.Message.Name}] with MessageId: {context.MessageId}");

            if (_secondsToDelay > 0)
            {
                var minutesToDelay = _secondsToDelay / 60;
                if (minutesToDelay > 0)
                {
                    Console.WriteLine($"Delaying for {_secondsToDelay} seconds, ({minutesToDelay} minutes)...");
                    var startTime = DateTime.Now;
                    for (int count = 0; count < minutesToDelay; ++count)
                    {
                        await Task.Delay(60 * 1000);
                        var duration = DateTime.Now - startTime;
                        Console.WriteLine($"Siumlating long processing: {duration.TotalMinutes:N0} minute(s) delayed so far...");
                    }
                }
                else
                {
                    Console.WriteLine($"Delaying for {_secondsToDelay} seconds...");
                    await Task.Delay(_secondsToDelay * 1000);
                }
            }
        }
    }

    public class HelloWorldConsumerDefinition : ConsumerDefinition<HelloWorldConsumer>
    {
        public HelloWorldConsumerDefinition()
        {
            EndpointName = "HelloWorld";
            ConcurrentMessageLimit = 1;
        }

        //protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<HelloWorldConsumer> consumerConfigurator)
        //{
        //}
    }
}
