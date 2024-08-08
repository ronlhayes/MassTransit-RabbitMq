using MassTransit;
using TestScheduler.Consumers;
using TestScheduler.HostedServices;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    services.AddMassTransit(x =>
    {
        x.AddDelayedMessageScheduler();
        x.AddConsumer<ScheduleNotificationConsumer>();
        x.AddConsumer<SendNotificationConsumer>();
        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.UseDelayedMessageScheduler();
            cfg.ConfigureEndpoints(context);
        });
    });

    // Hosted service to send a scheduled message
    services.AddHostedService<Worker>();
});

var host = builder.Build();
host.Run();
