using Microsoft.EntityFrameworkCore;
using System.Globalization;

public class Program
{
    private static readonly string[] DateFormats =
    {
        "dd.MM.yyyy",
        "yyyy-MM-dd"
    };

    public static async Task Main()
    {
        using var context = new AppDbContext();

        ICarService carService = new CarService(context);
        ICustomerService customerService = new CustomerService(context);
        IRentalService rentalService = new RentalService(context);
        IPaymentService paymentService = new PaymentService(context);
        IMaintenanceService maintenanceService = new MaintenanceService(context);

        while (true)
        {
            Console.WriteLine("\n===== AVTOMOBİL İCARƏ SİSTEMİ =====");
            Console.WriteLine("1. Avtomobil əməliyyatları");
            Console.WriteLine("2. Müştəri əməliyyatları");
            Console.WriteLine("3. Rezervasiya yaratmaq");
            Console.WriteLine("4. Avtomobili qaytarmaq");
            Console.WriteLine("5. Rezervasiyanı ləğv etmək");
            Console.WriteLine("6. Ödəniş etmək");
            Console.WriteLine("7. Texniki baxış qeydi əlavə etmək");
            Console.WriteLine("8. Hesabatlar");
            Console.WriteLine("0. Çıxış");
            Console.Write("Seçiminiz: ");

            string? choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        await CarMenu(carService);
                        break;

                    case "2":
                        await CustomerMenu(customerService);
                        break;

                    case "3":
                        await CreateReservation(
                            context,
                            customerService,
                            rentalService);
                        break;

                    case "4":
                        await ReturnRental(rentalService);
                        break;

                    case "5":
                        await CancelRental(rentalService);
                        break;

                    case "6":
                        await MakePayment(
                            context,
                            paymentService);
                        break;

                    case "7":
                        await AddMaintenance(
                            maintenanceService);
                        break;

                    case "8":
                        await ReportMenu(context);
                        break;

                    case "0":
                        Console.WriteLine("Proqram bağlandı.");
                        return;

                    default:
                        Console.WriteLine("Yanlış seçim!");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Xəta: {ex.Message}");
            }
        }
    }

    // ================= AVTOMOBİLLƏR =================

    private static async Task CarMenu(ICarService service)
    {
        Console.WriteLine("\n1. Avtomobil əlavə et");
        Console.WriteLine("2. Bütün avtomobilləri göstər");
        Console.WriteLine("3. ID ilə avtomobil tap");
        Console.WriteLine("4. Avtomobili deaktiv et");

        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                var car = new Car
                {
                    Brand = ReadText("Marka: "),
                    Model = ReadText("Model: "),
                    PlateNumber = ReadInt("Nömrə (rəqəm): "),
                    DailyPrice = ReadDouble("Gündəlik qiymət: "),
                    Year = ReadInt("İstehsal ili: "),
                    Mileage = ReadInt("Yürüş: "),
                    IsAvailable = true
                };

                await service.AddCarAsync(car);
                Console.WriteLine("Avtomobil əlavə edildi.");
                break;

            case "2":
                var cars = await service.GetAllCarsAsync();

                foreach (var item in cars)
                    Console.WriteLine(item);

                break;

            case "3":
                int id = ReadInt("Avtomobil ID-si: ");
                var found = await service.GetCarByIdAsync(id);

                Console.WriteLine(found is null
                    ? "Avtomobil tapılmadı."
                    : found.ToString());
                break;

            case "4":
                await service.DeactivateCarAsync(
                    ReadInt("Avtomobil ID-si: "));

                Console.WriteLine("Əməliyyat tamamlandı.");
                break;

            default:
                Console.WriteLine("Yanlış seçim.");
                break;
        }
    }

    // ================= MÜŞTƏRİLƏR =================

    private static async Task CustomerMenu(ICustomerService service)
    {
        Console.WriteLine("\n1. Müştəri əlavə et");
        Console.WriteLine("2. Bütün müştəriləri göstər");
        Console.WriteLine("3. ID ilə müştəri tap");
        Console.WriteLine("4. Müştərini sil");
        Console.WriteLine("5. İcarə tarixçəsi");

        switch (Console.ReadLine())
        {
            case "1":
                var customer = new Customer
                {
                    FullName = ReadText("Ad və soyad: "),
                    Email = ReadText("Email: "),
                    DrivingLicenseNumber =
                        ReadText("Sürücülük vəsiqəsi: "),
                    LicenseExpiryDate =
                        ReadDate("Vəsiqənin bitmə tarixi: ")
                };

                await service.AddCustomerAsync(customer);
                Console.WriteLine("Müştəri əlavə edildi.");
                break;

            case "2":
                var customers = await service.GetAllCustomersAsync();

                foreach (var item in customers)
                    Console.WriteLine(item);

                break;

            case "3":
                var found = await service.GetCustomerByIdAsync(
                    ReadInt("Müştəri ID-si: "));

                Console.WriteLine(found?.ToString()
                    ?? "Müştəri tapılmadı.");
                break;

            case "4":
                await service.DeleteCustomerAsync(
                    ReadInt("Müştəri ID-si: "));

                Console.WriteLine("Əməliyyat tamamlandı.");
                break;

            case "5":
                var history = await service.GetCustomerRentalHistoryAsync(
                    ReadInt("Müştəri ID-si: "));

                foreach (var rental in history)
                    Console.WriteLine(rental);

                break;

            default:
                Console.WriteLine("Yanlış seçim.");
                break;
        }
    }

    // ================= İCARƏ =================

    private static async Task CreateReservation(
        AppDbContext context,
        ICustomerService customerService,
        IRentalService rentalService)
    {
        int customerId = ReadInt("Müştəri ID-si: ");
        bool canRent = await customerService.CanCustomerRentAsync(customerId);
        if (!canRent)
        {
            Console.WriteLine("Bu müştəri icarə üçün uyğun deyil (vəsiqə müddəti bitmişdir). ");
            return;
        }

        int carId = ReadInt("Avtomobil ID-si: ");
        var car = await context.Cars.FindAsync(carId);
        if (car == null || !car.IsAvailable)
        {
            Console.WriteLine("Avtomobil mövcud deyil və ya istifadə üçün əlçatan deyil.");
            return;
        }

        DateTime expected = ReadDate("Gözlənilən qaytarma tarixi (dd.MM.yyyy): ");

        var rental = await rentalService.CreateRentalAsync(carId, customerId, expected, DateTime.MinValue);

        // Hesablanmış qiymət
        int days = (int)Math.Ceiling((expected - rental.StartDate).TotalDays);
        if (days <= 0) days = 1;
        rental.TotalPrice = days * car.DailyPrice;
        await context.SaveChangesAsync();

        Console.WriteLine($"Rezervasiya yaradıldı. İcarə ID: {rental.Id}, Ümumi: {rental.TotalPrice}");
    }

    private static async Task ReturnRental(IRentalService rentalService)
    {
        int id = ReadInt("İcarə ID-si: ");
        await rentalService.ReturnRentalAsync(id);
        Console.WriteLine("İcarə qaytarıldı.");
    }

    private static async Task CancelRental(IRentalService rentalService)
    {
        int id = ReadInt("İcarə ID-si: ");
        await rentalService.CancelRentalAsync(id);
        Console.WriteLine("İcarə ləğv edildi.");
    }

    private static async Task MakePayment(AppDbContext context, IPaymentService paymentService)
    {
        Console.WriteLine("1. Ödəniş əlavə et");
        Console.WriteLine("2. Bütün ödənişləri göstər");
        var choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                int rentalId = ReadInt("İcarə ID-si: ");
                double amount = ReadDouble("Məbləğ: ");
                var ptype = ReadText("Ödəniş növü (Cash/Card): ");
                var payment = new Payment
                {
                    RentalId = rentalId,
                    Amount = amount,
                    PaymentDate = DateTime.Now,
                    PaymentType = Enum.TryParse<PaymentType>(ptype, true, out var pt) ? pt : PaymentType.Penalty
                };
                await paymentService.AddPaymentAsync(payment);
                Console.WriteLine("Ödəniş əlavə olundu.");
                break;

            case "2":
                await paymentService.ShowAllPaymentsAsync();
                break;

            default:
                Console.WriteLine("Yanlış seçim.");
                break;
        }
    }

    private static async Task AddMaintenance(IMaintenanceService maintenanceService)
    {
        var m = new MaintenanceRecord
        {
            CarId = ReadInt("Avtomobil ID-si: "),
            MaintenanceDate = ReadDate("Tarix (dd.MM.yyyy): "),
            Cost = ReadDouble("Xərc: "),
            NextMaintenanceDate = ReadDate("Növbəti texniki tarix (dd.MM.yyyy): ")
        };

        await maintenanceService.AddMaintenanceAsync(m);
        Console.WriteLine("Texniki baxış qeydi əlavə edildi.");
    }

    private static async Task ReportMenu(AppDbContext context)
    {
        Console.WriteLine("1. Ən çox icarə olunmuş avtomobillər");
        Console.WriteLine("2. Heç icarə olunmamış avtomobillər");
        Console.WriteLine("3. Avtomobillərin təmir xərclərinə görə sıralama");
        Console.WriteLine("4. Verilmiş tarixdə boş avtomobillər");
        Console.WriteLine("5. Aktiv icarələr");
        Console.WriteLine("6. Gecikmiş icarələr");
        Console.WriteLine("7. Avtomobillərə görə gəlir");
        Console.WriteLine("8. Müştərilərin orta icarə müddəti");
        Console.WriteLine("9. Aylıq gəlir");
        Console.WriteLine("10. Ödənilməmiş icarələr");
        Console.WriteLine("11. Tarix aralığında ödənişlər");
        Console.WriteLine("12. Ödənilmiş cərimələr");

        switch (Console.ReadLine())
        {
            case "1":
                var mostRented = await context.Cars
                    .Select(c => new
                    {
                        Car = c,
                        Count = c.Rentals.Count(
                            r => r.Status != "Cancelled")
                    })
                    .OrderByDescending(x => x.Count)
                    .ToListAsync();

                foreach (var x in mostRented)
                    Console.WriteLine(
                        $"{x.Car.Brand} {x.Car.Model} — {x.Count} icarə");
                break;

            case "2":
                var neverRented = await context.Cars
                    .Where(c => !c.Rentals.Any(
                        r => r.Status != "Cancelled"))
                    .ToListAsync();

                foreach (var car in neverRented)
                    Console.WriteLine(car);
                break;

            case "3":
                var maintenanceCosts = await context.Cars
                    .Select(c => new
                    {
                        Car = c,
                        Cost = c.MaintenanceRecords
                            .Sum(m => (double?)m.Cost) ?? 0
                    })
                    .OrderByDescending(x => x.Cost)
                    .ToListAsync();

                foreach (var x in maintenanceCosts)
                    Console.WriteLine(
                        $"{x.Car.Brand} {x.Car.Model} — {x.Cost}");
                break;

            case "4":
                DateTime date = ReadDate("Yoxlanacaq tarix: ");

                var freeCars = await context.Cars
                    .Where(c => c.IsAvailable)
                    .Where(c => !c.Rentals.Any(r =>
                        r.Status != "Cancelled" &&
                        r.Status != "Returned" &&
                        r.StartDate.Date <= date.Date &&
                        r.ExpectedReturnDate.Date > date.Date))
                    .ToListAsync();

                foreach (var car in freeCars)
                    Console.WriteLine(car);
                break;

            case "5":
                var activeRentals = await context.Rentals
                    .Include(r => r.Car)
                    .Include(r => r.Customer)
                    .Where(r => r.Status == "Active")
                    .ToListAsync();

                foreach (var r in activeRentals)
                    Console.WriteLine(r);
                break;

            case "6":
                var overdue = await context.Rentals
                    .Where(r => r.Status == "Active" &&
                                r.ExpectedReturnDate < DateTime.Now)
                    .ToListAsync();

                foreach (var r in overdue)
                    Console.WriteLine(r);
                break;

            case "7":
                var revenue = await context.Rentals
                    .Where(r => r.Status != "Cancelled")
                    .GroupBy(r => new { r.CarId, r.Car.Brand, r.Car.Model })
                    .Select(g => new
                    {
                        g.Key.Brand,
                        g.Key.Model,
                        Income = g.Sum(r => r.TotalPrice)
                    })
                    .OrderByDescending(x => x.Income)
                    .ToListAsync();

                foreach (var x in revenue)
                    Console.WriteLine(
                        $"{x.Brand} {x.Model} — Gəlir: {x.Income}");
                break;

            case "8":
                var durations = await context.Rentals
                    .Where(r => r.ActualReturnDate != null)
                    .GroupBy(r => new
                    {
                        r.CustomerId,
                        r.Customer.FullName
                    })
                    .Select(g => new
                    {
                        g.Key.FullName,
                        AverageDays = g.Average(r =>
                            EF.Functions.DateDiffDay(
                                r.StartDate,
                                r.ActualReturnDate!.Value))
                    })
                    .ToListAsync();

                foreach (var x in durations)
                    Console.WriteLine(
                        $"{x.FullName} — Orta müddət: {x.AverageDays:F1} gün");
                break;

            case "9":
                var monthlyIncome = await context.Rentals
                    .Where(r => r.Status != "Cancelled")
                    .GroupBy(r => new
                    {
                        r.StartDate.Year,
                        r.StartDate.Month
                    })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        Income = g.Sum(r => r.TotalPrice)
                    })
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Month)
                    .ToListAsync();

                foreach (var x in monthlyIncome)
                    Console.WriteLine(
                        $"{x.Month:D2}.{x.Year} — {x.Income}");
                break;

            case "10":
                var unpaidRentals = await context.Rentals
                    .Include(r => r.Payments)
                    .Where(r => r.Status != "Cancelled" &&
                        r.Payments.Sum(p => (double?)p.Amount) < r.TotalPrice)
                    .ToListAsync();

                foreach (var r in unpaidRentals)
                    Console.WriteLine(
                        $"İcarə: {r.Id}, Qiymət: {r.TotalPrice}");
                break;

            case "11":
                DateTime from = ReadDate("Başlanğıc tarix: ");
                DateTime to = ReadDate("Son tarix: ");

                if (to < from)
                    throw new ArgumentException("Tarix aralığı yanlışdır.");

                double totalPayments = await context.Payments
                    .Where(p => p.PaymentDate >= from.Date &&
                                p.PaymentDate < to.Date.AddDays(1))
                    .SumAsync(p => (double?)p.Amount) ?? 0;

                Console.WriteLine($"Toplam ödəniş: {totalPayments}");
                break;

            case "12":
                double penalties = await context.Penalties
                    .Where(p => p.IsPaid)
                    .SumAsync(p => (double?)p.Amount) ?? 0;

                Console.WriteLine($"Ödənilmiş cərimələr: {penalties}");
                break;

            default:
                Console.WriteLine("Yanlış seçim.");
                break;
        }
    }

    // ================= INPUT METODLARI =================

    private static string ReadText(string message)
    {
        while (true)
        {
            Console.Write(message);
            string? value = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(value))
                return value!;

            Console.WriteLine("Dəyər boş ola bilməz.");
        }
    }

    private static int ReadInt(string message)
    {
        while (true)
        {
            Console.Write(message);
            if (int.TryParse(Console.ReadLine(), out var v))
                return v;

            Console.WriteLine("Tam ədəd daxil edin.");
        }
    }

    private static double ReadDouble(string message)
    {
        while (true)
        {
            Console.Write(message);
            if (double.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
                return v;

            Console.WriteLine("Decimal ədəd daxil edin (nöqtə istifadə edin).");
        }
    }

    private static DateTime ReadDate(string message)
    {
        while (true)
        {
            Console.Write(message);
            var input = Console.ReadLine();
            if (TryParseDate(input, out var dt))
                return dt;

            Console.WriteLine("Yanlış tarix formatı. İstifadə: dd.MM.yyyy və ya yyyy-MM-dd");
        }
    }

    private static bool TryParseDate(string? input, out DateTime result)
    {
        result = DateTime.MinValue;
        if (string.IsNullOrWhiteSpace(input))
            return false;

        foreach (var f in DateFormats)
        {
            if (DateTime.TryParseExact(input, f, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                return true;
        }

        return false;
    }
}
