using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamIndia.TalentFlow.Application.Interfaces;

namespace TeamIndia.TalentFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost("mentors/approve")]
    public async Task<IActionResult> ApproveMentor([FromBody] string email)
    {
        var res = await _adminService.ApproveMentorAsync(email);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("mentors/reject")]
    public async Task<IActionResult> RejectMentor([FromBody] string email)
    {
        var res = await _adminService.RejectMentorAsync(email);
        return StatusCode(res.StatusCode, res);
    }
}
