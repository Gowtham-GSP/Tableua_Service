using TABLEAU_SERVICE;
using TABLEAU_SERVICE.Services;
using TABLEAU_SERVICE.Interface;
using Serilog;



IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .UseSerilog((hostingContext, services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostingContext.Configuration)
    .Enrich.FromLogContext())
    .ConfigureServices((context, services) =>
    {
        services.AddOptions<Configurations>().Bind(context.Configuration.GetSection(Configurations.SectionName)).ValidateDataAnnotations();
        services.AddTransient<IServiceEngine, ServiceEngine>();
        services.AddTransient<IDBHelper, DBHelper>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();