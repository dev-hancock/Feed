using System.Runtime.CompilerServices;
using Feed.Events;
using Feed.Exceptions;
using Feed.Grpc;
using Feed.Models;
using Feed.Persistence;

namespace Feed.Features.Feeds
{
    public class StreamFeedHandler(FeedDbContext context, IEventStream events)
    {
        public async IAsyncEnumerable<FeedEvent> Handle(StreamFeedRequest request, [EnumeratorCancellation] CancellationToken token = default)
        {
            var plan = await context.Plans.FindAsync([Guid.Parse(request.Id)], token);

            if (plan is null)
            {
                throw new NotFoundException("Plan.NotFound", "Plan not found.");
            }

            var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

            var updates = events.OfType<ArtifactChangedEvent>().Where(x => x.Id == request.Id);

            await foreach(var update in updates.WithCancellation(cts.Token))
            {
                var dto = new FeedEvent
                {
                    Item = update.Item,
                    Title = update.Title,
                    Link = update.Link,
                    Description = update.Description,
                    Date = update.Date
                };

                yield return dto;
            }
        }
    }
}
