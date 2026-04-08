using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Dtos.Response;

namespace TeamIndia.TalentFlow.Application.Interfaces
{
    public interface IProgressService
    {
        Task<BaseResponse<ProgressResponseDto>> GetProgressAsync(Guid courseId, Guid userId);
        Task<BaseResponse<ProgressResponseDto>> MarkLessonCompletedAsync(Guid lessonId, Guid userId);
        Task<BaseResponse<ProgressSummaryDto>> GetProgressSummaryAsync(Guid userId);
    }
}
