public class Penalty
{
    public int Id { get; set; }
    public int RentalId { get; set; }
    public Rental Rental { get; set; }
    public double Amount { get; set; }
    public bool IsPaid { get; set; }
    public string Reason { get; set; }
    public override string ToString()
    {
        return $"Penalty Id: {Id}, Rental Id: {RentalId}, Amount: {Amount}, Is Paid: {IsPaid}, Reason: {Reason}";
    }
}