using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RentalConfiguration : IEntityTypeConfiguration<Rental>
{
    

    public void Configure(EntityTypeBuilder<Rental> builder)
    { 
        builder.HasKey(r => r.Id);
        builder.Property(r => r.StartDate).IsRequired();
        builder.Property(r => r.ExpectedReturnDate).IsRequired();
        builder.Property(r => r.ActualReturnDate).IsRequired(false);
        builder.Property(r => r.TotalPrice).IsRequired();
        builder.Property(r => r.DailyPrice).IsRequired();
        builder.Property(r => r.Status).IsRequired().HasMaxLength(50);

    }
}