using Agoda.IoC.Core;
using AutoGo.Constants;
using AutoGo.Services;
using Telegram.Bot;

namespace AutoGo.BotHandlers
{
    public interface IDriverStatusHandler
    {
        Task GoOffline(long userId, TelegramBotClient telegramBot, CancellationToken cancellationToken);
        Task GoOnline(long userId, TelegramBotClient telegramBot, CancellationToken cancellationToken);
    }
    [RegisterPerRequest]
    public class DriverStatusHandler(IUserService userService) : IDriverStatusHandler
    {
        public async Task GoOnline(long userId, TelegramBotClient telegramBot, CancellationToken cancellationToken)
        {
            var user = await userService.GetUserByTelegramId(userId);

            if (user != null)
            {
                await userService.UpdateOnlineStatus(user.Id);
                await telegramBot.SendMessage(
                        chatId: userId,
                        text: DriverMessages.GoOnlineMessage,
                        cancellationToken: cancellationToken
                    );
            }
        }

        public async Task GoOffline(long userId, TelegramBotClient telegramBot, CancellationToken cancellationToken)
        {
            var user = await userService.GetUserByTelegramId(userId);

            if (user != null)
            {
                await userService.UpdateOnlineStatus(user.Id, false);
                await telegramBot.SendMessage(
                        chatId: userId,
                        text: DriverMessages.GoOfflineMessage,
                        cancellationToken: cancellationToken
                    );
            }
        }
    }
}
