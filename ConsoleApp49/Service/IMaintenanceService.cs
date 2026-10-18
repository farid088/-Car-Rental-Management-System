using Microsoft.EntityFrameworkCore;
public interface IMaintenanceService
{
    // 1. Texniki baxış qeydi əlavə etmək
    Task AddMaintenanceAsync(MaintenanceRecord maintenance);

    // 2. Avtomobilin texniki baxış tarixçəsini göstərmək
    Task ShowMaintenanceHistoryAsync(int carId);

    // 3. Növbəti texniki baxış tarixi yaxınlaşan avtomobilləri göstərmək
    Task ShowUpcomingMaintenanceAsync();

    // 4. Avtomobil üzrə təmir xərclərini hesablamaq
    Task<double> CalculateMaintenanceCostsAsync(int carId);
}


public class MaintenanceService : IMaintenanceService
{
    private readonly AppDbContext _context;

    public MaintenanceService(AppDbContext context)
    {
        _context = context;
    }

    // 1. Texniki baxış qeydi əlavə etmək
    public async Task AddMaintenanceAsync(MaintenanceRecord maintenance)
    {
        var car = await _context.Cars
            .FirstOrDefaultAsync(c => c.Id == maintenance.CarId);

        if (car == null)
            throw new CarNotFoundException("Avtomobil tapılmadı.");

        _context.MaintenanceRecords.Add(maintenance);

        await _context.SaveChangesAsync();
    }

    // 2. Avtomobilin texniki baxış tarixçəsini göstərmək
    public async Task ShowMaintenanceHistoryAsync(int carId)
    {
        var car = await _context.Cars
            .FirstOrDefaultAsync(c => c.Id == carId);

        if (car == null)
            throw new CarNotFoundException("Avtomobil tapılmadı.");

        var records = await _context.MaintenanceRecords
            .Where(m => m.CarId == carId)
            .ToListAsync();

        foreach (var record in records)
        {
            Console.WriteLine(
                $"ID: {record.Id}, " +
                $"Tarix: {record.MaintenanceDate}, " +
                $"Xərc: {record.Cost}");
        }
    }

    // 3. Növbəti texniki baxış tarixi yaxınlaşan avtomobilləri göstərmək
    public async Task ShowUpcomingMaintenanceAsync()
    {
        var today = DateTime.Today;
        var limitDate = today.AddDays(7);

        var records = await _context.MaintenanceRecords
            .Include(m => m.Car)
            .Where(m => m.NextMaintenanceDate >= today &&
                        m.NextMaintenanceDate <= limitDate)
            .ToListAsync();

        foreach (var record in records)
        {
            Console.WriteLine(
                $"Avtomobil: {record.Car.Model}, " +
                $"Növbəti texniki baxış: {record.NextMaintenanceDate}");
        }
    }

    // 4. Avtomobil üzrə təmir xərclərini hesablamaq
    public async Task<double> CalculateMaintenanceCostsAsync(int carId)
    {
        bool carExists = await _context.Cars
            .AnyAsync(c => c.Id == carId);

        if (!carExists)
            throw new CarNotFoundException("Avtomobil tapılmadı.");

        return await _context.MaintenanceRecords
            .Where(m => m.CarId == carId)
            .SumAsync(m => m.Cost);
    }
}