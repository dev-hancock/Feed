namespace Feed.Events;

public interface IAsyncSubject<T> : IAsyncObservable<T>
{
    Task OnNext(T value, CancellationToken token = default);
}