using System.Threading.Channels;

namespace Feed.Events;

public class EventStream : IEventStream
{
    private readonly Lock _lock = new();

    private readonly List<Channel<IEvent>> _subscribers = [];

    public async Task Publish(IEvent value, CancellationToken token = default)
    {
        Channel<IEvent>[] snapshot;

        lock (_lock)
        {
            snapshot = _subscribers.ToArray();
        }

        var tasks = snapshot.Select(channel =>
        {
            try
            {
                return channel.Writer.WriteAsync(value, token).AsTask();
            }
            catch (InvalidOperationException)
            {
                return Task.CompletedTask;
            }
        });

        await Task.WhenAll(tasks);
    }

    public async IAsyncEnumerator<IEvent> GetAsyncEnumerator(CancellationToken token = default)
    {
        var channel = Channel.CreateUnbounded<IEvent>();

        lock (_lock)
        {
            _subscribers.Add(channel);
        }

        try
        {
            await foreach (var item in channel.Reader.ReadAllAsync(token))
            {
                yield return item;
            }
        }
        finally
        {
            lock (_lock)
            {
                _subscribers.Remove(channel);
            }

            channel.Writer.Complete();
        }
    }
}
