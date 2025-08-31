using FluentValidation;

namespace HairAI.Application.Features.Invitations.Queries.GetInvitationByToken;

public class GetInvitationByTokenQueryValidator : AbstractValidator<GetInvitationByTokenQuery>
{
    public GetInvitationByTokenQueryValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required");
    }
}