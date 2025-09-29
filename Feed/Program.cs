using Feed.Consumers;
using Feed.Events;
using Feed.Grpc.Services;
using Feed.Persistence;
using Feed.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Feed;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        Configure(app);

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FeedDbContext>(opt => { opt.UseNpgsql(configuration.GetConnectionString("Default")); });

        services.AddQuartz(x =>
        {
            x.UsePersistentStore(opt =>
            {
                opt.UseProperties = true;
                opt.UsePostgres(cfg => cfg.ConnectionString = configuration.GetConnectionString("quartz")!);
                opt.UseSystemTextJsonSerializer();
                opt.PerformSchemaValidation = true;
            });
        });
        services.AddQuartzHostedService(opt =>
        {
            opt.AwaitApplicationStarted = true;
            opt.WaitForJobsToComplete = true;
        });

        services.AddGrpc();
        services.AddGrpcReflection();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ArtifactEventsConsumer>();

            x.UsingInMemory((context, cfg) => { cfg.ConfigureEndpoints(context); });
        });

        services.AddSingleton<IEventAggregator, EventAggregator>();

        services.AddCors(o => o.AddPolicy("AllowAll", builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
        }));

        services.AddSingleton<IScrapeService, ScrapeService>();
        services.AddHostedService<MigrationService>();
    }

    private static void Configure(WebApplication app)
    {
        app.UseRouting();
        app.UseCors();
        app.UseGrpcWeb();

        app.MapGrpcService<FeedService>().EnableGrpcWeb();

        if (app.Environment.IsDevelopment())
        {
            app.MapGrpcReflectionService();
        }
    }
}