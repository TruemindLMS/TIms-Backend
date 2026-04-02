using Microsoft.AspNetCore.Identity;
using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Dtos;
using TeamIndia.TalentFlow.Application.Dtos.Response;
using TeamIndia.TalentFlow.Application.Interfaces;
using TeamIndia.TalentFlow.Domain.Entities;

namespace TeamIndia.TalentFlow.Application.Services
{
    public class TeamServices : ITeamServices
    {
        private readonly ITeamRepository _teamRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeamServices(
            ITeamRepository teamRepository,
            UserManager<ApplicationUser> userManager)
        {
            _teamRepository = teamRepository;
            _userManager = userManager;
        }

        public async Task<BaseResponse<IEnumerable<TeamMemberResponseDto>>> GetTeamMembersAsync(Guid teamId)
        {
            try
            {
                var teamExists = await _teamRepository.TeamExistsAsync(teamId);
                if (!teamExists)
                    return BaseResponse<IEnumerable<TeamMemberResponseDto>>.Fail("Team not found.", null, 404);

                var members = await _teamRepository.GetMembersByTeamIdAsync(teamId);
                var dtos = members.Select(MapMemberToDto);
                return BaseResponse<IEnumerable<TeamMemberResponseDto>>.Ok(dtos, "OK", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse<IEnumerable<TeamMemberResponseDto>>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        public async Task<BaseResponse<IEnumerable<TeamTaskResponseDto>>> GetTeamTasksAsync(Guid teamId)
        {
            try
            {
                var teamExists = await _teamRepository.TeamExistsAsync(teamId);
                if (!teamExists)
                    return BaseResponse<IEnumerable<TeamTaskResponseDto>>.Fail("Team not found.", null, 404);

                var tasks = await _teamRepository.GetTasksByTeamIdAsync(teamId);
                var dtos = tasks.Select(MapTaskToDto);
                return BaseResponse<IEnumerable<TeamTaskResponseDto>>.Ok(dtos, "OK", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse<IEnumerable<TeamTaskResponseDto>>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        public async Task<BaseResponse<IEnumerable<TeamUpdateResponseDto>>> GetTeamUpdatesAsync(Guid teamId)
        {
            try
            {
                var teamExists = await _teamRepository.TeamExistsAsync(teamId);
                if (!teamExists)
                    return BaseResponse<IEnumerable<TeamUpdateResponseDto>>.Fail("Team not found.", null, 404);

                var updates = await _teamRepository.GetUpdatesByTeamIdAsync(teamId);
                var dtos = updates.Select(MapUpdateToDto);
                return BaseResponse<IEnumerable<TeamUpdateResponseDto>>.Ok(dtos, "OK", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse<IEnumerable<TeamUpdateResponseDto>>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        public async Task<BaseResponse<TeamResponseDto>> CreateTeamAsync(CreateTeamRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return TeamIndia.TalentFlow.Application.Common.BaseResponse<TeamResponseDto>.Fail("Team name is required.", null, 400);

                var teamNameExists = await _teamRepository.TeamNameExistsAsync(request.Name.Trim());
                if (teamNameExists)
                    return BaseResponse<TeamResponseDto>.Fail("A team with this name already exists.", null, 409);

                var team = new Team
                {
                    TeamId = Guid.NewGuid(),
                    Name = request.Name.Trim(),
                    Description = request.Description?.Trim(),
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow
                };

                var createdTeam = await _teamRepository.AddTeamAsync(team);

                var dto = new TeamResponseDto
                {
                    TeamId = createdTeam.TeamId,
                    Name = createdTeam.Name,
                    Description = createdTeam.Description,
                    CreatedAtUtc = createdTeam.CreatedAtUtc,
                    UpdatedAtUtc = createdTeam.UpdatedAtUtc
                };

                return BaseResponse<TeamResponseDto>.Ok(dto, "Team created", 201);
            }
            catch (Exception ex)
            {
                return BaseResponse<TeamResponseDto>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        public async Task<BaseResponse<IEnumerable<TeamResponseDto>>> GetAllTeamsAsync()
        {
            try
            {
                var teams = await _teamRepository.GetAllTeamsAsync();
                var dtos = teams.Select(team => new TeamResponseDto
                {
                    TeamId = team.TeamId,
                    Name = team.Name,
                    Description = team.Description,
                    CreatedAtUtc = team.CreatedAtUtc,
                    UpdatedAtUtc = team.UpdatedAtUtc
                });

                return BaseResponse<IEnumerable<TeamResponseDto>>.Ok(dtos, "OK", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse<IEnumerable<TeamResponseDto>>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        public async Task<BaseResponse<TeamDetailsResponseDto?>> GetTeamByIdAsync(Guid teamId)
        {
            try
            {
                var team = await _teamRepository.GetTeamWithDetailsByIdAsync(teamId);
                if (team == null)
                    return BaseResponse<TeamDetailsResponseDto?>.Fail("Team not found.", null, 404);

                var dto = new TeamDetailsResponseDto
                {
                    TeamId = team.TeamId,
                    Name = team.Name,
                    Description = team.Description,
                    CreatedAtUtc = team.CreatedAtUtc,
                    UpdatedAtUtc = team.UpdatedAtUtc,
                    Members = team.Members.Select(MapMemberToDto).ToList(),
                    Tasks = team.Tasks.Select(MapTaskToDto).ToList(),
                    Updates = team.Updates
                        .OrderByDescending(x => x.CreatedAtUtc)
                        .Select(MapUpdateToDto)
                        .ToList()
                };

                return BaseResponse<TeamDetailsResponseDto?>.Ok(dto, "OK", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse<TeamDetailsResponseDto?>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        public async Task<BaseResponse<TeamMemberResponseDto>> AddMemberAsync(Guid teamId, AddTeamMemberRequestDto request)
        {
            try
            {
                var teamExists = await _teamRepository.TeamExistsAsync(teamId);
                if (!teamExists)
                    return BaseResponse<TeamMemberResponseDto>.Fail("Team not found.", null, 404);

                var user = await _userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                    return BaseResponse<TeamMemberResponseDto>.Fail("User not found.", null, 404);

                var alreadyExists = await _teamRepository.IsUserAlreadyInTeamAsync(teamId, request.UserId);
                if (alreadyExists)
                    return BaseResponse<TeamMemberResponseDto>.Fail("User is already a member of this team.", null, 409);

                var member = new TeamMember
                {
                    TeamMemberId = Guid.NewGuid(),
                    TeamId = teamId,
                    UserId = request.UserId,
                    TeamRole = request.TeamRole.Trim(),
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow
                };

                var createdMember = await _teamRepository.AddMemberAsync(member);

                var dto = new TeamMemberResponseDto
                {
                    TeamMemberId = createdMember.TeamMemberId,
                    TeamId = createdMember.TeamId,
                    UserId = createdMember.UserId,
                    TeamRole = createdMember.TeamRole,
                    FullName = $"{user.FullName}".Trim(),
                    Email = user.Email,
                    CreatedAtUtc = createdMember.CreatedAtUtc
                };

                return BaseResponse<TeamMemberResponseDto>.Ok(dto, "Member added", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse<TeamMemberResponseDto>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }
        public async Task<BaseResponse<TeamTaskResponseDto>> CreateTaskAsync(Guid teamId, CreateTeamTaskRequestDto request)
        {
            try
            {
                var teamExists = await _teamRepository.TeamExistsAsync(teamId);
                if (!teamExists)
                    return BaseResponse<TeamTaskResponseDto>.Fail("Team not found.", null, 404);

                if (string.IsNullOrWhiteSpace(request.Title))
                    return BaseResponse<TeamTaskResponseDto>.Fail("Task title is required.", null, 400);

                if (request.AssignedToUserId.HasValue)
                {
                    var isMember = await _teamRepository.IsUserAlreadyInTeamAsync(teamId, request.AssignedToUserId.Value);
                    if (!isMember)
                        return BaseResponse<TeamTaskResponseDto>.Fail("Assigned user is not a member of this team.", null, 409);
                }

                var task = new TeamTask
                {
                    TeamTaskId = Guid.NewGuid(),
                    TeamId = teamId,
                    Title = request.Title.Trim(),
                    Description = request.Description?.Trim(),
                    AssignedToUserId = request.AssignedToUserId,
                    DueDateUtc = request.DueDateUtc,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow
                };

                var createdTask = await _teamRepository.AddTaskAsync(task);
                var fullTask = await _teamRepository.GetTaskByIdAsync(createdTask.TeamTaskId);
                return BaseResponse<TeamTaskResponseDto>.Ok(MapTaskToDto(fullTask!), "Task created", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse<TeamTaskResponseDto>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        public async Task<BaseResponse<TeamTaskResponseDto>> UpdateTaskStatusAsync(Guid teamTaskId, UpdateTeamTaskStatusRequestDto request)
        {
            try
            {
                var task = await _teamRepository.GetTaskByIdAsync(teamTaskId);
                if (task == null)
                    return BaseResponse<TeamTaskResponseDto>.Fail("Task not found.", null, 404);

                task.Status = request.Status;
                task.UpdatedAtUtc = DateTime.UtcNow;

                await _teamRepository.UpdateTaskAsync(task);

                var updatedTask = await _teamRepository.GetTaskByIdAsync(teamTaskId);
                return BaseResponse<TeamTaskResponseDto>.Ok(MapTaskToDto(updatedTask!), "Task updated", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse<TeamTaskResponseDto>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        public async Task<BaseResponse<TeamUpdateResponseDto>> CreateUpdateAsync(Guid teamId, Guid userId, CreateTeamUpdateRequestDto request)
        {
            try
            {
                var teamExists = await _teamRepository.TeamExistsAsync(teamId);
                if (!teamExists)
                    return BaseResponse<TeamUpdateResponseDto>.Fail("Team not found.", null, 404);

                var isMember = await _teamRepository.IsUserAlreadyInTeamAsync(teamId, userId);
                if (!isMember)
                    return BaseResponse<TeamUpdateResponseDto>.Fail("Only team members can post team updates.", null, 403);

                if (string.IsNullOrWhiteSpace(request.Message))
                    return BaseResponse<TeamUpdateResponseDto>.Fail("Update message is required.", null, 400);

                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return BaseResponse<TeamUpdateResponseDto>.Fail("User not found.", null, 404);

                var update = new TeamUpdate
                {
                    TeamUpdateId = Guid.NewGuid(),
                    TeamId = teamId,
                    UserId = userId,
                    Message = request.Message.Trim(),
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow
                };

                var createdUpdate = await _teamRepository.AddUpdateAsync(update);

                var dto = new TeamUpdateResponseDto
                {
                    TeamUpdateId = createdUpdate.TeamUpdateId,
                    TeamId = createdUpdate.TeamId,
                    UserId = createdUpdate.UserId,
                    UserFullName = $"{user.FullName}".Trim(),
                    Message = createdUpdate.Message,
                    CreatedAtUtc = createdUpdate.CreatedAtUtc,
                    UpdatedAtUtc = createdUpdate.UpdatedAtUtc
                };

                return BaseResponse<TeamUpdateResponseDto>.Ok(dto, "Update created", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse<TeamUpdateResponseDto>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }



        private static TeamMemberResponseDto MapMemberToDto(TeamMember member)
        {
            return new TeamMemberResponseDto
            {
                TeamMemberId = member.TeamMemberId,
                TeamId = member.TeamId,
                UserId = member.UserId,
                TeamRole = member.TeamRole,
                FullName = member.User != null
                    ? $"{member.User.FullName}".Trim()
                    : null,
                Email = member.User?.Email,
                CreatedAtUtc = member.CreatedAtUtc
            };
        }

        private static TeamTaskResponseDto MapTaskToDto(TeamTask task)
        {
            return new TeamTaskResponseDto
            {
                TeamTaskId = task.TeamTaskId,
                TeamId = task.TeamId,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUserName = task.AssignedToUser != null
                    ? $"{task.AssignedToUser.FullName}".Trim()
                    : null,
                DueDateUtc = task.DueDateUtc,
                CreatedAtUtc = task.CreatedAtUtc,
                UpdatedAtUtc = task.UpdatedAtUtc
            };
        }

        private static TeamUpdateResponseDto MapUpdateToDto(TeamUpdate update)
        {
            return new TeamUpdateResponseDto
            {
                TeamUpdateId = update.TeamUpdateId,
                TeamId = update.TeamId,
                UserId = update.UserId,
                UserFullName = update.User != null
                    ? $"{update.User.FullName}".Trim()
                    : null,
                Message = update.Message,
                CreatedAtUtc = update.CreatedAtUtc,
                UpdatedAtUtc = update.UpdatedAtUtc
            };
        }
    }
}
