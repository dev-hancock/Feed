namespace Feed.Events;

public interface IAsyncObservable<out T>
{
    IAsyncEnumerable<T> Subscribe(CancellationToken token = default);
}