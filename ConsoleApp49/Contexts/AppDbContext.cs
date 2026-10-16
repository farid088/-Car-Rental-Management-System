using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Car> Cars { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Penalty> Penalties { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=TITAN08\\SQLEXPRESS;Database=Fe1;Trusted_Connection=True;TrustServerCertificate=True;");

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Car>().HasKey(c => c.Id);
        modelBuilder.Entity<Customer>().HasKey(c => c.Id);
        modelBuilder.Entity<Rental>().HasKey(r => r.Id);
        modelBuilder.Entity<MaintenanceRecord>().HasKey(m => m.Id);
        modelBuilder.Entity<Payment>().HasKey(p => p.Id);
        modelBuilder.Entity<Penalty>().HasKey(p => p.Id);
        
        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Car)
            .WithMany(c => c.Rentals)
            .HasForeignKey(r => r.CarId);
        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Customer)
            .WithMany(c => c.Rentals)
            .HasForeignKey(r => r.CustomerId);
        modelBuilder.Entity<MaintenanceRecord>()
            .HasOne(m => m.Car)
            .WithMany(c => c.MaintenanceRecords)
            .HasForeignKey(m => m.CarId);
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Rental)
            .WithMany(r => r.Payments)
            .HasForeignKey(p => p.RentalId);
        modelBuilder.Entity<Penalty>()
            .HasOne(p => p.Rental)
            .WithMany(r => r.Penalties)
            .HasForeignKey(p => p.RentalId);
    }
}