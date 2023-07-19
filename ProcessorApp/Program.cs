using DataPersistenceLayer.Entities;
using DataPersistenceLayer.Repositories;
using DataPersistenceLayer.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using ProcessorApp;
using ProcessorApp.Services;
using ProcessorApp.Services.Abstractions;
using SharedLibrary.Configs;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((builder, services) =>
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>(); 
        services.AddScoped<IHashRecordRepository, HashRecordRepository>(); 

        var rabbitMqSection = builder.Configuration.GetSection(RabbitMqConfiguration.ConfigName);
        services.Configure<RabbitMqConfiguration>(rabbitMqSection);

        services.AddScoped<IDataProcessorService, DataProcessorService>();

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
