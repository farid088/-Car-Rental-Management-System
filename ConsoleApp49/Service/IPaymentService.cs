using Microsoft.EntityFrameworkCore;
public interface IPaymentService
{
    Task AddPaymentAsync(Payment payment);
    Task ShowAllPaymentsAsync();
    Task<double> CalculatePaymentsForRentalAsync(int rentalId);
    Task ReturnRentalAsync(int rentalId);
}

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _context;
    public   PaymentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddPaymentAsync(Payment payment)
    {
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
    }
    public async Task ShowAllPaymentsAsync()
    {
        var payments = await _context.Payments
            .Include(p => p.Rental)
            .ThenInclude(r => r.Car)
            .Include(p => p.Rental)
            .ThenInclude(r => r.Customer)
            .ToListAsync();
        foreach (var payment in payments)
        {
            Console.WriteLine($"Payment Id: {payment.Id}, Rental Id: {payment.RentalId}, Amount: {payment.Amount}, Payment Date: {payment.PaymentDate.ToShortDateString()}, Payment Type: {payment.PaymentType}");
            Console.WriteLine($"Rental Details - Car: {payment.Rental.Car.Model}, Customer: {payment.Rental.Customer.FullName}");
        }
    }
    
    public async Task<double> CalculatePaymentsForRentalAsync(int rentalId)
    {
        var payments = await _context.Payments
            .Where(p => p.RentalId == rentalId)
            .ToListAsync();
        return payments.Sum(p => p.Amount);
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