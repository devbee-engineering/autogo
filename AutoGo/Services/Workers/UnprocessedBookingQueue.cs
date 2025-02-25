using Agoda.IoC.Core;
using System.Collections.Concurrent;

namespace AutoGo.Services.Workers;

public interface IUnprocessedBookingQueue
{
    void Dequeue(out long bookingId);
    void Enqueue(long bookingId);
}
[RegisterSingleton]
public class UnprocessedBookingQueue : IUnprocessedBookingQueue
{
    private readonly ConcurrentQueue<long> queue = new ConcurrentQueue<long>();

    public void Enqueue(long bookingId)
    {
        queue.Enqueue(bookingId);
    }

    public void Dequeue(out long bookingId)
    {
        queue.TryDequeue(out bookingId);
    }
}
