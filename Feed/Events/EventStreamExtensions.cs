namespace Feed.Events;

public static class EventStreamExtensions
{
    public static IDisposable Subscribe<T>(this IAsyncEnumerable<T> stream, Func<T, Task> action, CancellationToken token = default)
    {
        return stream.Subscribe((item, _) => action.Invoke(item), token);
    }

    public static IDisposable Subscribe<T>(this IAsyncEnumerable<T> stream, Func<T, CancellationToken, Task> action, CancellationToken token = default)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        _ = Task.Run(async () =>
        {
            await foreach (var item in stream.WithCancellation(cts.Token))
            {
                try
                {
                    await action.Invoke(item, cts.Token);
                }
                catch
                {
                    // ignore
                }
            }
        }, cts.Token);

        return new Disposable(cts);
    }

    private sealed class Disposable(CancellationTokenSource cts) : IDisposable
    {
        public void Dispose()
        {
            cts.Cancel();
        }
    }
}