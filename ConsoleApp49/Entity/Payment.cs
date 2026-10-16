public class Payment
{
    public int Id { get; set; }
    public int RentalId { get; set; }
    public Rental Rental { get; set; }
    public double Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentType  PaymentType { get; set; }
   
    public override string ToString()
    {
        return $"Payment Id: {Id}, Rental Id: {RentalId}, Amount: {Amount}, Payment Date: {PaymentDate.ToShortDateString()}, Payment Type: {PaymentType}";
    }
}