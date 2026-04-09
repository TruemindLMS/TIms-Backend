using TeamIndia.TalentFlow.Application.Interfaces;
using TeamIndia.TalentFlow.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace TeamIndia.TalentFlow.Infrastructure.Repositories

{
    public class CertificateService : ICertificateService
    {
        private readonly ApplicationDbContext _context;

        public CertificateService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsCourseCompletedAsync(Guid userId, Guid courseId)
        {
            var totalLessons = await _context.Lessons
                .CountAsync(l => l.Module.CourseId == courseId);

            var completedLessons = await _context.LessonCompletions
                .CountAsync(lc => lc.UserId == userId && lc.Lesson.Module.CourseId == courseId);

            return totalLessons > 0 && totalLessons == completedLessons;
        }

        public async Task<byte[]> GenerateCertificatePdfAsync(Guid userId, Guid courseId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            var course = await _context.Courses.FindAsync(courseId);

            // Simple HTML Template
            string htmlContent = $@"
                <html>
                    <body style='text-align:center; border: 10px solid #787878; padding: 50px;'>
                        <h1>Certificate of Completion</h1>
                        <p>This is to certify that</p>
                        <h2>{user?.FullName}</h2>
                        <p>has successfully completed the course</p>
                        <h3>{course?.Title}</h3>
                        <p>Issued on: {DateTime.Now:MMMM dd, yyyy}</p>
                    </body>
                </html>";

            // In a real scenario, use a library like DinkToPdf here to convert HTML to Byte[]
            // For now, we return a dummy byte array representing the PDF
            return System.Text.Encoding.UTF8.GetBytes(htmlContent);
        }
    }
}
