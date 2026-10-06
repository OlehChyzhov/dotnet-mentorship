using Airbnb.Application.DTOs.Users;
using FluentValidation;

namespace Airbnb.Application.Validators;

public class UpdateEmailDtoValidator : AbstractValidator<UpdateEmailDto>
{
    public UpdateEmailDtoValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("A valid email is required");
    }
}
