using Airbnb.Contracts.Broker.Messages.User;
using FluentValidation;

namespace Airbnb.Audit.Application.Broker.Validation;

public class UserEmailChangedMessageValidator : AbstractValidator<UserEmailChanged>
{
    public UserEmailChangedMessageValidator()
    {
        RuleFor(user => user.UserId)
            .NotEmpty();
        
        RuleFor(user => user.UserName)
            .NotEmpty();
        
        RuleFor(user => user.NewEmail)
            .EmailAddress()
            .NotEmpty();
        
        RuleFor(user => user.UserId)
            .NotEmpty();
        
        RuleFor(user => user.UserName)
            .NotEmpty();
        
        RuleFor(user => user.OldEmail)
            .EmailAddress()
            .NotEmpty();
        
        RuleFor(user => user.CreatedAt)
            .NotEmpty();
        
        RuleFor(user => user.CreatedAt)
            .NotEmpty();
    }
}