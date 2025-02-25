using Agoda.IoC.Core;
using Telegram.Bot;

namespace AutoGo.BotHandlers
{
    public interface ITelegramBotClientProvider
    {
        TelegramBotClient Get();
    }
    [RegisterSingleton]
    public class TelegramBotClientProvider : ITelegramBotClientProvider
    {
        private readonly TelegramBotClient telegramBotClient;

        public TelegramBotClientProvider(IConfiguration configuration)
        {
            var botToken = configuration["TelegramBot:Token"];
            telegramBotClient = new TelegramBotClient(botToken);
        }

        public TelegramBotClient Get() { return telegramBotClient; }
    }
}
