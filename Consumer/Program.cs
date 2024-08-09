using Consumer.Consumers;
using MassTransit;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    services.AddSingleton<IConfiguration>(hostContext.Configuration);

    var rmqHost = hostContext.Configuration.GetValue<string>("RabbitMq:Host") ?? "localhost";
    var usingCluster = (rmqHost.ToLower().Trim() == "virtual-host") ? true : false;
    
    var userName = hostContext.Configuration.GetValue<string>("RabbitMq:UserName") ?? "guest";
    var password = hostContext.Configuration.GetValue<string>("RabbitMq:Password") ?? "guest";
    var numNodes = 0;

    int messageLockInMinutes = hostContext.Configuration.GetValue<int>("MessageLockInMunutes");
    services.AddMassTransit(busConfig =>
    {
        busConfig.AddConfigureEndpointsCallback((name, cfg) =>
        {
            if (cfg is IRabbitMqReceiveEndpointConfigurator configurator)
            {
                if (usingCluster)
                {
                    Console.WriteLine($"Forcing all RabbitMq queues to be Quorum");
                    configurator.SetQuorumQueue();
                }

                if (messageLockInMinutes > 0)
                {
                    var timeoutMs = messageLockInMinutes * 60 * 1000;
                    Console.WriteLine($"Setting the message x-consumer-timeout for message locks for {name} to {messageLockInMinutes} minute(s), {timeoutMs} Ms");
                    configurator.SetQueueArgument("x-consumer-timeout", timeoutMs.ToString());
                }
            }
        });

        busConfig.AddDelayedMessageScheduler();

        busConfig.AddConsumer<HelloWorldConsumer>(typeof(HelloWorldConsumerDefinition));
        busConfig.AddConsumer<ScheduledHelloWorldConsumer>(typeof(ScheduledHelloWorldConsumerDefinition));

        busConfig.UsingRabbitMq((context, cfg) =>
        {
            cfg.UseDelayedMessageScheduler();

            cfg.Host(rmqHost, "/", h =>
            {
                h.Username(userName);
                h.Password(password);

                if (usingCluster)
                {
                    // Get a list of up to 3 nodes
                    var node1 = hostContext.Configuration.GetValue<string>("RabbitMq:ClusterNodes:Node1");
                    var node2 = hostContext.Configuration.GetValue<string>("RabbitMq:ClusterNodes:Node2");
                    var node3 = hostContext.Configuration.GetValue<string>("RabbitMq:ClusterNodes:Node3");
                    h.UseCluster(cluster =>
                    {
                        var nodes = new List<string>();
                        if (!string.IsNullOrEmpty(node1))
                            nodes.Add(node1);
                        if (!string.IsNullOrEmpty(node2))
                            nodes.Add(node2);
                        if (!string.IsNullOrEmpty(node3))
                            nodes.Add(node3);
                        numNodes = nodes.Count;
                        foreach(var node in nodes)
                        {
                            cluster.Node(node);
                        }
                    });
                }

                //cfg.Message<HelloWorld>(x => x.SetEntityName("HelloWorldExchange"));
                //cfg.Publish<HelloWorld>(x => x.ExchangeType = ExchangeType.Direct);

                cfg.ConfigureEndpoints(context);
            });
        });
    });
});

var host = builder.Build();
host.Run();
