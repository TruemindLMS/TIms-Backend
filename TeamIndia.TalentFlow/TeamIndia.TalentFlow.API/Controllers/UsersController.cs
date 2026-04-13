using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamIndia.TalentFlow.Application.Interfaces;

namespace TeamIndia.TalentFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly ITeamServices _teamServices;

    public UsersController(IAdminService adminService, ITeamServices teamServices)
    {
        _adminService = adminService;
        _teamServices = teamServices;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers([FromServices] IUserService userService)
    {
        var res = await userService.GetAllUsersAsync();
        return Ok(res);
    }

    [Authorize]
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        var userService = HttpContext.RequestServices.GetRequiredService<IUserService>();
        var res = await userService.GetUserByIdAsync(userId);
        if (res == null) return NotFound();
        return Ok(res);
    }

    [Authorize]
    [HttpGet("my-team")]
    public async Task<IActionResult> GetMyTeams([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value
                          ?? User.FindFirst("id")?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim)) return Unauthorized();
        if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

        var res = await _teamServices.GetTeamsForUserAsync(userId, page, pageSize);
        return StatusCode(res.StatusCode, res);
    }

}
