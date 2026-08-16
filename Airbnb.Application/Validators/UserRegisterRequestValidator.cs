using Airbnb.Domain.Requests;
using Airbnb.Domain.Requests.Authentication;
using FluentValidation;

namespace Airbnb.Application.Validators;

public class UserRegisterRequestValidator : AbstractValidator<UserRegisterRequest>
{
    public UserRegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .NotNull()
            .EmailAddress()
            .WithMessage("Email is required");
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull()
            .WithMessage("Password is required");
        
        RuleFor(x => x.Role)
            .NotEmpty()
            .NotNull()
            .WithMessage("Role is required");
    }
}