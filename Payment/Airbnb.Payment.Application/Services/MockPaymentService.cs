using Airbnb.Payment.Application.Abstractions;

namespace Airbnb.Payment.Application.Services;

public class MockPaymentService : IPaymentService
{
    private readonly Random _random;
    
    public MockPaymentService()
    {
        _random = new Random();
    }
    
    public bool Pay()
    {
        if (_random.Next() >= 0.5)
        {
            return true;
        }

        return false;
    }
}