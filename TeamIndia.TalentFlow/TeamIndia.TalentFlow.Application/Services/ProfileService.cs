using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Dtos;
using TeamIndia.TalentFlow.Application.Interfaces;
using TeamIndia.TalentFlow.Domain.Entities;
using TeamIndia.TalentFlow.Domain.Enums;

namespace TeamIndia.TalentFlow.Application.Services;

public class ProfileService : IProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly IOnboardingService _onboardingService;
    private readonly IOnboardingRepository _onboardingRepository;

    public ProfileService(IUserRepository userRepository, IOnboardingService onboardingService, IOnboardingRepository onboardingRepository)
    {
        _userRepository = userRepository;
        _onboardingService = onboardingService;
        _onboardingRepository = onboardingRepository;
    }

    public async Task<BaseResponse> UpdateProfileAsync(string userEmail, UpdateProfileRequest request)
    {
        var user = await _userRepository.FindByEmailAsync(userEmail);
        if (user == null) return BaseResponse.Fail("User not found", null, 404);

        var profile = (await _userRepository.GetProfileAsync(user)) ?? new UserProfile { UserId = user.Id };

        if (request.Address != null)
        {
            profile.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();
        }

        if (request.PostalCode != null)
        {
            profile.PostalCode = string.IsNullOrWhiteSpace(request.PostalCode) ? null : request.PostalCode.Trim();
        }

        if (request.Location != null)
        {
            profile.Location = string.IsNullOrWhiteSpace(request.Location) ? null : request.Location.Trim();
        }

        if (request.DateOfBirth.HasValue)
        {
            profile.DateOfBirth = request.DateOfBirth;
        }

        if (request.Gender.HasValue)
        {
            profile.Gender = request.Gender.Value;
        }


        var res = await _userRepository.CreateOrUpdateProfileAsync(profile);
        if (!res.Succeeded) return BaseResponse.Fail("Failed to update profile", res.Errors.Select(e => e.Description), 500);

        if (!string.IsNullOrWhiteSpace(request.PhotoUrl))
        {
            try
            {
                var onboarding = await _onboardingRepository.GetAsync(user.Id) ?? new UserOnboarding { UserId = user.Id };
                onboarding.ProfilePictureUrl = request.PhotoUrl;
                await _onboardingRepository.SaveAsync(onboarding);

            }
            catch
            {

            }
        }

        return BaseResponse.Ok("Profile updated", 200);
    }

    public async Task<BaseResponse<UpdateProfileRequest>> GetProfileAsync(string userEmail)
    {
        var user = await _userRepository.FindByEmailAsync(userEmail);
        if (user == null) return BaseResponse<UpdateProfileRequest>.Fail("User not found", null, 404);

        var profile = await _userRepository.GetProfileAsync(user);

        var dto = new UpdateProfileRequest
        {
            Address = profile?.Address,
            PostalCode = profile?.PostalCode,
            Location = profile?.Location,
            DateOfBirth = profile?.DateOfBirth,
            Gender = profile?.Gender ?? Gender.Male,
            PhotoUrl = null
        };

        try
        {
            var picRes = await _onboardingService.GetProfilePictureUrlAsync(user.Email);
            if (picRes != null && picRes.Success && !string.IsNullOrWhiteSpace(picRes.Data))
            {
                dto.PhotoUrl = picRes.Data;
            }
        }
        catch
        {
        }

        return BaseResponse<UpdateProfileRequest>.Ok(dto, "OK", 200);
    }
}
