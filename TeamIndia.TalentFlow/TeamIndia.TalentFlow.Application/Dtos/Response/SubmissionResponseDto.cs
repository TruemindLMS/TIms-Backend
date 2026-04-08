namespace TeamIndia.TalentFlow.Application.Dtos.Response
{
    public class SubmissionResponseDto
    {
        public Guid SubmissionId { get; set; }
        public Guid AssignmentId { get; set; }
        public Guid UserId { get; set; }
        public string? TextResponse { get; set; }
        public string? FileUrl { get; set; }
        public string? LinkUrl { get; set; }
        public DateTime SubmittedOnUtc { get; set; }
    }
}
