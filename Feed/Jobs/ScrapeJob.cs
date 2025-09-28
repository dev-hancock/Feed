using Feed.Consumers;
using Feed.Models;
using Feed.Persistence;
using Feed.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Feed.Jobs
{
    public class ScrapeJob(FeedDbContext db, IScrapeService service, IPublishEndpoint endpoint) : IJob
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

            // For now this is ok, this will be raised through domain events
            foreach (var artifact in plan.Artifacts)
            {
                await endpoint.Publish(
                    new ArtifactChangedEvent
                    {
                        Id = id,
                        Item = "Item",
                        Title = "Title",
                        Link = "Link",
                        Description = "Description",
                        Date = "Date"
                    });
            }
        }
    }
}
