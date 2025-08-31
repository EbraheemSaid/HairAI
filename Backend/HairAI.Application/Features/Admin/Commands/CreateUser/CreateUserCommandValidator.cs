using FluentValidation;

namespace HairAI.Application.Features.Admin.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(50)
            .WithMessage("First name must not exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(50)
            .WithMessage("Last name must not exceed 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email must be a valid email address")
            .MaximumLength(100)
            .WithMessage("Email must not exceed 100 characters");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role is required")
            .Must(role => role == "SuperAdmin" || role == "ClinicAdmin" || role == "Doctor")
            .WithMessage("Role must be SuperAdmin, ClinicAdmin, or Doctor");

        RuleFor(x => x.ClinicId)
            .NotEmpty()
            .WithMessage("Clinic is required for ClinicAdmin and Doctor roles")
            .When(x => x.Role == "ClinicAdmin" || x.Role == "Doctor");

        RuleFor(x => x.ClinicId)
            .Empty()
            .WithMessage("SuperAdmin should not be assigned to a clinic")
            .When(x => x.Role == "SuperAdmin");
    }
}

