using Agoda.IoC.Core;
using AutoGo.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoGo.Data.DbContexts;

public interface IBookingRepository
{
    Task<long> Add(Booking booking);
    Task Assign(long id, int assignToId);
    Task Attach(long id, string attachmentId);
    Task Cancel(long id);
    Task Complete(long id);
    Task<Booking?> Get(long id);
    Task Start(long id);
}
[RegisterPerRequest]
public class BookingRepository(AppDbContext appDbContext) : IBookingRepository
{
    public async Task<long> Add(Booking booking)
    {
        booking.Status = BookingStatus.Pending;
        booking.CreatedTime = DateTime.UtcNow;
        await appDbContext.Bookings.AddAsync(booking);
        await appDbContext.SaveChangesAsync();

        return booking.Id;
    }

    public async Task<Booking?> Get(long id)
    {
        return await appDbContext.Bookings.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task Assign(long id, int assignToId)
    {
        var booking = await appDbContext.Bookings.FirstAsync(x => x.Id == id);
        booking.AssignedToId = assignToId;
        booking.AssignedToTime = DateTime.UtcNow;
        booking.Status = BookingStatus.Assigned;
        await appDbContext.SaveChangesAsync();
    }

    public async Task Start(long id)
    {
        var booking = await appDbContext.Bookings.FirstAsync(x => x.Id == id);
        booking.StartTime = DateTime.UtcNow;
        booking.Status = BookingStatus.InProgress;
        await appDbContext.SaveChangesAsync();
    }

    public async Task Complete(long id)
    {
        var booking = await appDbContext.Bookings.FirstAsync(x => x.Id == id);
        booking.CompletedTime = DateTime.UtcNow;
        booking.Status = BookingStatus.Completed;
        await appDbContext.SaveChangesAsync();
    }

    public async Task Cancel(long id)
    {
        var booking = await appDbContext.Bookings.FirstAsync(x => x.Id == id);
        booking.Status = BookingStatus.Cancelled;
        await appDbContext.SaveChangesAsync();
    }

    public async Task Attach(long id, string attachmentId)
    {
        var booking = await appDbContext.Bookings.FirstAsync(x => x.Id == id);
        booking.AttachmentId = attachmentId;
        await appDbContext.SaveChangesAsync();
    }
}
