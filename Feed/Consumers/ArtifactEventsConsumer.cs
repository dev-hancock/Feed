using Feed.Events;
using MassTransit;

namespace Feed.Consumers
{
    public class ArtifactEventsConsumer(IEventAggregator events) : IConsumer<ArtifactChangedEvent>
    {
        public Task Consume(ConsumeContext<ArtifactChangedEvent> context)
        {
            events.Publish(context.Message);

            return Task.CompletedTask;
        }
    }
}
