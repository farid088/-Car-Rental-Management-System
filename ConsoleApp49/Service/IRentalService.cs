public interface IRentalService
{
    Task<Rental > CreateRentalAsync(int carId,int customerId,DateTime ExpectedReturnDate,DateTime ActuallyReturnDate);
    Task ReturnRentalAsync(int rentalId);
    Task CancelRentalAsync(int rentalId);
}

public class RentalService : IRentalService
{
    private readonly AppDbContext _context;
    public RentalService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Rental> CreateRentalAsync(int carId, int customerId, DateTime expectedReturnDate, DateTime actualReturnDate)
    {
        var rental = new Rental
        {
            CarId = carId,
            CustomerId = customerId,
            StartDate = DateTime.Now,
            ExpectedReturnDate = expectedReturnDate,
            ActualReturnDate = actualReturnDate,
            Status = "Active"
        };
        _context.Rentals.Add(rental);
        await _context.SaveChangesAsync();
        return rental;
    }
    public async Task ReturnRentalAsync(int rentalId)
    {
        var rental = await _context.Rentals.FindAsync(rentalId);
        if (rental != null)
        {
            rental.ActualReturnDate = DateTime.Now;
            rental.Status = "Returned";
            await _context.SaveChangesAsync();
        }
    }
    public async Task CancelRentalAsync(int rentalId)
    {
        var rental = await _context.Rentals.FindAsync(rentalId);
        if (rental != null)
        {
            rental.Status = "Cancelled";
            await _context.SaveChangesAsync();
        }
    }
}