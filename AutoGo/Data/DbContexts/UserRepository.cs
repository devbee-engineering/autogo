using Agoda.IoC.Core;
using AutoGo.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoGo.Data.DbContexts;

public interface IUserRepository
{
    Task Add(User user);
    Task<User> Get(int id);
    Task<List<User>> GetAllActiveDrivers();
    Task<User> GetUserByMobileNo(string mobileNo);
    Task<User?> GetUserByTelegramId(long telegramId);
    Task UpdateIsOnline(int id, bool isOnline);
    Task Verify(int id, long telegramId);
}

[RegisterPerRequest]
public class UserRepository(AppDbContext appDbContext) : IUserRepository
{

    public async Task Add(User user)
    {
        await appDbContext.Users.AddAsync(user);
        await appDbContext.SaveChangesAsync();
    }

    public async Task<User> Get(int id)
    {
        return await appDbContext.Users.Where(x => x.Id == id).FirstAsync();
    }

    public async Task<User> GetUserByMobileNo(string mobileNo)
    {
        return await appDbContext.Users.Where(x=>x.MobileNumber.Contains(mobileNo)).FirstAsync();
    }

    public async Task<User?> GetUserByTelegramId(long telegramId)
    {
        return await appDbContext.Users.Where(x => x.TelegramUserId == telegramId).FirstOrDefaultAsync();
    }

    public async Task<List<User>> GetAllActiveDrivers()
    {
        return await appDbContext.Users.Where(x => (x.UserType == UserType.Driver || x.UserType == UserType.SuperDriver) && x.IsOnline ).ToListAsync();
    }

    public async Task UpdateIsOnline(int id, bool isOnline)
    {
        var user = await appDbContext.Users.Where(x => x.Id == id).FirstAsync();

        user.IsOnline = isOnline;
        await appDbContext.SaveChangesAsync();
    }

    public async Task Verify(int id, long telegramId)
    {
        var user = await appDbContext.Users.Where(x => x.Id == id).FirstAsync();

        user.IsVerified = true;
        user.TelegramUserId = telegramId;

        await appDbContext.SaveChangesAsync();
    }
}
