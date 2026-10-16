using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Runtime.CompilerServices;

public class Car
{
    public int Id { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int PlateNumber { get; set; }
    public double DailyPrice { get; set; }
    public int Year { get; set; }
    public int Mileage { get; set; }
    public bool IsAvailable { get; set; }
    public List<Rental> Rentals { get; set; } = new List<Rental>();
    public List<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
    public override string ToString()
    {
        return $"Car Id: {Id}, Brand: {Brand}, Model: {Model}, Plate Number: {PlateNumber}, Daily Price: {DailyPrice}, Year: {Year}, Mileage: {Mileage}, Is Available: {IsAvailable}";
    }
}