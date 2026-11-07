namespace Airbnb.Shared.Contracts.Saga.BookingPayment;

// Step 5
public class RevertUpdatingTransactionAsSuccessCommand
{
    public Guid TransactionId { get; set; }
    public Guid BookingId { get; set; }
}

public class RevertUpdatingTransactionAsSuccessCommandExecuted
{
    public Guid TransactionId { get; set; }
    public Guid BookingId { get; set; }
}

// Step 4
public class RevertUpdatingBookingAsSuccessCommand
{
    public Guid TransactionId { get; set; }
    public Guid BookingId { get; set; }
}

public class RevertingUpdatingBookingAsSuccessCommandExecuted
{
    public Guid TransactionId { get; set; }
    public Guid BookingId { get; set; }
}

// Step 3
public class RefundPaidAmountCommand
{
    public Guid TransactionId { get; set; }
    public Guid BookingId { get; set; }
}

public class RefundingPaidAmountCommandExecuted
{
    public Guid TransactionId { get; set; }
    public Guid BookingId { get; set; }
}

// Step 2
public class InvalidatingPaymentSessionCommand
{
    public Guid TransactionId { get; set; }
    public Guid BookingId { get; set; }
}

public class InvalidatingPaymentSessionCommandExecuted
{
    public Guid TransactionId { get; set; }
    public Guid BookingId { get; set; }
}

// Step 1
public class RevertTransactionCreateCommand
{
    public Guid TransactionId { get; set; }
    public Guid BookingId { get; set; }
}

public class RevertingTransactionCreateCommandExecuted
{
    public Guid TransactionId { get; set; }
    public Guid BookingId { get; set; }
}