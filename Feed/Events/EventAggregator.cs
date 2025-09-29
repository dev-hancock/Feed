using System.Runtime.CompilerServices;

namespace Feed.Events;

public class EventAggregator : IEventAggregator
{
    private readonly AsyncSubject<object> _events = new();

    public Task Publish(IEvent value, CancellationToken token = default)
    {
        return _events.OnNext(value, token);
    }

    public async IAsyncEnumerable<T> Subscribe<T>([EnumeratorCancellation] CancellationToken token = default)
        where T : IEvent
    {
        await foreach (var value in _events.Subscribe(token))
        {
            if (value is T @event)
            {
                yield return @event;
            }
        }
    }
}