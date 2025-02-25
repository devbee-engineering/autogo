using Agoda.IoC.Core;
using System.Collections.Concurrent;

namespace AutoGo.Services.Workers
{
    public interface IAcknowledgeBookingQueue
    {
        void Dequeue(out AckDetails ackDetails);
        void Enqueue(long bookingId, long driverId);
    }

    [RegisterSingleton]
    public class AcknowledgeBookingQueue : IAcknowledgeBookingQueue
    {
        private readonly ConcurrentQueue<AckDetails> ackQueue = new ConcurrentQueue<AckDetails>();

        public void Enqueue(long bookingId, long driverId)
        {
            ackQueue.Enqueue(new AckDetails()
            {
                DriverTelegramId = driverId,
                BookingId = bookingId,
            });
        }

        public void Dequeue(out AckDetails ackDetails)
        {
            ackQueue.TryDequeue(out ackDetails);
        }
    }

    public class AckDetails
    {
        public long DriverTelegramId { get; set; }
        public long BookingId { get; set; }
    }
}
