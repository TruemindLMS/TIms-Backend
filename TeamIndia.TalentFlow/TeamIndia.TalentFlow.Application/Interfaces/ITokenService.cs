using TeamIndia.TalentFlow.Domain.Entities;
using System.Collections.Generic;

namespace TeamIndia.TalentFlow.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(ApplicationUser user, IEnumerable<string> roles);
}
