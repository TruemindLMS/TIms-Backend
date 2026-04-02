using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Dtos;
using TeamIndia.TalentFlow.Application.Dtos.Response;

namespace TeamIndia.TalentFlow.Application.Interfaces
{
    public interface ITeamServices
    {
        Task<BaseResponse<TeamResponseDto>> CreateTeamAsync(CreateTeamRequestDto request);
        Task<BaseResponse<IEnumerable<TeamResponseDto>>> GetAllTeamsAsync();
        Task<BaseResponse<TeamDetailsResponseDto?>> GetTeamByIdAsync(Guid teamId);

        Task<BaseResponse<TeamMemberResponseDto>> AddMemberAsync(Guid teamId, AddTeamMemberRequestDto request);
        Task<BaseResponse<IEnumerable<TeamMemberResponseDto>>> GetTeamMembersAsync(Guid teamId);

        Task<BaseResponse<TeamTaskResponseDto>> CreateTaskAsync(Guid teamId, CreateTeamTaskRequestDto request);
        Task<BaseResponse<IEnumerable<TeamTaskResponseDto>>> GetTeamTasksAsync(Guid teamId);
        Task<BaseResponse<TeamTaskResponseDto>> UpdateTaskStatusAsync(Guid teamTaskId, UpdateTeamTaskStatusRequestDto request);

        Task<BaseResponse<TeamUpdateResponseDto>> CreateUpdateAsync(Guid teamId, Guid userId, CreateTeamUpdateRequestDto request);
        Task<BaseResponse<IEnumerable<TeamUpdateResponseDto>>> GetTeamUpdatesAsync(Guid teamId);

    }
}
