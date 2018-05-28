﻿using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static async Task Main()
    {
        var configuration = new EndpointConfiguration("FluentValidationSample");
        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();
        var validation = configuration.UseFluentValidation();
        validation.RegisterValidatorsFromAssemblyContaining<MyMessage>();

        var endpoint = await Endpoint.Start(configuration);

        await endpoint.SendLocal(new MyMessage());

        Console.WriteLine("Press any key to stop program");
        Console.Read();
        await endpoint.Stop();
    }
}