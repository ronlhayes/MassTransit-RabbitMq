using MassTransit;

namespace TestScheduler.Consumers
{
    public class ScheduleNotificationConsumer : IConsumer<ScheduleNotification>
    {
        public async Task Consume(ConsumeContext<ScheduleNotification> context)
        {
            Console.WriteLine($"Received ScheduleNotification message at {DateTime.UtcNow.ToLongTimeString()} with Body [{context.Message.Body}]");

            var deliveryTime = DateTime.UtcNow + TimeSpan.FromSeconds(5);
            await context.SchedulePublish<SendNotification>(deliveryTime, new()
                            {
                                EmailAddress = context.Message.EmailAddress,
                                Body = context.Message.Body
                            });

            Console.WriteLine($"Publishing scheduled SendNotification message scheduled for: {deliveryTime.ToLongTimeString()}");
        }
    }
}
