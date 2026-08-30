using Airbnb.Application.DTOs.External;
using FluentValidation;

namespace Airbnb.Application.Validators;

public class ExternalHostDtoValidator : AbstractValidator<ExternalHostDto>
{
    public ExternalHostDtoValidator()
    {
        RuleFor(x => x.ExternalId)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address");

        RuleForEach(x => x.Apartments).ChildRules(apartment =>
        {
            apartment.RuleFor(x => x.ExternalId)
                .NotEmpty().WithMessage("Id is required");

            apartment.RuleFor(x => x.Title)
                .NotEmpty().MaximumLength(200).WithMessage("Title is required");

            apartment.RuleFor(x => x.Description)
                .NotEmpty().MaximumLength(2000).WithMessage("Description is required");

            apartment.RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Type must be a valid apartment type");

            apartment.RuleFor(x => x.Country)
                .NotEmpty().MaximumLength(100).WithMessage("Country is required");

            apartment.RuleFor(x => x.City)
                .NotEmpty().MaximumLength(100).WithMessage("City is required");

            apartment.RuleFor(x => x.Address)
                .NotEmpty().MaximumLength(300).WithMessage("Address is required");

            apartment.RuleFor(x => x.MaxGuests)
                .GreaterThan(0).WithMessage("MaxGuests must be greater than 0");

            apartment.RuleFor(x => x.Bedrooms)
                .GreaterThanOrEqualTo(0).WithMessage("Bedrooms cannot be negative");

            apartment.RuleFor(x => x.Bathrooms)
                .GreaterThanOrEqualTo(0).WithMessage("Bathrooms cannot be negative");

            apartment.RuleFor(x => x.Kitchens)
                .GreaterThanOrEqualTo(0).WithMessage("Kitchens cannot be negative");

            apartment.RuleFor(x => x.LivingRooms)
                .GreaterThanOrEqualTo(0).WithMessage("LivingRooms cannot be negative");

            apartment.RuleFor(x => x.PricePerNight)
                .GreaterThan(0).WithMessage("PricePerNight must be greater than 0");
        });
    }
}
