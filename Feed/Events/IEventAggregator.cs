namespace Feed.Events;

public interface IEventAggregator
{
    public IAsyncEnumerable<T> Subscribe<T>(CancellationToken token = default) where T : IEvent;

    public Task Publish(IEvent value, CancellationToken token = default);
}