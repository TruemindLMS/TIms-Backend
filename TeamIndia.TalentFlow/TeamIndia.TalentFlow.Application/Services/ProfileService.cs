using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Dtos;
using TeamIndia.TalentFlow.Application.Interfaces;
using TeamIndia.TalentFlow.Domain.Entities;
using TeamIndia.TalentFlow.Domain.Enums;

namespace TeamIndia.TalentFlow.Application.Services;

public class ProfileService : IProfileService
{
    private readonly IUserRepository _userRepository;

    public ProfileService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<BaseResponse> UpdateProfileAsync(string userEmail, UpdateProfileRequest request)
    {
        var user = await _userRepository.FindByEmailAsync(userEmail);
        if (user == null) return BaseResponse.Fail("User not found", null, 404);

        var profile = (await _userRepository.GetProfileAsync(user)) ?? new UserProfile { UserId = user.Id };

        profile.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();
        profile.PostalCode = string.IsNullOrWhiteSpace(request.PostalCode) ? null : request.PostalCode.Trim();
        profile.Location = string.IsNullOrWhiteSpace(request.Location) ? null : request.Location.Trim();
        profile.DateOfBirth = request.DateOfBirth;
        profile.Gender = request.Gender ?? Gender.Male;

        var res = await _userRepository.CreateOrUpdateProfileAsync(profile);
        if (!res.Succeeded) return BaseResponse.Fail("Failed to update profile", res.Errors.Select(e => e.Description), 500);

        return BaseResponse.Ok("Profile updated", 200);
    }

    public async Task<BaseResponse<UpdateProfileRequest>> GetProfileAsync(string userEmail)
    {
        var user = await _userRepository.FindByEmailAsync(userEmail);
        if (user == null) return BaseResponse<UpdateProfileRequest>.Fail("User not found", null, 404);

        var profile = await _userRepository.GetProfileAsync(user);
        if (profile == null) return BaseResponse<UpdateProfileRequest>.Ok(new UpdateProfileRequest(), "No profile", 200);

        var dto = new UpdateProfileRequest
        {
            Address = profile.Address,
            PostalCode = profile.PostalCode,
            Location = profile.Location,
            DateOfBirth = profile.DateOfBirth,
            Gender = profile.Gender
        };

        return BaseResponse<UpdateProfileRequest>.Ok(dto, "OK", 200);
    }
}
