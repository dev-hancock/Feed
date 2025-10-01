using Feed.Events;
using Feed.Models;
using Feed.Persistence;
using Feed.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Feed.Jobs;

public class ScrapeJob(FeedDbContext db, IScrapeService service, IEventStream events) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var id = Guid.Parse(context.JobDetail.Key.Name);

        var plan = db.Plans.Include(plan => plan.Artifacts).First(x => x.Id == id);

        if (!plan.IsActive)
        {
            return;
        }

        plan.CheckedNow();

        var results = await service.ScrapeAsync(plan, context.CancellationToken);

        await db.SaveChangesAsync(context.CancellationToken);

        // Temporary code to simulate publishing events
        foreach (var artifact in plan.Artifacts)
        {
            await events.Publish(
                new ArtifactChangedEvent
                {
                    Id = id.ToString(),
                    Item = "Item",
                    Title = "Title",
                    Link = "Link",
                    Description = "Description",
                    Date = "Date"
                });
        }
    }
}