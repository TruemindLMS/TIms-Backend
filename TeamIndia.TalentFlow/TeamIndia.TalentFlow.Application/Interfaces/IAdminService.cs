using TeamIndia.TalentFlow.Application.Common;

namespace TeamIndia.TalentFlow.Application.Interfaces;

public interface IAdminService
{
    Task<BaseResponse> ApproveMentorAsync(string email);
    Task<BaseResponse> RejectMentorAsync(string email);
}
