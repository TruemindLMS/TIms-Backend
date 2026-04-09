using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamIndia.TalentFlow.Application.Interfaces;

namespace TeamIndia.TalentFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IAdminService _adminService;

    public UsersController(IAdminService adminService)
    {
        _adminService = adminService;
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
}
