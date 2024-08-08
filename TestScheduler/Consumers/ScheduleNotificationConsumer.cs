using MassTransit;

namespace TestScheduler.Consumers
{
    public class ScheduleNotificationConsumer : IConsumer<ScheduleNotification>
    {
        public async Task Consume(ConsumeContext<ScheduleNotification> context)
        {
            Console.WriteLine($"Received ScheduleNotification message at {DateTime.UtcNow.ToLongTimeString()} with Body [{context.Message.Body}]");

            Uri notificationService = new Uri("queue:notification-service");
            await context.ScheduleSend<SendNotification>(notificationService,
                context.Message.DeliveryTime, new()
                {
                    EmailAddress = context.Message.EmailAddress,
                    Body = context.Message.Body
                });

            Console.WriteLine($"Sent scheduled SendNotification message scheduled for: {context.Message.DeliveryTime.ToLongTimeString()}");
        }
    }
}
