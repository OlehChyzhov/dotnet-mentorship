using Airbnb.Contracts.Broker.Messages.User;
using FluentValidation;

namespace Airbnb.Audit.Application.Broker.Validation;

public class UserCreatedMessageValidator : AbstractValidator<UserCreated>
{
    public UserCreatedMessageValidator()
    {
        RuleFor(user => user.UserId)
            .NotEmpty();
        
        RuleFor(user => user.UserName)
            .NotEmpty();
        
        RuleFor(user => user.Email)
            .EmailAddress()
            .NotEmpty();
        
        RuleFor(user => user.CreatedAt)
            .NotEmpty();
    }
}