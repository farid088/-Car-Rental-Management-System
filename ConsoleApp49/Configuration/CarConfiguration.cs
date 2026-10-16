using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Brand).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Model).IsRequired().HasMaxLength(50);

        builder.Property(c => c.PlateNumber).IsRequired().HasMaxLength(20);
        // PlateNumber üçün unikal indeks:
        builder.HasIndex(c => c.PlateNumber).IsUnique();

        builder.Property(c => c.DailyPrice).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(c => c.Year).IsRequired();
        builder.Property(c => c.Mileage).IsRequired();
        builder.Property(c => c.IsAvailable).IsRequired();

    }
}