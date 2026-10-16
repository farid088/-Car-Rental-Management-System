public class MaintenanceRecord
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public Car Car { get; set; }
    public DateTime MaintenanceDate { get; set; }
    public string Description { get; set; }
    public double Cost { get; set; }
    public DateTime NextMaintenanceDate { get; set; }
    public override string ToString()
    {
        return $"Maintenance Record Id: {Id}, Car Id: {CarId}, Maintenance Date: {MaintenanceDate.ToShortDateString()}, Description: {Description}, Cost: {Cost}, Next Maintenance Date: {NextMaintenanceDate.ToShortDateString()}";
    }
}