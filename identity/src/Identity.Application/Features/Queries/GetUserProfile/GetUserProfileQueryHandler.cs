using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using MediatR;

namespace Identity.Application.Features.Queries.GetUserProfile;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly IKycRepository _kycRepository;

    public GetUserProfileQueryHandler(IUserRepository userRepository, IKycRepository kycRepository)
    {
        _userRepository = userRepository;
        _kycRepository = kycRepository;
    }

    public async Task<UserProfileDto?> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null) return null;

        var kyc = await _kycRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (kyc is null) return null;

        return new UserProfileDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            KycData = new IdentityDataDto
            {
                FullName = user.FullName,
                NationalId = kyc.NationalId,
                DateOfBirth = kyc.DateOfBirth,
                Address = kyc.Address,
                PhoneNumber = kyc.PhoneNumber,
                Email = kyc.Email
            }
        };
    }
}
