using AutoGo.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoGo.Data.DbContexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Booking> Bookings { get; set; }
}
