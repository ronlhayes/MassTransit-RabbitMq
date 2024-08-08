using MassTransit;
using TestScheduler;
using TestScheduler.Consumers;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    services.AddMassTransit(x =>
    {
        Uri schedulerEndpoint = new Uri("queue:scheduler");
        x.AddMessageScheduler(schedulerEndpoint);

        x.AddConsumer<ScheduleNotificationConsumer>();
        x.AddConsumer<SendNotificationConsumer>();

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.UseMessageScheduler(schedulerEndpoint);
            cfg.ConfigureEndpoints(context);
        });
    });

    services.AddHostedService<Worker>();
});


var host = builder.Build();
host.Run();
