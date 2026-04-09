using Microsoft.AspNetCore.Http;
using TeamIndia.TalentFlow.Domain.Enums;

namespace TeamIndia.TalentFlow.Application.Dtos.Request
{
    public class CreateOnboardingRequestDto
    {
        public string? Bio { get; set; }
        public IFormFile? ProfilePictureFile { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public Discipline? Discipline { get; set; }
        public Goal? Goal { get; set; }
    }
}
