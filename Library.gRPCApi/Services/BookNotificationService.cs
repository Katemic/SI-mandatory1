using System.Collections.Concurrent;
using System.Threading.Channels;
using Library.Data.Models;

namespace Library.gRPCApi.Services;

public interface IBookNotificationService
{
    IDisposable Subscribe(ChannelWriter<Tbook> writer);
    Task PublishAsync(Tbook book, CancellationToken cancellationToken);
}

public sealed class BookNotificationService : IBookNotificationService
{
    private readonly ConcurrentDictionary<Guid, ChannelWriter<Tbook>> _subscribers = new();

    public IDisposable Subscribe(ChannelWriter<Tbook> writer)
    {
        var subscriptionId = Guid.NewGuid();
        _subscribers[subscriptionId] = writer;

        return new Subscription(() => _subscribers.TryRemove(subscriptionId, out _));
    }

    public async Task PublishAsync(
        Tbook book,
        CancellationToken cancellationToken)
    {
        foreach (var subscriber in _subscribers)
        {
            await subscriber.Value.WriteAsync(book, cancellationToken);
        }
    }

    private sealed class Subscription : IDisposable
    {
        private readonly Action _unsubscribe;

        public Subscription(Action unsubscribe)
        {
            _unsubscribe = unsubscribe;
        }

        public void Dispose()
        {
            _unsubscribe();
        }
    }
}
