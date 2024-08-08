namespace TestScheduler
{
    public record ScheduleNotification
    {
        public DateTime DeliveryTime { get; init; }
        public string EmailAddress { get; init; }
        public string Body { get; init; }
    }

    public record SendNotification
    {
        public string EmailAddress { get; init; }
        public string Body { get; init; }
    }
}
