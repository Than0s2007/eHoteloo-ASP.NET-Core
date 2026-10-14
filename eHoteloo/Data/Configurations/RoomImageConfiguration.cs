// Data/Configurations/RoomImageConfiguration.cs
using eHoteloo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eHoteloo.Data.Configurations
{
    public class RoomImageConfiguration : IEntityTypeConfiguration<RoomImage>
    {
        public void Configure(EntityTypeBuilder<RoomImage> builder)
        {
            builder.ToTable("RoomImages");

            builder.HasKey(ri => ri.Id);

            builder.Property(ri => ri.ImagePath)
                .IsRequired()
                .HasMaxLength(500);

            // Relations
            builder.HasOne(ri => ri.Room)
                .WithMany(r => r.Images)
                .HasForeignKey(ri => ri.RoomId);
        }
    }
}