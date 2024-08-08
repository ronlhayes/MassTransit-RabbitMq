using MassTransit;

namespace TestScheduler.Consumers
{
    public class ScheduleNotificationConsumer : IConsumer<ScheduleNotification>
    {
        public async Task Consume(ConsumeContext<ScheduleNotification> context)
        {
            Console.WriteLine($"Received ScheduleNotification message with body [{context.Message.Body}] with MessageId: {context.MessageId}");

            Uri notificationService = new Uri("queue:notification-service");
            await context.ScheduleSend<SendNotification>(notificationService,
                context.Message.DeliveryTime, new()
                {
                    EmailAddress = context.Message.EmailAddress,
                    Body = context.Message.Body
                });
            Console.WriteLine($"Sent scheduled SendNotification message to be sent at: {context.Message.DeliveryTime}");
        }
    }
}
