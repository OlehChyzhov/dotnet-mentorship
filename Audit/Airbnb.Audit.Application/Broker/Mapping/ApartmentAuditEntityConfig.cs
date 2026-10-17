using Airbnb.Contracts.Broker.Messages.Apartment;
using Mapster;

namespace Airbnb.Audit.Application.Broker.Mapping;

public class ApartmentAuditEntityConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // ApartmentCreated => ApartmentAuditEntity
        config.NewConfig<ApartmentCreated, ApartmentAuditEntityConfig>();
        
        // ApartmentUpdated => ApartmentAuditEntity
        config.NewConfig<ApartmentUpdated, ApartmentAuditEntityConfig>();
        
        // ApartmentDeleted => ApartmentAuditEntity
        config.NewConfig<ApartmentDeleted, ApartmentAuditEntityConfig>();
    }
}