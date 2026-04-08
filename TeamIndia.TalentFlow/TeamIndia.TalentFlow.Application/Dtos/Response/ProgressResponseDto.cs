namespace TeamIndia.TalentFlow.Application.Dtos.Response
{
    public class ProgressResponseDto
    {
        public Guid CourseId { get; set; }
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
        public int TotalAssignments { get; set; }
        public int SubmittedAssignments { get; set; }
        // Return percentage rounded to two decimal places for consistent frontend display
        public double CompletionPercentage => TotalLessons == 0 ? 0 : Math.Round(((double)CompletedLessons / TotalLessons) * 100.0, 2);
    }
}
