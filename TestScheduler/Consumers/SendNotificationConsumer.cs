using MassTransit;

namespace TestScheduler.Consumers
{
    public class SendNotificationConsumer : IConsumer<SendNotification>
    {
        public async Task Consume(ConsumeContext<SendNotification> context)
        {
            Console.WriteLine($"Received SendNotification message with Body [{context.Message.Body}] at {DateTime.UtcNow.ToLongTimeString()}");
        }
    }
}
