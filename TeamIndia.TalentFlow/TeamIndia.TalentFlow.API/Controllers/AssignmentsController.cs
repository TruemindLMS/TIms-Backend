using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamIndia.TalentFlow.Application.Dtos.Request;
using TeamIndia.TalentFlow.Application.Interfaces;

namespace TeamIndia.TalentFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentService _service;
    private readonly ICourseRepository _courseRepo;

    public AssignmentsController(IAssignmentService service, ICourseRepository courseRepo)
    {
        _service = service;
        _courseRepo = courseRepo;
    }

    [HttpPost]
    [Authorize(Roles = "Mentor")]
    public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentRequestDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim)) return Unauthorized();
        if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

        var course = await _courseRepo.GetCourseWithDetailsAsync(dto.CourseId);
        if (course == null) return NotFound();

        if (course.MentorId.HasValue && course.MentorId.Value != userId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var res = await _service.CreateAssignmentAsync(dto, userId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("{assignmentId:guid}/submit")]
    public async Task<IActionResult> SubmitAssignment(Guid assignmentId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim)) return Unauthorized();
        if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

        // support both multipart/form-data and json
        if (Request.HasFormContentType)
        {
            var form = await Request.ReadFormAsync();
            var text = form["TextResponse"].FirstOrDefault();
            var link = form["LinkUrl"].FirstOrDefault();
            var file = form.Files.FirstOrDefault();

            var dto = new CreateSubmissionRequestDto
            {
                AssignmentId = assignmentId,
                TextResponse = text,
                LinkUrl = link,
                File = file
            };

            var assignmentRes = await _service.GetAssignmentAsync(assignmentId);
            if (assignmentRes == null || assignmentRes.StatusCode == 404) return NotFound();
            var courseId = assignmentRes.Data.CourseId;

            var isEnrolled = await _courseRepo.IsUserEnrolledAsync(courseId, userId);
            if (!isEnrolled && !User.IsInRole("Admin") && !User.IsInRole("Mentor"))
            {
                return Forbid();
            }

            var res = await _service.SubmitAssignmentAsync(dto, userId);
            return StatusCode(res.StatusCode, res);
        }
        else
        {
            var body = await System.Text.Json.JsonSerializer.DeserializeAsync<CreateSubmissionRequestDto>(Request.Body);
            if (body == null) return BadRequest();
            body.AssignmentId = assignmentId;
            var assignmentRes = await _service.GetAssignmentAsync(assignmentId);
            if (assignmentRes == null || assignmentRes.StatusCode == 404) return NotFound();
            var courseId = assignmentRes.Data.CourseId;

            var isEnrolled = await _courseRepo.IsUserEnrolledAsync(courseId, userId);
            if (!isEnrolled && !User.IsInRole("Admin") && !User.IsInRole("Mentor"))
            {
                return Forbid();
            }

            var res = await _service.SubmitAssignmentAsync(body, userId);
            return StatusCode(res.StatusCode, res);
        }
    }
}
