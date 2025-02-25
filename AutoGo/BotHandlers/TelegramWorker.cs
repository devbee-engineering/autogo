using AutoGo.Constants;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AutoGo.BotHandlers
{
    public class TelegramWorker(ILogger<TelegramWorker> logger,
        IUserStateService userStateService,
        IServiceScopeFactory serviceScopeFactory,
        IBookingCallBackHandler bookingAcknowledgeHandler,
        IBookingCompletionHandler bookingCompletionHandler,
        ITelegramBotClientProvider telegramBotClientProvider) : IHostedService
    {
        private readonly TelegramBotClient botClient = telegramBotClientProvider.Get();

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting the Telegram Polling Service");

            await botClient.SetMyCommands(MenuHandler.GetMenu());

            botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandleErrorAsync,
            receiverOptions: new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() 
            },
            cancellationToken: cancellationToken
        );

        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    if (update.Message != null)
                    {
                        await HandleMessageAsync(update.Message, cancellationToken);
                    }
                    break;

                case UpdateType.CallbackQuery:
                    if (update.CallbackQuery != null)
                    {
                        await HandleCallbackQueryAsync(update.CallbackQuery, cancellationToken);
                    }
                    break;

                default:
                    Console.WriteLine($"Unhandled update type: {update.Type}");
                    break;
            }
        }

        private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiEx => $"Telegram API Error:\n[{apiEx.ErrorCode}]\n{apiEx.Message}",
                _ => exception.ToString()
            };

            logger.LogError(errorMessage);
            await Task.CompletedTask;
        }

        private async Task HandleMessageAsync(Message message, CancellationToken cancellationToken)
        {
            var user = message.From;
            var text = message.Text ?? string.Empty;
            var voice = message.Voice;
            var photo = message.Photo;

            var state = userStateService.GetCommandState(user.Id);

            if (user == null)
                return;

            if (voice != null)
            {
                await HandleVoiceMessageAsync(user.Id, voice, state, cancellationToken);
                return;
            }

            if (photo != null)
            {
                await HandlePhotoAsync(user.Id, photo.Last(), state, cancellationToken);
                return;
            }

            Console.WriteLine($"{user.FirstName} | {user.Id} wrote: {text}");

            // Handle commands
            if (text.StartsWith("/"))
            {
                await HandleCommandAsync(user.Id, text, cancellationToken);
            }
            else
            {

                if (state != null)
                {
                    if (state.Command == "/start")
                    {
                        using var scope = serviceScopeFactory.CreateScope();
                        var startHandler = scope.ServiceProvider.GetRequiredService<IStartHandler>();
                        await startHandler.HandleVerify(user.Id, botClient, text, cancellationToken);


                        return;
                    }

                    if (state.Command == "/create_booking")
                    {
                        using var scope = serviceScopeFactory.CreateScope();
                        var bookingCreationHandler = scope.ServiceProvider.GetRequiredService<IBookingCreationHandler>();
                        await bookingCreationHandler.Handle(user.Id, botClient, text, cancellationToken);


                        return;
                    }
                }

            }
        }

        private async Task HandlePhotoAsync(long userId, PhotoSize photoSize, UserCommandState state, CancellationToken cancellationToken)
        {
            if (state.Command == "/complete_booking")
            {
                await bookingCompletionHandler.Handle(botClient, state, photoSize, cancellationToken);
                return;
            }
        }

        private async Task HandleVoiceMessageAsync(long userId, Telegram.Bot.Types.Voice voice, UserCommandState? state, CancellationToken cancellationToken)
        {
            if (state != null && state.Command == "/create_booking")
            {
                using var scope = serviceScopeFactory.CreateScope();
                var bookingCreationHandler = scope.ServiceProvider.GetRequiredService<IBookingCreationHandler>();
                await bookingCreationHandler.HandleVoiceAttachment(userId, botClient, voice, cancellationToken);


                return;
            }
        }


        private async Task HandleCommandAsync(long userId, string command, CancellationToken cancellationToken)
        {
            using var scope = serviceScopeFactory.CreateScope();
            switch (command)
            {
                case "/start":
                    {

                        var startHandler = scope.ServiceProvider.GetRequiredService<IStartHandler>();
                        await startHandler.Handle(userId, botClient, cancellationToken);
                        break;
                    }



                case "/create_booking":
                    {
                        var bookingCreationHandler = scope.ServiceProvider.GetRequiredService<IBookingCreationHandler>();
                        await bookingCreationHandler.Handle(userId, botClient, null, cancellationToken);
                        break;
                    }

                case "/online":
                    {

                        var driverStatusHandler = scope.ServiceProvider.GetRequiredService<IDriverStatusHandler>();
                        await driverStatusHandler.GoOnline(userId, botClient, cancellationToken);
                        break;
                    }

                case "/offline":
                    {

                        var driverStatusHandler = scope.ServiceProvider.GetRequiredService<IDriverStatusHandler>();
                        await driverStatusHandler.GoOffline(userId, botClient, cancellationToken);
                        break;
                    }

                default:
                    await botClient.SendMessage(
                        chatId: userId,
                        text: "Unknown command. Type /help to see available commands.",
                        cancellationToken: cancellationToken
                    );
                    break;
            }
        }

        private async Task HandleCallbackQueryAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            if (callbackQuery.Data == null)
                return;

            var callbackData = callbackQuery.Data.Split(':');
            var action = callbackData[0]; // "accept" or "decline"


            switch (action)
            {
                case "accept":
                    await bookingAcknowledgeHandler.Handle(botClient, CallBack.Accept, callbackQuery, cancellationToken);
                    break;
                case "decline":
                    await bookingAcknowledgeHandler.Handle(botClient, CallBack.Decline, callbackQuery, cancellationToken);
                    break;
                case "start":
                    await bookingAcknowledgeHandler.Handle(botClient, CallBack.Start, callbackQuery, cancellationToken);
                    break;
                case "complete":
                    await bookingAcknowledgeHandler.Handle(botClient, CallBack.Complete, callbackQuery, cancellationToken);
                    break;
            }



            // Acknowledge the callback query to stop the loading animation in Telegram
            await botClient.AnswerCallbackQuery(
                callbackQueryId: callbackQuery.Id,
                cancellationToken: cancellationToken
            );
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
