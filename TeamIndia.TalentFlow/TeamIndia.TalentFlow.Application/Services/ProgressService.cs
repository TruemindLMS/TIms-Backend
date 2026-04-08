using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Dtos.Response;
using TeamIndia.TalentFlow.Application.Interfaces;

namespace TeamIndia.TalentFlow.Application.Services
{
    public class ProgressService : IProgressService
    {
        private readonly IProgressRepository _repo;

        public ProgressService(IProgressRepository repo)
        {
            _repo = repo;
        }

        public async Task<BaseResponse<ProgressResponseDto>> GetProgressAsync(Guid courseId, Guid userId)
        {
            try
            {
                var total = await _repo.GetTotalLessonsAsync(courseId);
                var completed = await _repo.GetCompletedLessonsAsync(courseId, userId);

                var totalAssignments = await _repo.GetTotalAssignmentsAsync(courseId);
                var submitted = await _repo.GetSubmittedAssignmentsAsync(courseId, userId);

                var dto = new ProgressResponseDto
                {
                    CourseId = courseId,
                    TotalLessons = total,
                    CompletedLessons = completed,
                    TotalAssignments = totalAssignments,
                    SubmittedAssignments = submitted
                };

                return BaseResponse<ProgressResponseDto>.Ok(dto, "OK", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse<ProgressResponseDto>.Fail("An error occurred", new[] { ex.Message }, 500);
            }

        }

        public async Task<BaseResponse<ProgressResponseDto>> MarkLessonCompletedAsync(Guid lessonId, Guid userId)
        {
            try
            {
                // check if lesson exists and find its course
                var courseId = await _repo.GetLessonCourseIdAsync(lessonId);
                if (!courseId.HasValue) return BaseResponse<ProgressResponseDto>.Fail("Lesson not found", null, 404);

                // if already completed, return current progress
                var has = await _repo.HasLessonCompletionAsync(lessonId, userId);
                if (has)
                {
                    var totalA = await _repo.GetTotalLessonsAsync(courseId.Value);
                    var completedA = await _repo.GetCompletedLessonsAsync(courseId.Value, userId);
                    var totalAssignA = await _repo.GetTotalAssignmentsAsync(courseId.Value);
                    var submittedA = await _repo.GetSubmittedAssignmentsAsync(courseId.Value, userId);

                    var alreadyDto = new ProgressResponseDto
                    {
                        CourseId = courseId.Value,
                        TotalLessons = totalA,
                        CompletedLessons = completedA,
                        TotalAssignments = totalAssignA,
                        SubmittedAssignments = submittedA
                    };

                    return BaseResponse<ProgressResponseDto>.Ok(alreadyDto, "Already completed", 200);
                }

                // add completion
                var completion = new TeamIndia.TalentFlow.Domain.Entities.LessonCompletion
                {
                    LessonCompletionId = Guid.NewGuid(),
                    LessonId = lessonId,
                    UserId = userId,
                    CompletedOnUtc = DateTime.UtcNow
                };
                await _repo.AddLessonCompletionAsync(completion);

                // update progress record
                await _repo.UpsertProgressRecordAsync(courseId.Value, userId);

                // check if user has completed all lessons in course
                var allDone = await _repo.HasCompletedAllLessonsAsync(courseId.Value, userId);

                var total = await _repo.GetTotalLessonsAsync(courseId.Value);
                var completedCount = await _repo.GetCompletedLessonsAsync(courseId.Value, userId);
                var totalAssignments = await _repo.GetTotalAssignmentsAsync(courseId.Value);
                var submitted = await _repo.GetSubmittedAssignmentsAsync(courseId.Value, userId);

                var dto = new ProgressResponseDto
                {
                    CourseId = courseId.Value,
                    TotalLessons = total,
                    CompletedLessons = completedCount,
                    TotalAssignments = totalAssignments,
                    SubmittedAssignments = submitted
                };

                var message = allDone ? "Course completed" : "Lesson marked completed";
                return BaseResponse<ProgressResponseDto>.Ok(dto, message, 200);
            }
            catch (Exception ex)
            {
                return BaseResponse<ProgressResponseDto>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        // Per-course list endpoint removed as requested; summary endpoint remains.

        public async Task<BaseResponse<ProgressSummaryDto>> GetProgressSummaryAsync(Guid userId)
        {
            try
            {
                var taken = await _repo.GetEnrolledCourseCountAsync(userId);
                var completed = await _repo.GetCompletedCoursesCountAsync(userId);
                var pending = taken - completed;

                var dto = new ProgressSummaryDto
                {
                    CoursesTaken = taken,
                    CoursesCompleted = completed,
                    CoursesPending = pending
                };

                return BaseResponse<ProgressSummaryDto>.Ok(dto, "OK", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse<ProgressSummaryDto>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }
    }
}
