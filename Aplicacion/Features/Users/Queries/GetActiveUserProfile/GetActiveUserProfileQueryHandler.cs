using FoodApp.Aplicacion.Common.Interfaces;
using MediatR;

namespace FoodApp.Aplicacion.Features.Users.Queries.GetActiveUserProfile;

public class GetActiveUserProfileQueryHandler : IRequestHandler<GetActiveUserProfileQuery, string>
{
    private readonly IUserRepository _userRepository;

    public GetActiveUserProfileQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<string> Handle(GetActiveUserProfileQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _userRepository.GetUserNameAsync(cancellationToken);
    }
}
