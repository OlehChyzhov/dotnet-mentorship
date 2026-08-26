using Airbnb.Application.DTOs.Apartment;
using FluentValidation;

namespace Airbnb.Application.Validators;

public class CreateApartmentDtoValidator : AbstractValidator<CreateApartmentDto>
{
    public CreateApartmentDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Title is required");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(2000)
            .WithMessage("Description is required");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Type must be a valid apartment type");

        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Country is required");

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("City is required");

        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(300)
            .WithMessage("Address is required");

        RuleFor(x => x.MaxGuests)
            .GreaterThan(0)
            .WithMessage("MaxGuests must be greater than 0");

        RuleFor(x => x.Bedrooms)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Bedrooms cannot be negative");

        RuleFor(x => x.Bathrooms)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Bathrooms cannot be negative");

        RuleFor(x => x.Kitchens)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Kitchens cannot be negative");

        RuleFor(x => x.LivingRooms)
            .GreaterThanOrEqualTo(0)
            .WithMessage("LivingRooms cannot be negative");

        RuleFor(x => x.PricePerNight)
            .GreaterThan(0)
            .WithMessage("PricePerNight must be greater than 0");
    }
}
