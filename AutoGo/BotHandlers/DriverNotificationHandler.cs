using Agoda.IoC.Core;
using AutoGo.Constants;
using AutoGo.Data.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AutoGo.BotHandlers
{
    public interface IDriverNotificationHandler
    {
        Task NotifyDriverForBookingAssignment(Booking booking, Data.Entities.User user);
        Task NotifyDriverForBookingAssignmentFailure(Booking booking, Data.Entities.User user);
        Task NotifyDriverForNewBooking(Booking booking, Data.Entities.User user);
    }
    [RegisterSingleton]
    public class DriverNotificationHandler(ILogger<TelegramWorker> logger,ITelegramBotClientProvider telegramBotClientProvider) : IDriverNotificationHandler
    {
        private readonly TelegramBotClient botClient = telegramBotClientProvider.Get();

        public async Task NotifyDriverForNewBooking(Booking booking, Data.Entities.User user)
        {
            var bookingId = booking.Id;

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(DriverMessages.Accept, $"accept:{bookingId}"),
                InlineKeyboardButton.WithCallbackData(DriverMessages.Decline, $"decline:{bookingId}")
            }
             });

            await botClient.SendVoice(
                        chatId: user.TelegramUserId,
                        voice: InputFile.FromFileId(booking.VoiceFileId),
                        caption: DriverMessages.NewBookingMessage(bookingId),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: inlineKeyboard // Attach the inline keyboard
                    );
            logger.LogInformation("Sent booking {bookingId} creation notification to user: {userId}", bookingId, user.Id);
        }

        public async Task NotifyDriverForBookingAssignment(Booking booking, Data.Entities.User user)
        {
            var bookingId = booking.Id;

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
            new[]
            {
                 InlineKeyboardButton.WithCallbackData(DriverMessages.Start, $"start:{booking.Id}"),
                 InlineKeyboardButton.WithCallbackData(DriverMessages.Complete, $"complete:{booking.Id}")
            }
             });

            await botClient.SendVoice(
                        chatId: user.TelegramUserId,
                        voice: InputFile.FromFileId(booking.VoiceFileId),
                        caption: DriverMessages.BookingAssignmentMessage(bookingId, booking.MobileNumber),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: inlineKeyboard // Attach the inline keyboard
                    );
            logger.LogInformation("Sent booking {bookingId} assignment notification to user: {userId}", bookingId, user.Id);
        }

        public async Task NotifyDriverForBookingAssignmentFailure(Booking booking, Data.Entities.User user)
        {
            var bookingId = booking.Id;

            await botClient.SendMessage(
                        chatId: user.TelegramUserId,
                        text: DriverMessages.BookingAssignmentFailureMessage(bookingId),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                    );
            logger.LogInformation("Sent booking {bookingId} assignment Failure notification to user: {userId}", bookingId, user.Id);
        }
    }
}
