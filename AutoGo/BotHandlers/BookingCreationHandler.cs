using Agoda.IoC.Core;
using AutoGo.Constants;
using AutoGo.Services;
using AutoGo.Services.Model;
using AutoGo.Services.Workers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AutoGo.BotHandlers
{
    public interface IBookingCreationHandler
    {
        Task Handle(long userId, TelegramBotClient telegramBot, string? text, CancellationToken cancellationToken);
        Task HandleVoiceAttachment(long userId, TelegramBotClient telegramBot, Voice voice, CancellationToken cancellationToken);
    }
    [RegisterPerRequest]
    public class BookingCreationHandler(IBookingService bookingService, 
        IUserStateService userStateService, 
        IUserService userService, 
        IUnprocessedBookingQueue unprocessedBooking) : IBookingCreationHandler
    {
        private const string createBookingCommand = "/create_booking";
        public async Task Handle(long userId, TelegramBotClient telegramBot, string? text, CancellationToken cancellationToken)
        {
            var state = userStateService.GetCommandState(userId);

            if (string.IsNullOrEmpty(state.Command))
            {
                await telegramBot.SendMessage(
                    chatId: userId,
                    text: AdminMessages.BookingCreateMobileNoPromptMessage,
                    cancellationToken: cancellationToken
                );
                userStateService.SetCommandState(userId, createBookingCommand, UserState.WaitingForMobileNumber);
            }

            if (text != null && state.State == UserState.WaitingForMobileNumber)
            {


                await telegramBot.SendMessage(
                    chatId: userId,
                    text: AdminMessages.BookingCreateVoicePromptMessage,
                    cancellationToken: cancellationToken
                );
                userStateService.SetCommandState(userId, createBookingCommand, UserState.WaitingForVoiceNote, new AdditionalInfo()
                {
                    MobileNumber = text
                });
            }
        }

        public async Task HandleVoiceAttachment(long userId, TelegramBotClient telegramBot, Telegram.Bot.Types.Voice voice, CancellationToken cancellationToken)
        {

            if (voice != null)
            {
                var fileId = voice.FileId;

                var state = userStateService.GetCommandState(userId);

                var user = await userService.GetUserByTelegramId(userId);
                BookingCreationModel booking = new() { createdById = user.Id, MobileNumber = state.AddtionalInfo.MobileNumber, VoiceFileId = fileId };
                var bookingId = await bookingService.Create(booking);


                unprocessedBooking.Enqueue(bookingId);

                // Send the voice message back to the user
                await telegramBot.SendMessage(
                    chatId: userId,
                    text: AdminMessages.BookingCreatedMessage(bookingId),
                    cancellationToken: cancellationToken
                );

                userStateService.ClearCommandState(userId);

            }
        }


    }
}
