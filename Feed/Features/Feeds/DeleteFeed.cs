using Feed.Exceptions;
using Feed.Grpc;
using Feed.Persistence;
using Quartz;

namespace Feed.Features.Feeds;

public class DeleteFeedHandler(FeedDbContext context, ISchedulerFactory factory)
{
    public async Task<DeleteFeedResponse> Handle(DeleteFeedRequest request, CancellationToken token = default)
    {
        var plan = await context.Plans.FindAsync([Guid.Parse(request.Id)], token);

        if (plan is null)
        {
            throw new NotFoundException("Plan.NotFound", "Plan not found.");
        }

        var scheduler = await factory.GetScheduler(token);

        var key = new JobKey(request.Id);

        var deleted = await scheduler.DeleteJob(key, token);

        if (!deleted)
        {
            throw new NotFoundException("Job.NotFound", "Job not found.");
        }

        plan.Deactivate();

        await context.SaveChangesAsync(token);

        return new DeleteFeedResponse();
    }
}