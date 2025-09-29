using Feed.Consumers;
using Feed.Events;
using Feed.Jobs;
using Feed.Models;
using Feed.Persistence;
using Grpc.Core;
using Quartz;

namespace Feed.Grpc.Services;

public class FeedService(FeedDbContext db, IEventAggregator events, ISchedulerFactory factory) : Feed.FeedBase
{
    public override async Task Subscribe(FeedRequest request, IServerStreamWriter<FeedEvent> response, ServerCallContext context)
    {
        var activitiy = Plan.Create(request.Url, request.Interval);

        foreach (var selector in request.Selectors)
        {
            activitiy.AddArtifact(
                selector.Item,
                selector.Title,
                selector.Link,
                selector.Description,
                selector.Date);
        }

        var key = new JobKey(activitiy.Id.ToString());

        var detail = JobBuilder.Create<ScrapeJob>()
            .WithIdentity(key)
            .Build();

        var trigger = TriggerBuilder.Create()
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(activitiy.Interval)
                .RepeatForever())
            .Build();

        db.Add(activitiy);

        await db.SaveChangesAsync(context.CancellationToken);

        var scheduler = await factory.GetScheduler(context.CancellationToken);

        await scheduler.ScheduleJob(detail, trigger);

        try
        {
            await foreach (var update in events.Subscribe<ArtifactChangedEvent>(context.CancellationToken))
            {
                var dto = new FeedEvent
                {
                    Item = update.Item,
                    Title = update.Title,
                    Link = update.Link,
                    Description = update.Description,
                    Date = update.Date
                };

                await response.WriteAsync(dto, context.CancellationToken);
            }
        }
        finally
        {
            activitiy.Deactivate();

            await scheduler.DeleteJob(key);

            await db.SaveChangesAsync();
        }
    }
}