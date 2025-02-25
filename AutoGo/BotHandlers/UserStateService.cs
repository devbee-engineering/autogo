using Agoda.IoC.Core;
using System.Collections.Concurrent;

namespace AutoGo.BotHandlers;

public interface IUserStateService
{
    void ClearCommandState(long userId);
    UserCommandState GetCommandState(long userId);
    void SetCommandState(long userId, string command, UserState state);
    void SetCommandState(long userId, string command, UserState state, AdditionalInfo additionalInfo);
}
[RegisterSingleton]
public class UserStateService : IUserStateService
{
    // A thread-safe dictionary to store user states and commands
    private readonly ConcurrentDictionary<long, UserCommandState> _userStates = new();

    // Get the current command and state of a user
    public UserCommandState GetCommandState(long userId)
    {
        return _userStates.TryGetValue(userId, out var commandState) ? commandState : new UserCommandState();
    }

    // Set the current command and state for a user
    public void SetCommandState(long userId, string command, UserState state)
    {
        _userStates[userId] = new UserCommandState
        {
            Command = command,
            State = state
        };
    }

    public void SetCommandState(long userId, string command, UserState state, AdditionalInfo additionalInfo)
    {
        _userStates[userId] = new UserCommandState
        {
            Command = command,
            State = state,
            AddtionalInfo = additionalInfo
        };
    }

    // Clear the command and state for a user
    public void ClearCommandState(long userId)
    {
        _userStates.TryRemove(userId, out _);
    }
}

// Class to represent the user's current command and state
public class UserCommandState
{
    public string Command { get; set; } = string.Empty; // Current command (e.g., "start", "register")
    public UserState State { get; set; } = UserState.None; // Current state within the command

    public AdditionalInfo? AddtionalInfo { get; set; }
}

public class AdditionalInfo
{
    public string? MobileNumber { get; set; }
    public long? CurrentBookingId { get; set; }
}

public enum UserState
{
    None, // Default state
    WaitingForMobileNumber,
    WaitingForPin,
    WaitingForCommandSpecificInput, // Example: State for a specific command
    WaitingForVoiceNote,
    WaitingForAttachment,
}
