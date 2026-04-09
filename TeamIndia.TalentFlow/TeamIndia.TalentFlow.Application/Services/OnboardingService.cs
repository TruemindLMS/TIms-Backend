using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Dtos;
using TeamIndia.TalentFlow.Application.Interfaces;
using TeamIndia.TalentFlow.Domain.Entities;
using TeamIndia.TalentFlow.Domain.Enums;

namespace TeamIndia.TalentFlow.Application.Services;

public class OnboardingService : IOnboardingService
{
    private readonly IOnboardingRepository _repo;
    private readonly IUserRepository _userRepo;

    public OnboardingService(IOnboardingRepository repo, IUserRepository userRepo)
    {
        _repo = repo;
        _userRepo = userRepo;
    }

    public async Task<BaseResponse> SaveAsync(string userEmail, string? bio, string? profilePictureUrl, Discipline? discipline, Goal? goal)
    {
        var user = await _userRepo.FindByEmailAsync(userEmail);
        if (user == null) return BaseResponse.Fail("User not found", null, 404);

        var onboarding = await _repo.GetAsync(user.Id) ?? new UserOnboarding { UserId = user.Id };
        if (bio != null) onboarding.Bio = string.IsNullOrWhiteSpace(bio) ? null : bio.Trim();
        if (profilePictureUrl != null) onboarding.ProfilePictureUrl = profilePictureUrl;
        if (discipline.HasValue) onboarding.Discipline = discipline.Value;
        if (goal.HasValue) onboarding.Goal = goal.Value;

        onboarding.IsComplete = onboarding.Bio != null && onboarding.ProfilePictureUrl != null && onboarding.Discipline != null && onboarding.Goal != null;
        onboarding.UpdatedAtUtc = DateTime.UtcNow;

        var res = await _repo.SaveAsync(onboarding);
        if (!res.Succeeded) return BaseResponse.Fail("Failed to save onboarding", res.Errors.Select(e => e.Description), 500);

        return BaseResponse.Ok("Onboarding saved", 200);
    }

    public async Task<BaseResponse> GetOnboardingStatusAsync(string userEmail)
    {
        var user = await _userRepo.FindByEmailAsync(userEmail);
        if (user == null) return BaseResponse.Fail("User not found", null, 404);

        var onboarding = await _repo.GetAsync(user.Id);

        var dto = new OnboardingStatusDto
        {
            BioCompleted = !string.IsNullOrWhiteSpace(onboarding?.Bio),
            DisciplineCompleted = onboarding?.Discipline != null,
            GoalCompleted = onboarding?.Goal != null,
            IsComplete = onboarding?.IsComplete ?? false
        };

        return BaseResponse<OnboardingStatusDto>.Ok(dto, "OK", 200);
    }

    public async Task<BaseResponse<string>> GetProfilePictureUrlAsync(string userEmail)
    {
        var user = await _userRepo.FindByEmailAsync(userEmail);
        if (user == null) return BaseResponse<string>.Fail("User not found", null, 404);

        var onboarding = await _repo.GetAsync(user.Id);
        var url = onboarding?.ProfilePictureUrl;
        return BaseResponse<string>.Ok(url ?? string.Empty, "OK", 200);
    }

    public async Task<BaseResponse<string>> UploadProfilePictureForUserAsync(Guid userId, Microsoft.AspNetCore.Http.IFormFile file)
    {
        // This method is intentionally left as a pass-through placeholder. Actual file upload is handled by the controller which has access to ICloudinaryService.
        return BaseResponse<string>.Fail("Not implemented here", null, 500);
    }

    public async Task<UserOnboarding?> GetOnboardingAsync(Guid userId)
    {
        return await _repo.GetAsync(userId);
    }
}
