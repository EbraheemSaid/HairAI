using MediatR;
using HairAI.Application.Common.Interfaces;

namespace HairAI.Application.Features.Clinics.Commands.UpdateClinic;

public class UpdateClinicCommandHandler : IRequestHandler<UpdateClinicCommand, UpdateClinicCommandResponse>
{
    private readonly IApplicationDbContext _context;

    public UpdateClinicCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateClinicCommandResponse> Handle(UpdateClinicCommand request, CancellationToken cancellationToken)
    {
        var clinic = await _context.Clinics.FindAsync(new object[] { request.ClinicId }, cancellationToken);
        
        if (clinic == null)
        {
            return new UpdateClinicCommandResponse
            {
                Success = false,
                Message = "Clinic not found",
                Errors = new List<string> { "Clinic not found" }
            };
        }

        clinic.Name = request.Name;
        clinic.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new UpdateClinicCommandResponse
        {
            Success = true,
            Message = "Clinic updated successfully"
        };
    }
}