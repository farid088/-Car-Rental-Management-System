public class Customer
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string DrivingLicenseNumber { get; set; }
    public DateTime LicenseExpiryDate { get; set; }
    public List<Rental> Rentals { get; set; } = new List<Rental>();
    public override string ToString()
    {
        return $"Customer Id: {Id}, Full Name: {FullName}, Email: {Email}, Driving License Number: {DrivingLicenseNumber}, License Expiry Date: {LicenseExpiryDate.ToShortDateString()}";
    }
}