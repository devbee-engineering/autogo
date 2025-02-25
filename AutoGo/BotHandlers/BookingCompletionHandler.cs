using Agoda.IoC.Core;
using AutoGo.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AutoGo.BotHandlers;

public interface IBookingCompletionHandler
{
    Task Handle(TelegramBotClient telegramBot, UserCommandState state, PhotoSize photoSize, CancellationToken cancellationToken);
}
[RegisterSingleton]
public class BookingCompletionHandler(IServiceProvider serviceProvider, IAdminNotificationHandler adminNotificationHandler) : IBookingCompletionHandler
{
    private const string completeBookingCommand = "/complete_booking";
    public async Task Handle(TelegramBotClient telegramBot, UserCommandState state, PhotoSize photoSize, CancellationToken cancellationToken)
    {
        if (state.State == UserState.WaitingForAttachment)
        {
            var fileId = photoSize.FileId;

            var bookingId = state.AddtionalInfo.CurrentBookingId.Value;

            var scope = serviceProvider.CreateScope();
            var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            await bookingService.Attach(bookingId, fileId);
            var booking = await bookingService.Get(bookingId);
            var admin = await userService.GetUser(booking.CreatedById);
            var driver = await userService.GetUser(booking.AssignedToId.Value);


            await adminNotificationHandler.NotifyAdminAboutBookingCompletion(fileId, booking, driver, admin);


        }
    }

}
