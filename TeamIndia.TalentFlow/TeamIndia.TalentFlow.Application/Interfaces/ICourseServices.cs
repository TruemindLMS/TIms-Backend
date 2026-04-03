using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Dtos;
using TeamIndia.TalentFlow.Application.Dtos.Response;

namespace TeamIndia.TalentFlow.Application.Interfaces
{
    public interface ICourseServices
    {
        Task<BaseResponse<CourseResponseDto>> CreateCourseAsync(CreateCourseRequestDto request);
        Task<BaseResponse<IEnumerable<CourseResponseDto>>> GetAllCoursesAsync();
        Task<BaseResponse<CourseResponseDto?>> GetCourseByIdAsync(Guid courseId);
        Task<BaseResponse<CourseResponseDto>> UpdateCourseAsync(Guid courseId, CreateCourseRequestDto request);
        Task<BaseResponse<ModuleResponseDto>> UpdateModuleAsync(Guid moduleId, CreateModuleRequestDto request);
        Task<BaseResponse<LessonResponseDto>> UpdateLessonAsync(Guid lessonId, CreateLessonRequestDto request);
        Task<BaseResponse> DeleteCourseAsync(Guid courseId);
        Task<BaseResponse> DeleteModuleAsync(Guid moduleId);
        Task<BaseResponse> DeleteLessonAsync(Guid lessonId);
        Task<BaseResponse<ModuleResponseDto>> CreateModuleAsync(Guid courseId, CreateModuleRequestDto request);
        Task<BaseResponse<LessonResponseDto>> CreateLessonAsync(Guid courseId, Guid moduleId, CreateLessonRequestDto request);
    }
}
