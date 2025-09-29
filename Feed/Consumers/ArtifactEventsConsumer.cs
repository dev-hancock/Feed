using Feed.Events;
using MassTransit;

namespace Feed.Consumers;

public class ArtifactEventsConsumer(IEventAggregator events) : IConsumer<ArtifactChangedEvent>
{
    public Task Consume(ConsumeContext<ArtifactChangedEvent> context)
    {
        return events.Publish(context.Message);
    }
}