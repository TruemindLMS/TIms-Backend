using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeamIndia.TalentFlow.Application.Interfaces;
using TeamIndia.TalentFlow.Domain.Entities;
using TeamIndia.TalentFlow.Infrastructure.DbContext;

namespace TeamIndia.TalentFlow.Infrastructure.Repositories;

public class OnboardingRepository : IOnboardingRepository
{
    private readonly ApplicationDbContext _db;

    public OnboardingRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<UserOnboarding?> GetAsync(Guid userId)
    {
        return await _db.UserOnboardings.FirstOrDefaultAsync(o => o.UserId == userId);
    }

    public async Task<IdentityResult> SaveAsync(UserOnboarding onboarding)
    {
        var existing = await _db.UserOnboardings.FirstOrDefaultAsync(o => o.UserId == onboarding.UserId);
        if (existing == null)
        {
            onboarding.Id = Guid.NewGuid();
            await _db.UserOnboardings.AddAsync(onboarding);
        }
        else
        {
            existing.Bio = onboarding.Bio;
            existing.ProfilePictureUrl = onboarding.ProfilePictureUrl;
            existing.Discipline = onboarding.Discipline;
            existing.Goal = onboarding.Goal;
            existing.IsComplete = onboarding.IsComplete;
            existing.UpdatedAtUtc = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        return IdentityResult.Success;
    }

    public async Task<bool> IsOnboardingCompleteAsync(Guid userId)
    {
        var o = await GetAsync(userId);
        return o != null && o.IsComplete;
    }
}
