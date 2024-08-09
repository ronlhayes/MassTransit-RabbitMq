using Contracts;
using MassTransit;

namespace Consumer.Consumers
{
    public class ScheduledHelloWorldConsumer : IConsumer<ScheduledHelloWorld>
    {
        private readonly IConfiguration _configuration;

        public ScheduledHelloWorldConsumer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Consume(ConsumeContext<ScheduledHelloWorld> context)
        {
            Console.WriteLine($"Received ScheduledHelloWorld message at {DateTime.UtcNow.ToString()} for [{context.Message.Name}] with MessageId: {context.MessageId}");
        }
    }

    public class ScheduledHelloWorldConsumerDefinition : ConsumerDefinition<ScheduledHelloWorldConsumer>
    {
        public ScheduledHelloWorldConsumerDefinition()
        {
            EndpointName = "ScheduledHelloWorld";
            ConcurrentMessageLimit = 1;
        }

        //protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<HelloWorldConsumer> consumerConfigurator)
        //{
        //}
    }
}
