// Data/ApplicationDbContext.cs
using eHoteloo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eHoteloo.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomImage> RoomImages { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Service> Services { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // لازم تيجي الأول عشان Identity يعمل setup بتاعه
            base.OnModelCreating(modelBuilder);

            // عدّل أسماء جداول Identity
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");

            // Apply your custom configurations
            modelBuilder.ApplyConfiguration(new Configurations.RoomConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.RoomImageConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.BookingConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.ServiceConfiguration());

            // User configuration (بدون Role وبدون PasswordHash وبدون Email index)
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.IdentityPhotoName)
                    .HasMaxLength(500)
                    .IsRequired(false);

                entity.Property(u => u.Balance)
                    .HasPrecision(18, 2)
                    .HasDefaultValue(0);

                entity.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relations
                entity.HasMany(u => u.Bookings)
                    .WithOne(b => b.User)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}