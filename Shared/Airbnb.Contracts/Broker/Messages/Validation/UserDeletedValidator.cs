using Airbnb.Contracts.Broker.Messages.User;
using FluentValidation;

namespace Airbnb.Contracts.Broker.Messages.Validation;

public class UserDeletedValidator : AbstractValidator<UserDeleted>
{
    public UserDeletedValidator()
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