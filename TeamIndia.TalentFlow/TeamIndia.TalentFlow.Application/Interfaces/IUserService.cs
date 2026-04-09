using TeamIndia.TalentFlow.Application.Dtos.Response;

namespace TeamIndia.TalentFlow.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserFullResponseDto>> GetAllUsersAsync();
    Task<UserFullResponseDto?> GetUserByIdAsync(Guid userId);
}
