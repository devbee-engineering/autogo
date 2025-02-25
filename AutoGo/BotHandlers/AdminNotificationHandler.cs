using Agoda.IoC.Core;
using AutoGo.Constants;
using AutoGo.Data.Entities;
using Telegram.Bot;
using Telegram.Bots.Http;

namespace AutoGo.BotHandlers
{
    public interface IAdminNotificationHandler
    {
        Task NotifyAdminAboutBookingAssignment(Booking booking, User driver, User admin);
        Task NotifyAdminAboutBookingCompletion(string? fileId, Booking booking, User driver, User admin);
    }
    [RegisterSingleton]
    public class AdminNotificationHandler(ILogger<AdminNotificationHandler> logger,ITelegramBotClientProvider telegramBotClientProvider) : IAdminNotificationHandler
    {
        private readonly TelegramBotClient botClient = telegramBotClientProvider.Get();

        public async Task NotifyAdminAboutBookingAssignment(Booking booking, User driver, User admin)
        {
            var bookingId = booking.Id;
            var adminTelId = admin.TelegramUserId;

            await botClient.SendMessage(
                        chatId: adminTelId,
                        text: AdminMessages.BookingAssignedToDriverMessage(bookingId, driver.Name, driver.MobileNumber),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                    );
            logger.LogInformation("Sent booking {bookingId} assignment Notification to admin", bookingId);
        }

        public async Task NotifyAdminAboutBookingCompletion(string? fileId,Booking booking, User driver, User admin)
        {
            var bookingId = booking.Id;
            var adminTelId = admin.TelegramUserId;

            await botClient.SendPhoto(
                        photo: Telegram.Bot.Types.InputFile.FromFileId(fileId),
                        chatId: adminTelId,
                        caption: AdminMessages.BookingCompletedMessage(bookingId, driver.Name, driver.MobileNumber),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                    );
            logger.LogInformation("Sent booking {bookingId} completed Notification to admin", bookingId);
        }
    }
}
