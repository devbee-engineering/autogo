using AutoGo.Constants;
using Telegram.Bot.Types;

namespace AutoGo.BotHandlers;

public class MenuHandler
{
    public static BotCommand[] GetMenu()
    {
        var commands = new[]
        {
            new BotCommand { Command = "start", Description = MenuMessages.Start },
            new BotCommand { Command = "online", Description = MenuMessages.Online },
            new BotCommand { Command = "offline", Description = MenuMessages.Offline },
            new BotCommand { Command = "create_booking", Description = MenuMessages.CreateBooking }
         };

        return commands;
    }
}
