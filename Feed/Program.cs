using Feed.Events;
using Feed.Features.Feeds;
using Feed.Grpc;
using Feed.Grpc.Services.v1;
using Feed.Interceptors;
using Feed.interfaces;
using Feed.Persistence;
using Feed.Services;
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

        services.AddGrpc(opt =>
        {
            opt.Interceptors.Add<ExceptionInterceptor>();
        });
        services.AddGrpcReflection();

        services.AddSingleton<IEventStream, EventStream>();

        services.AddCors(o => o.AddPolicy("AllowAll", builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
        }));

        services.AddSingleton<IScrapeService, ScrapeService>();
        services.AddHostedService<MigrationService>();

        services.AddTransient<IValidator<CreateFeedRequest>, CreateFeedValidator>();

        services.AddTransient<CreateFeedHandler>();
        services.AddTransient<DeleteFeedHandler>();
        services.AddTransient<StreamFeedHandler>();
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