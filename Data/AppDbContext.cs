using Microsoft.EntityFrameworkCore;
using Pozdravlyator.Models;

namespace Pozdravlyator.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<BirthdayPerson> Birthdays { get; set; }
}