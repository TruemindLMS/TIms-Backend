namespace TeamIndia.TalentFlow.Application.Dtos.Response
{
    public class CertificateResponseDto
    {
        public Guid CertificateId { get; set; }
        public Guid CourseId { get; set; }
        public Guid UserId { get; set; }
        public DateTime IssuedOnUtc { get; set; }
        public string? FileUrl { get; set; }
        public string HtmlContent { get; set; } = string.Empty;
    }
}
