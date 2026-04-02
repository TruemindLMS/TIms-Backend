using TeamIndia.TalentFlow.Domain.Entities;

namespace TeamIndia.TalentFlow.Application.Interfaces
{
    public interface ITeamRepository
    {
        Task<Team> AddTeamAsync(Team team);
        Task<IEnumerable<Team>> GetAllTeamsAsync();
        Task<Team?> GetTeamByIdAsync(Guid teamId);
        Task<Team?> GetTeamWithDetailsByIdAsync(Guid teamId);
        Task<bool> TeamExistsAsync(Guid teamId);
        Task<bool> TeamNameExistsAsync(string teamName);

        Task<TeamMember> AddMemberAsync(TeamMember member);
        Task<IEnumerable<TeamMember>> GetMembersByTeamIdAsync(Guid teamId);
        Task<bool> IsUserAlreadyInTeamAsync(Guid teamId, Guid userId);

        Task<TeamTask> AddTaskAsync(TeamTask task);
        Task<TeamTask?> GetTaskByIdAsync(Guid teamTaskId);
        Task<IEnumerable<TeamTask>> GetTasksByTeamIdAsync(Guid teamId);
        Task UpdateTaskAsync(TeamTask task);

        Task<TeamUpdate> AddUpdateAsync(TeamUpdate update);
        Task<IEnumerable<TeamUpdate>> GetUpdatesByTeamIdAsync(Guid teamId);
    }
}
