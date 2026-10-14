// Data/Configurations/PaymentConfiguration.cs
using eHoteloo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eHoteloo.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.Method)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(p => p.PaidAt)
                .IsRequired();

            // Relations
            builder.HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BookingId);

            builder.HasMany(p => p.Services)
                .WithOne(s => s.Payment)
                .HasForeignKey(s => s.PaymentId)
                .OnDelete(DeleteBehavior.SetNull); // لو اتمسح الدفع, الخدمة ترجع Pending
        }
    }
}