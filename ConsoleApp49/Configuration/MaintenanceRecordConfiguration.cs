using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class MaintenanceRecordConfiguration : IEntityTypeConfiguration<MaintenanceRecord>
{
    public void Configure(EntityTypeBuilder<MaintenanceRecord> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.CarId)
            .IsRequired();

        builder.Property(m => m.MaintenanceDate)
            .IsRequired();

        builder.Property(m => m.Description)
            .HasMaxLength(500);

        builder.Property(m => m.Cost)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(m => m.NextMaintenanceDate)
            .IsRequired();
    }
}