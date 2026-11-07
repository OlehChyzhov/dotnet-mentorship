namespace Airbnb.Payment.Domain;

public enum RefundStatus
{
    RefundPending, 
    RefundNotNeeded, 
    RefundSucceeded, 
    RefundFailed
}