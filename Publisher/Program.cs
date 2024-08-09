using Contracts;
using MassTransit;
using Publisher;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);

var rmqHost = builder.Configuration.GetValue<string>("RabbitMq:Host") ?? "localhost";
var usingCluster = (rmqHost.ToLower().Trim() == "virtual-host") ? true : false;

var userName = builder.Configuration.GetValue<string>("RabbitMq:UserName") ?? "guest";
var password = builder.Configuration.GetValue<string>("RabbitMq:Password") ?? "guest";

builder.Services.AddMassTransit(x =>
{
    x.AddDelayedMessageScheduler();
    x.UsingRabbitMq((context, cfg) =>
    {
        // This is required when no receive endpoints are defined
        cfg.AutoStart = true;

        cfg.UseDelayedMessageScheduler();

        cfg.Host(rmqHost, "/", h =>
        {
            h.Username(userName);
            h.Password(password);

            if (rmqHost.ToLower().Trim() == "virtual-host")
            {
                // Get a list of up to 3 nodes
                var node1 = builder.Configuration.GetValue<string>("RabbitMq:ClusterNodes:Node1");
                var node2 = builder.Configuration.GetValue<string>("RabbitMq:ClusterNodes:Node2");
                var node3 = builder.Configuration.GetValue<string>("RabbitMq:ClusterNodes:Node3");
                h.UseCluster(cluster =>
                {
                    if(!string.IsNullOrEmpty(node1))
                        cluster.Node(node1);
                    if (!string.IsNullOrEmpty(node2))
                        cluster.Node(node2);
                    if (!string.IsNullOrEmpty(node3))
                        cluster.Node(node3);
                });
            }
        });
        //cfg.Message<HelloWorld>(x => x.SetEntityName("HelloWorldExchange"));
        //cfg.Publish<HelloWorld>(x => x.ExchangeType = ExchangeType.Direct);

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
