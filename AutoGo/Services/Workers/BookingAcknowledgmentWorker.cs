using AutoGo.BotHandlers;

namespace AutoGo.Services.Workers
{
    public class BookingAcknowledgmentWorker(ILogger<BookingAcknowledgmentWorker> logger,
    IServiceProvider serviceProvider,
    IAcknowledgeBookingQueue acknowledgeBookingQueue,
    IDriverNotificationHandler driverNotificationHandler,
    IAdminNotificationHandler adminNotificationHandler) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = serviceProvider.CreateScope();
            var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            while (!stoppingToken.IsCancellationRequested)
            {
                // Dequeue acknowledgment
                acknowledgeBookingQueue.Dequeue(out AckDetails ack);

                if (ack == null)
                {
                    await Task.Delay(1000, stoppingToken); // Wait before checking again
                    continue;
                }

                logger.LogInformation("Processing acknowledgment for bookingId {id} ",
                    ack.BookingId);

                var booking = await bookingService.Get(ack.BookingId);
                var user = await userService.GetUserByTelegramId(ack.DriverTelegramId);
                var admin = await userService.GetUser(booking.CreatedById);

                if (booking.Status == Data.Entities.BookingStatus.Pending)
                {
                    await bookingService.Assign(ack.BookingId, user.Id);

                    logger.LogInformation("BookingId {id} successfully assigned to user {driverId}",
                    ack.BookingId, user.Id);
                    await driverNotificationHandler.NotifyDriverForBookingAssignment(booking, user);
                    await adminNotificationHandler.NotifyAdminAboutBookingAssignment(booking, user, admin);

                }
                else
                {
                    await driverNotificationHandler.NotifyDriverForBookingAssignmentFailure(booking, user);
                }




            }
        }
    }

    public class Acknowledgment
    {
        public long BookingId { get; set; }
        public long DriverId { get; set; }
    }
}
