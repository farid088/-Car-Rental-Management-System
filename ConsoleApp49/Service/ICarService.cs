using Microsoft.EntityFrameworkCore;
public interface ICarService
{
    Task AddCarAsync(Car car);
    Task<IEnumerable<Car>> GetAllCarsAsync();
    Task<Car?> GetCarByIdAsync(int id);
    Task UpdateCarAsync(Car car);
    Task DeactivateCarAsync(int id);
}
public class CarService : ICarService
{
    private readonly AppDbContext _context;
    public CarService(AppDbContext context)
    {
        _context = context;
    }
    public async Task AddCarAsync(Car car)
    {
        _context.Cars.Add(car);
        await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<Car>> GetAllCarsAsync()
    {
        return await _context.Cars.ToListAsync();
    }
    public async Task<Car?> GetCarByIdAsync(int id)
    {
        return await _context.Cars.FindAsync(id);
    }
    public async Task UpdateCarAsync(Car car)
    {
        _context.Cars.Update(car);
        await _context.SaveChangesAsync();
    }
    public async Task DeactivateCarAsync(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car != null)
        {
            car.IsAvailable = false;
            await _context.SaveChangesAsync();
        }
    }
}