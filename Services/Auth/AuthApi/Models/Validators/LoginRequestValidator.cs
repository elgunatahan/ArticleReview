using AuthApi.Models.Requests;
using FluentValidation;

namespace AuthApi.Models.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(p => p.Username).NotEmpty();
            RuleFor(p => p.Password).NotEmpty();
        }
    }
}
