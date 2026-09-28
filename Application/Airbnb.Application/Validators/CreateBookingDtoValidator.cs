using Airbnb.Application.DTOs.Booking;
using FluentValidation;

namespace Airbnb.Application.Validators;

public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingDtoValidator()
    {
        RuleFor(x => x.ApartmentId)
            .NotEmpty()
            .WithMessage("ApartmentId is required");

        RuleFor(x => x.CheckIn)
            .NotEmpty()
            .Must(checkIn => checkIn.Date >= DateTime.UtcNow.Date)
            .WithMessage("CheckIn cannot be in the past");

        RuleFor(x => x.CheckOut)
            .NotEmpty()
            .GreaterThan(x => x.CheckIn)
            .WithMessage("CheckOut must be after CheckIn");

        RuleFor(x => x.GuestsCount)
            .GreaterThan(0)
            .WithMessage("GuestsCount must be greater than 0");
    }
}
