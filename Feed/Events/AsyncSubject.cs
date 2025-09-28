using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Feed.Events
{
    public class AsyncSubject<T> : IAsyncSubject<T>
    {
        private readonly List<Channel<T>> _subscribers = [];

        private readonly Lock _lock = new();

        public async Task OnNext(T value, CancellationToken token = default)
        {
            Channel<T>[] snapshot;

            lock (_lock)
            {
                snapshot = _subscribers.ToArray();
            }

            var tasks = snapshot.Select(ch =>
            {
                try
                {
                    return ch.Writer.WriteAsync(value, token).AsTask();
                }
                catch (InvalidOperationException)
                {
                    return Task.CompletedTask;
                }
            });

            await Task.WhenAll(tasks);
        }

        public async IAsyncEnumerable<T> Subscribe([EnumeratorCancellation] CancellationToken token = default)
        {
            var channel = Channel.CreateUnbounded<T>();

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
}
