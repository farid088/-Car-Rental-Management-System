using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);

        builder
            .Property(c => c.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(c => c.DrivingLicenseNumber)
            .IsRequired()
            .HasMaxLength(20);

        // DrivingLicenseNumber üçün unikal indeks
        builder
            .HasIndex(c => c.DrivingLicenseNumber)
            .IsUnique();

        builder
            .Property(c => c.LicenseExpiryDate)
            .IsRequired();
    }
}