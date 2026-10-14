// Data/Configurations/RoomConfiguration.cs
using eHoteloo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eHoteloo.Data.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("Rooms");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.RoomNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(r => r.RoomNumber)
                .IsUnique();

            builder.Property(r => r.RoomType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(r => r.Description)
                .HasMaxLength(1000);

            builder.Property(r => r.PricePerDay)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(r => r.IsActive)
                .HasDefaultValue(true);

            builder.Property(r => r.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relations
            builder.HasMany(r => r.Bookings)
                .WithOne(b => b.Room)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict); // متمسحش الغرفة لو فيها حجوزات

            builder.HasMany(r => r.Images)
                .WithOne(ri => ri.Room)
                .HasForeignKey(ri => ri.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}