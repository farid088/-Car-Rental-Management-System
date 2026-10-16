using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
       builder.HasKey(p => p.Id);
        builder.Property(p => p.RentalId)
            .IsRequired();
        builder.Property(p => p.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        builder.Property(p => p.PaymentDate)
            .IsRequired();
        builder.Property(p => p.PaymentType)
            .IsRequired()
            .HasMaxLength(50);
    }
}