using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public interface ICustomerService
{
    // Async CRUD metodları
    Task AddCustomerAsync(Customer customer);
    Task<IEnumerable<Customer>> GetAllCustomersAsync();
    Task<Customer?> GetCustomerByIdAsync(int id);
    Task UpdateCustomerAsync(Customer customer);
    Task DeleteCustomerAsync(int id);

    // Qayda: Vəsiqənin müddətini yoxlamaq üçün metod
    Task<bool> CanCustomerRentAsync(int customerId);

    // Qayda: Müştərinin bütün icarə tarixçəsini göstərən metod
    Task<IEnumerable<Rental>> GetCustomerRentalHistoryAsync(int customerId);

    // Qayda: Müştəriləri ümumi xərclədikləri məbləğə görə sıralayan metod
    Task<IEnumerable<CustomerSpendsDto>> GetCustomersOrderedByTotalSpendAsync();
}

public record CustomerSpendsDto(int CustomerId, string FullName, decimal TotalSpent);

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _context;

    public CustomerService(AppDbContext context)
    {
        _context = context;
    }

    // 1. Müştəri əlavə etmək (Email və Sürücülük vəsiqəsi unikal olmalıdır)
    public async Task AddCustomerAsync(Customer customer)
    {
        bool emailExists = await _context.Customers
            .AnyAsync(c => c.Email == customer.Email);
        if (emailExists)
            throw new InvalidOperationException("Bu Email ünvanı artıq istifadə olunur.");

        bool licenseExists = await _context.Customers
            .AnyAsync(c => c.DrivingLicenseNumber == customer.DrivingLicenseNumber);
        if (licenseExists)
            throw new InvalidOperationException("Bu sürücülük vəsiqəsi nömrəsi artıq sistemdə mövcuddur.");

        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    }

    // 2. Bütün müştəriləri göstərmək
    public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
    {
        return await _context.Customers
            .AsNoTracking()
            .ToListAsync();
    }

    // 3. ID-yə görə müştəri tapmaq
    public async Task<Customer?> GetCustomerByIdAsync(int id)
    {
        return await _context.Customers.FindAsync(id);
    }

    // 4. Müştəri məlumatlarını yeniləmək
    public async Task UpdateCustomerAsync(Customer customer)
    {
        bool emailExists = await _context.Customers
            .AnyAsync(c => c.Email == customer.Email && c.Id != customer.Id);
        if (emailExists)
            throw new InvalidOperationException("Bu Email başqa bir müştəri tərəfindən istifadə olunur.");

        bool licenseExists = await _context.Customers
            .AnyAsync(c => c.DrivingLicenseNumber == customer.DrivingLicenseNumber && c.Id != customer.Id);
        if (licenseExists)
            throw new InvalidOperationException("Bu sürücülük vəsiqəsi nömrəsi başqa bir müştəriyə aiddir.");

        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }

    // 5. Müştərini silmək
    public async Task DeleteCustomerAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }

    // Qayda: Vəsiqənin müddəti bitibsə, müştəri yeni icarə yarada bilməz
    public async Task<bool> CanCustomerRentAsync(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer == null)
            return false;

        return customer.LicenseExpiryDate >= DateTime.UtcNow;
    }

    // Qayda: Müştərinin bütün icarə tarixçəsini göstərən metod
    public async Task<IEnumerable<Rental>> GetCustomerRentalHistoryAsync(int customerId)
    {
        return await _context.Rentals
            .Include(r => r.Car)
            .Include(r => r.Payments)
            .Include(r => r.Penalties)
            .Where(r => r.CustomerId == customerId)
            .AsNoTracking()
            .ToListAsync();
    }

    // Qayda: Müştəriləri ümumi xərclədikləri məbləğə görə sıralayan metod
    public async Task<IEnumerable<CustomerSpendsDto>> GetCustomersOrderedByTotalSpendAsync()
    {
        return await _context.Customers
            .Select(c => new CustomerSpendsDto(
                c.Id,
                c.FullName,
                (c.Rentals.SelectMany(r => r.Payments).Sum(p => (decimal?)p.Amount) ?? 0) +
                (c.Rentals.SelectMany(r => r.Penalties).Sum(p => (decimal?)p.Amount) ?? 0)
            ))
            .OrderByDescending(c => c.TotalSpent)
            .AsNoTracking()
            .ToListAsync();
    }
}