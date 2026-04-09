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

    [HttpGet("summary")]
    public async Task<IActionResult> Summary()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim)) return Unauthorized();
        if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

        var res = await _service.GetAssignmentSummaryForUserAsync(userId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("user")]
    public async Task<IActionResult> ForUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim)) return Unauthorized();
        if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

        // support paging and filtering: ?page=1&pageSize=10&filter=keyword&status=pending|submitted|overdue
        var qp = HttpContext.Request.Query;
        var page = 1;
        var pageSize = 20;
        if (int.TryParse(qp["page"].FirstOrDefault(), out var p)) page = Math.Max(1, p);
        if (int.TryParse(qp["pageSize"].FirstOrDefault(), out var ps)) pageSize = Math.Clamp(ps, 1, 200);
        var filter = qp["filter"].FirstOrDefault();
        var status = qp["status"].FirstOrDefault();

        var res = await _service.GetAssignmentsForUserPagedAsync(userId, page, pageSize, filter, status);
        return StatusCode(res.StatusCode, res);
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
    public async Task<IActionResult> SubmitAssignment(Guid assignmentId, [FromForm] CreateSubmissionRequestDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim)) return Unauthorized();
        if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();
        if (dto == null) return BadRequest();

        dto.AssignmentId = assignmentId;

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
}
