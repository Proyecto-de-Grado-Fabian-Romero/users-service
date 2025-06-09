using Microsoft.EntityFrameworkCore;
using UsersService.Src.Domain.Entities;

namespace UsersService.Src.Infraestructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    public DbSet<BankPaymentData> BankPaymentData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

        modelBuilder.Entity<User>()
            .HasOne(u => u.BankPaymentData)
            .WithOne(b => b.User)
            .HasForeignKey<BankPaymentData>(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
