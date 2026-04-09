using Microsoft.AspNetCore.Http;
using TeamIndia.TalentFlow.Domain.Enums;

namespace TeamIndia.TalentFlow.Application.Dtos;

public class UpdateProfileRequest
{
    public string? Address { get; set; }
    public string? PostalCode { get; set; }
    public string? Location { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public string? PhotoUrl { get; set; }
    public IFormFile? PhotoFile { get; set; }
}
