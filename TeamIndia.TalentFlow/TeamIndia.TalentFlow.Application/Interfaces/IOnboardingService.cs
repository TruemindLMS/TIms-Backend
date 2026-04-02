using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Dtos;
using TeamIndia.TalentFlow.Domain.Enums;

namespace TeamIndia.TalentFlow.Application.Interfaces;

public interface IOnboardingService
{
    Task<BaseResponse> SaveAsync(string userEmail, string? bio, string? profilePictureUrl, Discipline? discipline, Goal? goal);
    Task<BaseResponse> GetOnboardingStatusAsync(string userEmail);
}
