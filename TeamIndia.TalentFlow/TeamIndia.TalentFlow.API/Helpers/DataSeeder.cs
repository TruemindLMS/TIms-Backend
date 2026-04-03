using Microsoft.EntityFrameworkCore;
using TeamIndia.TalentFlow.Domain.Entities;
using TeamIndia.TalentFlow.Infrastructure.DbContext;

namespace TeamIndia.TalentFlow.API.Helpers
{
    public static class DataSeeder
    {
        public static async Task SeedCoursesAsync(ApplicationDbContext db)
        {
            if (db == null) return;

            if (await db.Courses.AnyAsync()) return;

            var courses = new List<Course>();
            var lessons = new List<Lesson>();

            // UI/UX Design
            {
                var course = new Course { CourseId = Guid.NewGuid(), Title = "Introduction to UI/UX Design", Description = "Introductory UI/UX design course" };
                var m1 = new Module { ModuleId = Guid.NewGuid(), CourseId = course.CourseId, Title = "Foundations of UX", OrderIndex = 1 };
                var l1 = new Lesson { LessonId = Guid.NewGuid(), CourseId = course.CourseId, ModuleId = m1.ModuleId, Title = "What is UX Design and Why It Matters", Content = string.Empty };
                var m2 = new Module { ModuleId = Guid.NewGuid(), CourseId = course.CourseId, Title = "User Research and Personas", OrderIndex = 2 };
                var l2 = new Lesson { LessonId = Guid.NewGuid(), CourseId = course.CourseId, ModuleId = m2.ModuleId, Title = "How to Create User Personas", Content = string.Empty };
                var m3 = new Module { ModuleId = Guid.NewGuid(), CourseId = course.CourseId, Title = "Wireframing and Prototyping", OrderIndex = 3 };
                var l3 = new Lesson { LessonId = Guid.NewGuid(), CourseId = course.CourseId, ModuleId = m3.ModuleId, Title = "Designing Low-Fidelity Wireframes", Content = string.Empty };
                course.Modules.Add(m1); course.Modules.Add(m2); course.Modules.Add(m3);
                courses.Add(course); lessons.AddRange(new[] { l1, l2, l3 });
            }

            // Project Management
            {
                var course = new Course { CourseId = Guid.NewGuid(), Title = "Fundamentals of Project Management", Description = "Core project management skills" };
                var m1 = new Module { ModuleId = Guid.NewGuid(), CourseId = course.CourseId, Title = "Introduction to Project Management", OrderIndex = 1 };
                var l1 = new Lesson { LessonId = Guid.NewGuid(), CourseId = course.CourseId, ModuleId = m1.ModuleId, Title = "Understanding the Project Lifecycle", Content = string.Empty };
                var m2 = new Module { ModuleId = Guid.NewGuid(), CourseId = course.CourseId, Title = "Planning and Scheduling", OrderIndex = 2 };
                var l2 = new Lesson { LessonId = Guid.NewGuid(), CourseId = course.CourseId, ModuleId = m2.ModuleId, Title = "How to Create a Simple Project Plan", Content = string.Empty };
                var m3 = new Module { ModuleId = Guid.NewGuid(), CourseId = course.CourseId, Title = "Team Collaboration and Risk Management", OrderIndex = 3 };
                var l3 = new Lesson { LessonId = Guid.NewGuid(), CourseId = course.CourseId, ModuleId = m3.ModuleId, Title = "Identifying and Managing Project Risks", Content = string.Empty };
                course.Modules.Add(m1); course.Modules.Add(m2); course.Modules.Add(m3);
                courses.Add(course); lessons.AddRange(new[] { l1, l2, l3 });
            }

            // Graphics Design
            {
                var course = new Course { CourseId = Guid.NewGuid(), Title = "Basics of Graphic Design", Description = "Fundamentals of visual design" };
                var m1 = new Module { ModuleId = Guid.NewGuid(), CourseId = course.CourseId, Title = "Principles of Design", OrderIndex = 1 };
                var l1 = new Lesson { LessonId = Guid.NewGuid(), CourseId = course.CourseId, ModuleId = m1.ModuleId, Title = "Understanding Contrast, Alignment, and Balance", Content = string.Empty };
                var m2 = new Module { ModuleId = Guid.NewGuid(), CourseId = course.CourseId, Title = "Typography and Color", OrderIndex = 2 };
                var l2 = new Lesson { LessonId = Guid.NewGuid(), CourseId = course.CourseId, ModuleId = m2.ModuleId, Title = "Choosing the Right Fonts and Color Combinations", Content = string.Empty };
                var m3 = new Module { ModuleId = Guid.NewGuid(), CourseId = course.CourseId, Title = "Designing Social Media Creatives", OrderIndex = 3 };
                var l3 = new Lesson { LessonId = Guid.NewGuid(), CourseId = course.CourseId, ModuleId = m3.ModuleId, Title = "Creating an Engaging Social Media Poster", Content = string.Empty };
                course.Modules.Add(m1); course.Modules.Add(m2); course.Modules.Add(m3);
                courses.Add(course); lessons.AddRange(new[] { l1, l2, l3 });
            }

            // Frontend Development
            {
                var course = new Course { CourseId = Guid.NewGuid(), Title = "Frontend Web Development Essentials", Description = "Fundamentals of building responsive, accessible web interfaces" };
                var m1 = new Module { ModuleId = Guid.NewGuid(), CourseId = course.CourseId, Title = "HTML Fundamentals", OrderIndex = 1 };
                var l1 = new Lesson { LessonId = Guid.NewGuid(), CourseId = course.CourseId, ModuleId = m1.ModuleId, Title = "Building the Structure of a Web Page", Content = string.Empty };
                var m2 = new Module { ModuleId = Guid.NewGuid(), CourseId = course.CourseId, Title = "CSS Styling Basics", OrderIndex = 2 };
                var l2 = new Lesson { LessonId = Guid.NewGuid(), CourseId = course.CourseId, ModuleId = m2.ModuleId, Title = "Styling Pages with CSS", Content = string.Empty };
                var m3 = new Module { ModuleId = Guid.NewGuid(), CourseId = course.CourseId, Title = "JavaScript Introduction", OrderIndex = 3 };
                var l3 = new Lesson { LessonId = Guid.NewGuid(), CourseId = course.CourseId, ModuleId = m3.ModuleId, Title = "Adding Interactivity with JavaScript", Content = string.Empty };
                course.Modules.Add(m1); course.Modules.Add(m2); course.Modules.Add(m3);
                courses.Add(course); lessons.AddRange(new[] { l1, l2, l3 });
            }

            // Backend Development
            {
                var course = new Course { CourseId = Guid.NewGuid(), Title = "Backend Development with ASP.NET Core", Description = "Server-side development with ASP.NET Core" };
                var m1 = new Module { ModuleId = Guid.NewGuid(), CourseId = course.CourseId, Title = "Introduction to Backend Development", OrderIndex = 1 };
                var l1 = new Lesson { LessonId = Guid.NewGuid(), CourseId = course.CourseId, ModuleId = m1.ModuleId, Title = "What Happens Behind the Scenes of a Web App", Content = string.Empty };
                var m2 = new Module { ModuleId = Guid.NewGuid(), CourseId = course.CourseId, Title = "Building APIs with ASP.NET Core", OrderIndex = 2 };
                var l2 = new Lesson { LessonId = Guid.NewGuid(), CourseId = course.CourseId, ModuleId = m2.ModuleId, Title = "Creating Your First REST API Endpoint", Content = string.Empty };
                var m3 = new Module { ModuleId = Guid.NewGuid(), CourseId = course.CourseId, Title = "Database Integration", OrderIndex = 3 };
                var l3 = new Lesson { LessonId = Guid.NewGuid(), CourseId = course.CourseId, ModuleId = m3.ModuleId, Title = "Connecting ASP.NET Core to a Database", Content = string.Empty };
                course.Modules.Add(m1); course.Modules.Add(m2); course.Modules.Add(m3);
                courses.Add(course); lessons.AddRange(new[] { l1, l2, l3 });
            }

            await db.Courses.AddRangeAsync(courses);
            await db.Modules.AddRangeAsync(courses.SelectMany(c => c.Modules));
            await db.Lessons.AddRangeAsync(lessons);
            await db.SaveChangesAsync();
        }
    }
}
