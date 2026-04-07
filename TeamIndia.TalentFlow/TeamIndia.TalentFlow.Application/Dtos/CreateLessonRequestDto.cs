namespace TeamIndia.TalentFlow.Application.Dtos
{
    public class CreateLessonRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? VideoUrl { get; set; }
    }
}
