using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamIndia.TalentFlow.Application.Dtos;
using TeamIndia.TalentFlow.Application.Interfaces;

namespace TeamIndia.TalentFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamServices _teamService;

        public TeamController(ITeamServices teamService)
        {
            _teamService = teamService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<IActionResult> CreateTeam([FromBody] CreateTeamRequestDto request)
        {
            var res = await _teamService.CreateTeamAsync(request);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTeams()
        {
            var res = await _teamService.GetAllTeamsAsync();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("{teamId:guid}")]
        public async Task<IActionResult> GetTeamById(Guid teamId)
        {
            var res = await _teamService.GetTeamByIdAsync(teamId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("{teamId:guid}/members")]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<IActionResult> AddMember(Guid teamId, [FromBody] AddTeamMemberRequestDto request)
        {
            var res = await _teamService.AddMemberAsync(teamId, request);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("{teamId:guid}/members")]
        public async Task<IActionResult> GetTeamMembers(Guid teamId)
        {
            var res = await _teamService.GetTeamMembersAsync(teamId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("{teamId:guid}/tasks")]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<IActionResult> CreateTask(Guid teamId, [FromBody] CreateTeamTaskRequestDto request)
        {
            var res = await _teamService.CreateTaskAsync(teamId, request);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("{teamId:guid}/tasks")]
        public async Task<IActionResult> GetTeamTasks(Guid teamId)
        {
            var res = await _teamService.GetTeamTasksAsync(teamId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPatch("tasks/{teamTaskId:guid}/status")]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<IActionResult> UpdateTaskStatus(Guid teamTaskId, [FromBody] UpdateTeamTaskStatusRequestDto request)
        {
            var res = await _teamService.UpdateTaskStatusAsync(teamTaskId, request);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("{teamId:guid}/updates")]
        public async Task<IActionResult> CreateUpdate(Guid teamId, [FromBody] CreateTeamUpdateRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value
                              ?? User.FindFirst("id")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized(new { message = "User id claim not found in token." });

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user id in token." });

            var res = await _teamService.CreateUpdateAsync(teamId, userId, request);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("{teamId:guid}/updates")]
        public async Task<IActionResult> GetTeamUpdates(Guid teamId)
        {
            var res = await _teamService.GetTeamUpdatesAsync(teamId);
            return StatusCode(res.StatusCode, res);
        }
    }
}
