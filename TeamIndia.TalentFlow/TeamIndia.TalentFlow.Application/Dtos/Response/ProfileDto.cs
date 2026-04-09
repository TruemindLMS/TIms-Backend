using TeamIndia.TalentFlow.Domain.Enums;

namespace TeamIndia.TalentFlow.Application.Dtos.Response;

public class ProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Address { get; set; }
    public string? PostalCode { get; set; }
    public string? Location { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
