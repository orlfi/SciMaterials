using FluentValidation;

namespace SciMaterials.UI.BWASM.Models.Validations;

public class SignUpFormValidator : MudBlazorValidator<SignUpForm>
{
    public SignUpFormValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(255).WithMessage("Name length can't be more than 255.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .Length(8, 30).WithMessage("Password must be at least 8 characters long.");
    }
}