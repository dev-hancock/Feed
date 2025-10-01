using Feed.Grpc;
using Feed.interfaces;
using Feed.Jobs;
using Feed.Models;
using Feed.Persistence;
using Quartz;

namespace Feed.Features.Feeds
{
    public class CreateFeedValidator : IValidator<CreateFeedRequest>
    {
        public void Validate(CreateFeedRequest request)
        {
            // todo
        }
    }

    public class CreateFeedHandler(IValidator<CreateFeedRequest> validator, FeedDbContext context, ISchedulerFactory factory)
    {
        public async Task<CreateFeedResponse> Handle(CreateFeedRequest request, CancellationToken token = default)
        {
            validator.Validate(request);
            
            var plan = Plan.Create(request.Url, request.Interval);

            foreach (var selector in request.Selectors)
            {
                plan.AddArtifact(
                    selector.Item,
                    selector.Title,
                    selector.Link,
                    selector.Description,
                    selector.Date);
            }

            var detail = JobBuilder.Create<ScrapeJob>()
                .WithIdentity(plan.Id.ToString())
                .Build();

            var trigger = TriggerBuilder.Create()
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(plan.Interval)
                    .RepeatForever())
                .Build();

            context.Add(plan);

            await context.SaveChangesAsync(token);

            var scheduler = await factory.GetScheduler(token);

            await scheduler.ScheduleJob(detail, trigger, token);

            return new CreateFeedResponse
            {
                Id = plan.Id.ToString()
            };
        }
    }
}
