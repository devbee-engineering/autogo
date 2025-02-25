using Agoda.IoC.Core;
using AutoGo.Constants;
using AutoGo.Data.Entities;
using AutoGo.Services;
using AutoGo.Services.Workers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bots.Http;

namespace AutoGo.BotHandlers
{
    public interface IBookingCallBackHandler
    {
        Task Handle(TelegramBotClient botClient, CallBack ackStatus, CallbackQuery callbackQuery, CancellationToken cancellation);
    }
    [RegisterSingleton]
    public class BookingCallBackHandler(IAcknowledgeBookingQueue acknowledgeBookingQueue, IServiceProvider serviceProvider, ILogger<BookingCallBackHandler> logger, IUserStateService userStateService) : IBookingCallBackHandler
    {
        public async Task Handle(TelegramBotClient botClient, CallBack callBack, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var chatId = callbackQuery.Message?.Chat.Id;
            var messageId = callbackQuery.Message?.MessageId ?? 0;

            // Parse the callback data
            var callbackData = callbackQuery.Data.Split(':');
            Console.WriteLine(callbackData);
            var action = callbackData[0]; // "accept" or "decline"
            var bookingId = callbackData.Length > 1 ? callbackData[1] : "unknown";

            switch (callBack)
            {
                case CallBack.Accept:
                    {
                        acknowledgeBookingQueue.Enqueue(Convert.ToInt64(bookingId), chatId.Value);

                        await botClient.SendMessage(
                           chatId: chatId,
                           text: DriverMessages.BookingAcknowledgeAcceptMessage(bookingId),
                           cancellationToken: cancellationToken
                       );
                        await RemoveOptionButtons(botClient, chatId, messageId, cancellationToken);

                        logger.LogInformation("BookingAcknowledgeAcceptMessage has been sent to driver for bookingId {bookingId}, {driverTelId}", bookingId,chatId);
;                        break;
                    }
                case CallBack.Decline:
                    {
                        await botClient.SendMessage(
                            chatId: chatId,
                            text: DriverMessages.BookingAcknowledgeDeclinedMessage(bookingId),
                            cancellationToken: cancellationToken
                        );
                        await RemoveOptionButtons(botClient, chatId, messageId, cancellationToken);
                        logger.LogInformation("BookingAcknowledgeDeclinedMessage has been sent to driver for bookingId {bookingId}, {driverTelId}", bookingId, chatId);
                        break;
                    }
                case CallBack.Start:
                    {
                        using var scope = serviceProvider.CreateScope();
                        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                        await bookingService.Start(Convert.ToInt64(bookingId));
                        await UpdateButtonToCompleteOnly(botClient, chatId, messageId, bookingId, cancellationToken);
                        logger.LogInformation("Trip has been started for bookingId {bookingId}, {driverTelId}", bookingId, chatId);
                        break;
                    }
                case CallBack.Complete:
                    {
                        using var scope = serviceProvider.CreateScope();
                        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                        await bookingService.Complete(Convert.ToInt64(bookingId));
                        var booking = await bookingService.Get(Convert.ToInt64(bookingId));
                        var driver = await userService.GetUser(booking.AssignedToId.Value);
                        userStateService.SetCommandState(chatId.Value, "/complete_booking", UserState.WaitingForAttachment, new AdditionalInfo()
                        {
                            CurrentBookingId = booking.Id
                        });
                        await NotifyDriverForBookingAssignmentFailure(botClient, bookingId, driver);
                        await RemoveOptionButtons(botClient, chatId, messageId, cancellationToken);
                        logger.LogInformation("Trip has been completed for bookingId {bookingId}, {driverTelId} and prompted for attachment", bookingId, chatId);
                        break;
                    }
            }



        }

        private static async Task RemoveOptionButtons(TelegramBotClient botClient, long? chatId, int messageId, CancellationToken cancellationToken)
        {
            // Disable the buttons by editing the message and removing the keyboard
            await botClient.EditMessageReplyMarkup(
                chatId: chatId,
                messageId: messageId,
                replyMarkup: null, // Remove the inline keyboard
                cancellationToken: cancellationToken
            );
        }

        private static async Task UpdateButtonToCompleteOnly(TelegramBotClient botClient, long? chatId, int messageId, string bookingId, CancellationToken cancellationToken)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                 InlineKeyboardButton.WithCallbackData(DriverMessages.Complete, $"complete:{bookingId}")
            }
             });
            // Disable the buttons by editing the message and removing the keyboard
            await botClient.EditMessageReplyMarkup(
                chatId: chatId,
                messageId: messageId,
                replyMarkup: inlineKeyboard, // Remove the inline keyboard
                cancellationToken: cancellationToken
            );
        }

        private async Task NotifyDriverForBookingAssignmentFailure(TelegramBotClient botClient, string bookingId, Data.Entities.User user)
        {

            await botClient.SendMessage(
                        chatId: user.TelegramUserId,
                        text: DriverMessages.PromptForScreenShotMessage(bookingId),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                    );
            logger.LogInformation("Sent booking {bookingId} prompt for attachment to user: {userId}", bookingId, user.Id);
        }
    }

    public enum CallBack
    {
        Accept,
        Decline,
        Start,
        Complete
    }
}
