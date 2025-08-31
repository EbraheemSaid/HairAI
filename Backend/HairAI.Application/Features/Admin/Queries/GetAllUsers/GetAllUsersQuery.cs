using MediatR;

namespace HairAI.Application.Features.Admin.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<GetAllUsersQueryResponse>
{
    // No parameters needed for getting all users
}

