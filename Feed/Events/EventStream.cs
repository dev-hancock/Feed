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

    public IDisposable Subscribe<T>(Func<T, Task> action, CancellationToken token = default)
    {
        return Subscribe<T>((item, _) => action.Invoke(item), token);
    }

    public IDisposable Subscribe<T>(Func<T, CancellationToken, Task> action, CancellationToken token = default)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        _ = Task.Run(async () =>
        {
            await foreach (var item in this.WithCancellation(cts.Token))
            {
                if (item is not T typed)
                {
                    continue;
                }

                try
                {
                    await action.Invoke(typed, cts.Token);
                }
                catch
                {
                    // ignore
                }
            }
        }, cts.Token);

        return new CancellationDisposable(cts);
    }
}

public sealed class CancellationDisposable(CancellationTokenSource cts) : IDisposable
{
    public void Dispose()
    {
        cts.Cancel();
    }
}