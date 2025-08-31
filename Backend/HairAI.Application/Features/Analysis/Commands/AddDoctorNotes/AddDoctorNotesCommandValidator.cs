using FluentValidation;

namespace HairAI.Application.Features.Analysis.Commands.AddDoctorNotes;

public class AddDoctorNotesCommandValidator : AbstractValidator<AddDoctorNotesCommand>
{
    public AddDoctorNotesCommandValidator()
    {
        RuleFor(x => x.JobId)
            .NotEqual(Guid.Empty).WithMessage("Job ID is required");

        RuleFor(x => x.DoctorNotes)
            .NotEmpty().WithMessage("Doctor notes are required");
    }
}