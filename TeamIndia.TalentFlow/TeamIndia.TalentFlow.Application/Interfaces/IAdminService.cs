using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Dtos.Response;

namespace TeamIndia.TalentFlow.Application.Interfaces;

public interface IAdminService
{
    Task<BaseResponse> ApproveMentorAsync(string email);
    Task<BaseResponse> RejectMentorAsync(string email);
}
