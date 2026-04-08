using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamIndia.TalentFlow.Application.Interfaces;

namespace TeamIndia.TalentFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProgressController : ControllerBase
{
    private readonly IProgressService _progressService;

    public ProgressController(IProgressService progressService)
    {
        _progressService = progressService;
    }

    [HttpGet("courses/{courseId:guid}")]
    public async Task<IActionResult> GetCourseProgress(Guid courseId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim)) return Unauthorized();

        if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

        var res = await _progressService.GetProgressAsync(courseId, userId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("lessons/{lessonId:guid}/complete")]
    public async Task<IActionResult> MarkLessonCompleted(Guid lessonId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim)) return Unauthorized();

        if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

        var res = await _progressService.MarkLessonCompletedAsync(lessonId, userId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetProgressSummary()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim)) return Unauthorized();

        if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

        var res = await _progressService.GetProgressSummaryAsync(userId);
        return StatusCode(res.StatusCode, res);
    }
}
