using Microsoft.EntityFrameworkCore;
using TeamIndia.TalentFlow.Domain.Entities;
using TeamIndia.TalentFlow.Infrastructure.DbContext;
using TeamIndia.TalentFlow.Application.Interfaces;

namespace TeamIndia.TalentFlow.Infrastructure.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly ApplicationDbContext _context;

        public TeamRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Team> AddTeamAsync(Team team)
        {
            await _context.Teams.AddAsync(team);
            await _context.SaveChangesAsync();
            return team;
        }

        Task<Team> ITeamRepository.AddTeamAsync(Team team) => AddTeamAsync(team);

        public async Task<IEnumerable<Team>> GetAllTeamsAsync()
        {
            return await _context.Teams
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        Task<IEnumerable<Team>> ITeamRepository.GetAllTeamsAsync() => GetAllTeamsAsync();

        public async Task<Team?> GetTeamByIdAsync(Guid teamId)
        {
            return await _context.Teams
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TeamId == teamId);
        }

        Task<Team?> ITeamRepository.GetTeamByIdAsync(Guid teamId) => GetTeamByIdAsync(teamId);

        public async Task<Team?> GetTeamWithDetailsByIdAsync(Guid teamId)
        {
            return await _context.Teams
                .AsNoTracking()
                .Include(x => x.Members)
                    .ThenInclude(x => x.User)
                .Include(x => x.Tasks)
                    .ThenInclude(x => x.AssignedToUser)
                .Include(x => x.Updates)
                    .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.TeamId == teamId);
        }

        Task<Team?> ITeamRepository.GetTeamWithDetailsByIdAsync(Guid teamId) => GetTeamWithDetailsByIdAsync(teamId);

        public async Task<bool> TeamExistsAsync(Guid teamId)
        {
            return await _context.Teams.AnyAsync(x => x.TeamId == teamId);
        }

        Task<bool> ITeamRepository.TeamExistsAsync(Guid teamId) => TeamExistsAsync(teamId);

        public async Task<bool> TeamNameExistsAsync(string teamName)
        {
            return await _context.Teams.AnyAsync(x => x.Name.ToLower() == teamName.ToLower());
        }

        Task<bool> ITeamRepository.TeamNameExistsAsync(string teamName) => TeamNameExistsAsync(teamName);

        public async Task<TeamMember> AddMemberAsync(TeamMember member)
        {
            await _context.TeamMembers.AddAsync(member);
            await _context.SaveChangesAsync();
            return member;
        }

        Task<TeamMember> ITeamRepository.AddMemberAsync(TeamMember member) => AddMemberAsync(member);

        public async Task<IEnumerable<TeamMember>> GetMembersByTeamIdAsync(Guid teamId)
        {
            return await _context.TeamMembers
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.TeamId == teamId)
                .OrderBy(x => x.TeamRole)
                .ToListAsync();
        }

        Task<IEnumerable<TeamMember>> ITeamRepository.GetMembersByTeamIdAsync(Guid teamId) => GetMembersByTeamIdAsync(teamId);

        public async Task<bool> IsUserAlreadyInTeamAsync(Guid teamId, Guid userId)
        {
            return await _context.TeamMembers
                .AnyAsync(x => x.TeamId == teamId && x.UserId == userId);
        }

        Task<bool> ITeamRepository.IsUserAlreadyInTeamAsync(Guid teamId, Guid userId) => IsUserAlreadyInTeamAsync(teamId, userId);

        public async Task<TeamTask> AddTaskAsync(TeamTask task)
        {
            await _context.TeamTasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return task;
        }

        Task<TeamTask> ITeamRepository.AddTaskAsync(TeamTask task) => AddTaskAsync(task);

        public async Task<TeamTask?> GetTaskByIdAsync(Guid teamTaskId)
        {
            return await _context.TeamTasks
                .Include(x => x.AssignedToUser)
                .FirstOrDefaultAsync(x => x.TeamTaskId == teamTaskId);
        }

        Task<TeamTask?> ITeamRepository.GetTaskByIdAsync(Guid teamTaskId) => GetTaskByIdAsync(teamTaskId);

        public async Task<IEnumerable<TeamTask>> GetTasksByTeamIdAsync(Guid teamId)
        {
            return await _context.TeamTasks
                .AsNoTracking()
                .Include(x => x.AssignedToUser)
                .Where(x => x.TeamId == teamId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        Task<IEnumerable<TeamTask>> ITeamRepository.GetTasksByTeamIdAsync(Guid teamId) => GetTasksByTeamIdAsync(teamId);

        public async Task UpdateTaskAsync(TeamTask task)
        {
            _context.TeamTasks.Update(task);
            await _context.SaveChangesAsync();
        }

        Task ITeamRepository.UpdateTaskAsync(TeamTask task)
        {
            return Task.Run(async () => await UpdateTaskAsync(task));
        }

        public async Task<TeamUpdate> AddUpdateAsync(TeamUpdate update)
        {
            await _context.TeamUpdates.AddAsync(update);
            await _context.SaveChangesAsync();
            return update;
        }

        Task<TeamUpdate> ITeamRepository.AddUpdateAsync(TeamUpdate update) => AddUpdateAsync(update);

        public async Task<IEnumerable<TeamUpdate>> GetUpdatesByTeamIdAsync(Guid teamId)
        {
            return await _context.TeamUpdates
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.TeamId == teamId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        Task<IEnumerable<TeamUpdate>> ITeamRepository.GetUpdatesByTeamIdAsync(Guid teamId) => GetUpdatesByTeamIdAsync(teamId);
    }
}
