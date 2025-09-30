namespace Feed.Events;

public interface IEvent;

public interface IEventStream : IAsyncEnumerable<IEvent>
{
    public Task Publish(IEvent value, CancellationToken token = default);
}