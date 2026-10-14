using eHoteloo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eHoteloo.Data.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.ToTable("Services");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Description)
                .HasMaxLength(1000);

            builder.Property(s => s.Price)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(s => s.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relation: Service → Booking (CASCADE OK)
            builder.HasOne(s => s.Booking)
                .WithMany(b => b.Services)
                .HasForeignKey(s => s.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Service → Payment (NO ACTION ← هنا التعديل المهم!)
            builder.HasOne(s => s.Payment)
                .WithMany(p => p.Services)
                .HasForeignKey(s => s.PaymentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);  // ← لازم يكون NoAction مش SetNull
        }
    }
}