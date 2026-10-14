// Data/Configurations/BookingConfiguration.cs
using eHoteloo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eHoteloo.Data.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("Bookings");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.BookingNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(b => b.BookingNumber)
                .IsUnique();

            builder.Property(b => b.CheckIn)
                .IsRequired();

            builder.Property(b => b.CheckOut)
                .IsRequired();

            builder.Property(b => b.BookingPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(b => b.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Check constraint: CheckOut لازم يكون بعد CheckIn
            builder.HasCheckConstraint("CK_Booking_Dates", "[CheckOut] > [CheckIn]");

            // Relations
            builder.HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId);

            builder.HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId);

            builder.HasMany(b => b.Payments)
                .WithOne(p => p.Booking)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(b => b.Services)
                .WithOne(s => s.Booking)
                .HasForeignKey(s => s.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}