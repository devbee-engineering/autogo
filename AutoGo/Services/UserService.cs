using Agoda.IoC.Core;
using AutoGo.Data.DbContexts;
using AutoGo.Data.Entities;
using AutoGo.Services.Exceptions;
using AutoGo.Services.Model;
using System.Text.RegularExpressions;

namespace AutoGo.Services;

public interface IUserService
{
    Task<int> AddDriver(AddDriverModel addDriverModel);
    Task<List<User>> GetAllActiveDrivers();
    Task<User> GetUser(int userId);
    Task<User> GetUserByMobileNo(string mobileNo);
    Task<User?> GetUserByTelegramId(long telegramId);
    Task<bool> IsUserVerified(long telegramId);
    Task UpdateOnlineStatus(int id, bool isOnline = true);
    Task VerifyUser(int userId, string pin, long telegramId);
}

[RegisterPerRequest]
public class UserService(IUserRepository userRepository, ILogger<UserService> logger) : IUserService
{
    public async Task<int> AddDriver(AddDriverModel addDriverModel)
    {
        if (!IsValidMobileNumber(addDriverModel.MobileNumber))
        {
            logger.LogInformation("user creation tried with invalid mobile number, {invalidNo}", addDriverModel.MobileNumber);
            throw new BadDataException("Invalid Mobile Number");
        }

        var createdByUser = await GetUser(addDriverModel.CreatedByUser);

        var currentOrg = createdByUser.OrganizationIds.ToList().FirstOrDefault();


        User user = new()
        {
            Name = addDriverModel.Name,
            MobileNumber = addDriverModel.MobileNumber,
            Pin = GeneratePin(),
            OrganizationIds = [currentOrg],
            UserType = UserType.Driver,
        };

        await userRepository.Add(user);

        return user.Id;
    }

    public async Task<User> GetUser(int userId)
    {
        return await userRepository.Get(userId);
    }

    public async Task<User> GetUserByMobileNo(string mobileNo)
    {
        return await userRepository.GetUserByMobileNo(mobileNo);
    }

    public async Task<User?> GetUserByTelegramId(long telegramId)
    {
        return await userRepository.GetUserByTelegramId(telegramId);
    }

    public async Task<bool> IsUserVerified(long telegramId)
    {
        var user = await userRepository.GetUserByTelegramId(telegramId);

        return user != null && user.IsVerified;
    }

    public async Task<List<User>> GetAllActiveDrivers()
    {
        return await userRepository.GetAllActiveDrivers();
    }


    public async Task VerifyUser(int userId, string pin, long telegramId)
    {
        var user = await userRepository.Get(userId);

        if (user.Pin == pin)
        {
            await userRepository.Verify(userId, telegramId);
        }
        else
        {
            logger.LogInformation("user attempted verification with invalid pin, userId {userId}, pin {pin}", userId, pin);
            throw new BadDataException("Invalid Pin");
        }
    }

    public async Task UpdateOnlineStatus(int id,bool isOnline = true)
    {
        await userRepository.UpdateIsOnline(id, isOnline);
    }

    private static bool IsValidMobileNumber(string mobileNumber)
    {
        // Regex to match either:
        // - A 10-digit number (e.g., 91234567891)
        // - A 13-character number starting with +91 (e.g., +9191234567891)
        var regex = new Regex(@"^(\+91\d{10}|\d{10})$");

        return regex.IsMatch(mobileNumber);
    }

    private static string GeneratePin()
    {
        var random = new Random();
        return random.Next(1000, 10000).ToString(); // Generates a number between 1000 and 9999
    }
}
