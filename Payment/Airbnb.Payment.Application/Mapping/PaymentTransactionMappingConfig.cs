using Airbnb.Payment.Application.DTOs;
using Airbnb.Payment.Domain;
using Mapster;

namespace Airbnb.Payment.Application.Mapping;

public class PaymentTransactionMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // PaymentTransaction => PaymentSessionUrlDto
        config.NewConfig<PaymentTransaction, PaymentSessionUrlDto>()
            .Map(dest => dest.PaymentSessionUrl, src => src.StripePaymentSessionUrl);
    }
}
