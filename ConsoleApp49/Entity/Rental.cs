public class Rental
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public Car Car { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpectedReturnDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public double TotalPrice { get; set; }
    public double DailyPrice { get; set; }
    public string Status { get; set; }
    public List<Payment> Payments { get; set; } = new List<Payment>();
    public List<Penalty> Penalties { get; set; } = new List<Penalty>();

    public override string ToString()
    {
        return $"Rental Id: {Id}, Car Id: {CarId}, Customer Id: {CustomerId}, Start Date: {StartDate.ToShortDateString()}, Expected Return Date: {ExpectedReturnDate.ToShortDateString()}, Actual Return Date: {(ActualReturnDate.HasValue ? ActualReturnDate.Value.ToShortDateString() : "Not Returned")}, Total Price: {TotalPrice}, Daily Price: {DailyPrice}, Status: {Status}";
    }
}