using Agoda.IoC.Core;
using AutoGo.Constants;
using AutoGo.Data.Entities;
using AutoGo.Services;
using AutoGo.Services.Exceptions;
using Telegram.Bot;

namespace AutoGo.BotHandlers;

public interface IStartHandler
{
    Task Handle(long userId, TelegramBotClient telegramBot, CancellationToken cancellationToken);
    Task HandleVerify(long userId, TelegramBotClient telegramBot, string text, CancellationToken cancellationToken);
}

[RegisterPerRequest]
public class StartHandler(IUserService userService, IUserStateService userStateService) : IStartHandler
{
    private const string StartCommand = "/start";

    public async Task Handle(long userId, TelegramBotClient telegramBot, CancellationToken cancellationToken)
    {
        var user = await userService.GetUserByTelegramId(userId);
        var state = userStateService.GetCommandState(userId);

        if (user == null && string.IsNullOrEmpty(state.Command))
        {
            await PromptForMobileNumber(userId, telegramBot, cancellationToken);
            return;
        }

        if (user != null)
        {
            await SendWelcomeMessage(user, telegramBot, cancellationToken);
        }
    }

    public async Task HandleVerify(long userId, TelegramBotClient telegramBot, string text, CancellationToken cancellationToken)
    {
        var state = userStateService.GetCommandState(userId);

        if (state == null)
        {
            await PromptForMobileNumber(userId, telegramBot, cancellationToken);
            return;
        }

        switch (state.State)
        {
            case UserState.WaitingForMobileNumber:
                await HandleMobileNumberVerification(userId, telegramBot, text, cancellationToken);
                break;

            case UserState.WaitingForPin:
                await HandlePinVerification(userId, telegramBot, text, cancellationToken, state.AddtionalInfo?.MobileNumber);
                break;

            default:
                await PromptForMobileNumber(userId, telegramBot, cancellationToken);
                break;
        }
    }

    private async Task PromptForMobileNumber(long userId, TelegramBotClient telegramBot, CancellationToken cancellationToken)
    {
        await telegramBot.SendMessage(
            chatId: userId,
            text: UserMessages.MobileNumberPromptMessage,
            cancellationToken: cancellationToken
        );

        userStateService.SetCommandState(userId, StartCommand, UserState.WaitingForMobileNumber);
    }

    private async Task HandleMobileNumberVerification(long userId, TelegramBotClient telegramBot, string mobileNumber, CancellationToken cancellationToken)
    {
        var user = await userService.GetUserByMobileNo(mobileNumber);

        if (user != null)
        {
            await telegramBot.SendMessage(
                chatId: userId,
                text: UserMessages.EnterPinPromptMessage,
                cancellationToken: cancellationToken
            );

            userStateService.SetCommandState(userId, StartCommand, UserState.WaitingForPin, new AdditionalInfo { MobileNumber = mobileNumber});
        }
        else
        {
            await telegramBot.SendMessage(
                chatId: userId,
                text: UserMessages.MobileNoNotFoundMessage,
                cancellationToken: cancellationToken
            );
        }
    }

    private async Task HandlePinVerification(long userId, TelegramBotClient telegramBot, string pin, CancellationToken cancellationToken, string mobileNumber)
    {
        if (string.IsNullOrEmpty(mobileNumber))
        {
            await PromptForMobileNumber(userId, telegramBot, cancellationToken);
            return;
        }

        var user = await userService.GetUserByMobileNo(mobileNumber);

        if (user == null)
        {
            await PromptForMobileNumber(userId, telegramBot, cancellationToken);
            return;
        }

        try
        {
            await userService.VerifyUser(user.Id, pin, userId);
            userStateService.ClearCommandState(userId);

            await telegramBot.SendMessage(
                chatId: userId,
                text: UserMessages.AccountVerificationSuccessMessage(user.Name),
                cancellationToken: cancellationToken
            );
        }
        catch (BadDataException)
        {
            await telegramBot.SendMessage(
                chatId: userId,
                text: "Wrong PIN. Please try again.",
                cancellationToken: cancellationToken
            );
        }
    }

    private async Task SendWelcomeMessage(User user, TelegramBotClient telegramBot, CancellationToken cancellationToken)
    {
        await telegramBot.SendMessage(
            chatId: user.TelegramUserId,
            text: UserMessages.WelcomeMessage(user.Name),
            cancellationToken: cancellationToken
        );
    }
}