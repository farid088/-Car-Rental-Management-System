public class CarNotFoundException : Exception
{
    public CarNotFoundException(string message) : base(message)
    {
    }
}


public class CustomerNotFoundException : Exception
{
    public CustomerNotFoundException(string message) : base(message)
    {
    }
}



public class InvalidRentalDateException : Exception
{
    public InvalidRentalDateException(string message) : base(message)
    {
    }
}

public class CarAlreadyReservedException : Exception
{
    public CarAlreadyReservedException(string message) : base(message)
    {
    }
}


public class LicenseExpiredException : Exception
{
    public LicenseExpiredException(string message) : base(message)
    {
    }
}

public class RentalAlreadyCompletedException : Exception
{
    public RentalAlreadyCompletedException(string message) : base(message)
    {
    }
}
public class PaymentExceedsDebtException : Exception
{
    public PaymentExceedsDebtException(string message) : base(message)
    {
    }
}