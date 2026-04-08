using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Dtos.Request;
using TeamIndia.TalentFlow.Application.Dtos.Response;
using TeamIndia.TalentFlow.Application.Interfaces;
using TeamIndia.TalentFlow.Domain.Entities;

namespace TeamIndia.TalentFlow.Application.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _repo;
        private readonly ICloudinaryService _cloudinary;

        public AssignmentService(IAssignmentRepository repo, ICloudinaryService cloudinary)
        {
            _repo = repo;
            _cloudinary = cloudinary;
        }

        public async Task<BaseResponse<AssignmentResponseDto>> CreateAssignmentAsync(CreateAssignmentRequestDto dto, Guid mentorId)
        {
            try
            {
                // verify mentor role is handled at controller level; course existence is validated in controller
                var assignment = new Assignment
                {
                    AssignmentId = Guid.NewGuid(),
                    CourseId = dto.CourseId,
                    LessonId = dto.LessonId,
                    Title = dto.Title,
                    Description = dto.Description,
                    DueDateUtc = dto.DueDateUtc ?? DateTime.UtcNow.AddDays(7)
                };

                await _repo.AddAssignmentAsync(assignment);

                var res = new AssignmentResponseDto
                {
                    AssignmentId = assignment.AssignmentId,
                    CourseId = assignment.CourseId,
                    LessonId = assignment.LessonId,
                    Title = assignment.Title,
                    Description = assignment.Description,
                    DueDateUtc = assignment.DueDateUtc
                };

                return BaseResponse<AssignmentResponseDto>.Ok(res, "Created", 201);
            }
            catch (Exception ex)
            {
                return BaseResponse<AssignmentResponseDto>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        public async Task<BaseResponse<SubmissionResponseDto>> SubmitAssignmentAsync(CreateSubmissionRequestDto dto, Guid userId)
        {
            try
            {
                var assignment = await _repo.GetAssignmentAsync(dto.AssignmentId);
                if (assignment == null) return BaseResponse<SubmissionResponseDto>.Fail("Assignment not found", null, 404);

                // Enrollment is validated at the controller level before calling this service

                // check if already submitted
                var existing = await _repo.GetSubmissionByAssignmentAndUserAsync(dto.AssignmentId, userId);
                if (existing != null) return BaseResponse<SubmissionResponseDto>.Fail("Already submitted", null, 409);

                string? fileUrl = null;
                if (dto.File != null)
                {
                    fileUrl = await _cloudinary.UploadAssignmentFileAsync(dto.File, userId.ToString(), dto.AssignmentId);
                }

                var submission = new Submission
                {
                    SubmissionId = Guid.NewGuid(),
                    AssignmentId = dto.AssignmentId,
                    UserId = userId,
                    TextResponse = dto.TextResponse,
                    FileUrl = fileUrl,
                    LinkUrl = dto.LinkUrl,
                    SubmittedOnUtc = DateTime.UtcNow
                };

                await _repo.AddSubmissionAsync(submission);

                var resp = new SubmissionResponseDto
                {
                    SubmissionId = submission.SubmissionId,
                    AssignmentId = submission.AssignmentId,
                    UserId = submission.UserId,
                    TextResponse = submission.TextResponse,
                    FileUrl = submission.FileUrl,
                    LinkUrl = submission.LinkUrl,
                    SubmittedOnUtc = submission.SubmittedOnUtc
                };

                return BaseResponse<SubmissionResponseDto>.Ok(resp, "Submitted", 201);
            }
            catch (Exception ex)
            {
                return BaseResponse<SubmissionResponseDto>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        public async Task<BaseResponse<AssignmentResponseDto>> GetAssignmentAsync(Guid assignmentId)
        {
            try
            {
                var assignment = await _repo.GetAssignmentAsync(assignmentId);
                if (assignment == null) return BaseResponse<AssignmentResponseDto>.Fail("Not found", null, 404);

                var res = new AssignmentResponseDto
                {
                    AssignmentId = assignment.AssignmentId,
                    CourseId = assignment.CourseId,
                    LessonId = assignment.LessonId,
                    Title = assignment.Title,
                    Description = assignment.Description,
                    DueDateUtc = assignment.DueDateUtc
                };

                return BaseResponse<AssignmentResponseDto>.Ok(res, "OK", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse<AssignmentResponseDto>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }
    }
}
