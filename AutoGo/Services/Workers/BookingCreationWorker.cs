using AutoGo.BotHandlers;

namespace AutoGo.Services.Workers
{
    public class BookingCreationWorker(ILogger<BookingCreationWorker> logger,
        IServiceProvider serviceProvider,
        IUnprocessedBookingQueue unprocessedBookingQueue,
        IDriverNotificationHandler driverNotificationHandler) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = serviceProvider.CreateScope();
            var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            while (!stoppingToken.IsCancellationRequested)
            {

                unprocessedBookingQueue.Dequeue(out long bookingId);

                if (bookingId == 0)
                {
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                var booking = await bookingService.Get(bookingId);

                logger.LogInformation("starting to process bookingId {id}", bookingId);

                var activeDrivers = await userService.GetAllActiveDrivers();

                foreach (var driver in activeDrivers.Where(x => x.OrganizationIds.Contains(booking.OrganizationId)))
                {
                    await driverNotificationHandler.NotifyDriverForNewBooking(booking, driver);
                }

            }
        }
    }
}
