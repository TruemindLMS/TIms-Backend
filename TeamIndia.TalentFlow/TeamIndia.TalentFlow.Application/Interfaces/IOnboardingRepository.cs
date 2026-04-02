using Microsoft.AspNetCore.Identity;
using TeamIndia.TalentFlow.Domain.Entities;

namespace TeamIndia.TalentFlow.Application.Interfaces;

public interface IOnboardingRepository
{
    Task<UserOnboarding?> GetAsync(Guid userId);
    Task<IdentityResult> SaveAsync(UserOnboarding onboarding);
    Task<bool> IsOnboardingCompleteAsync(Guid userId);
}
