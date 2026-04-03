using Microsoft.EntityFrameworkCore;
using TeamIndia.TalentFlow.Application.Interfaces;
using TeamIndia.TalentFlow.Domain.Entities;
using TeamIndia.TalentFlow.Infrastructure.DbContext;

namespace TeamIndia.TalentFlow.Infrastructure.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _db;
        public CourseRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Course> AddCourseAsync(Course course)
        {
            await _db.Courses.AddAsync(course);
            await _db.SaveChangesAsync();
            return course;
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _db.Courses.AsNoTracking().OrderBy(c => c.Title).ToListAsync();
        }

        public async Task<Course?> GetCourseWithDetailsAsync(Guid courseId)
        {
            return await _db.Courses.AsNoTracking()
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Lessons)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
        }

        public async Task<Module> AddModuleAsync(Module module)
        {
            await _db.Modules.AddAsync(module);
            await _db.SaveChangesAsync();
            return module;
        }

        public async Task<Module?> GetModuleByIdAsync(Guid moduleId)
        {
            return await _db.Modules.FirstOrDefaultAsync(m => m.ModuleId == moduleId);
        }

        public async Task UpdateModuleAsync(Module module)
        {
            _db.Modules.Update(module);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Module>> GetModulesByCourseIdAsync(Guid courseId)
        {
            return await _db.Modules.AsNoTracking().Where(m => m.CourseId == courseId).OrderBy(m => m.OrderIndex).ToListAsync();
        }

        public async Task<Lesson> AddLessonAsync(Lesson lesson)
        {
            await _db.Lessons.AddAsync(lesson);
            await _db.SaveChangesAsync();
            return lesson;
        }

        public async Task<Module> AddModuleToCourseAsync(Module module)
        {
            await _db.Modules.AddAsync(module);
            await _db.SaveChangesAsync();
            return module;
        }

        public async Task<Lesson> AddLessonToModuleAsync(Lesson lesson)
        {
            await _db.Lessons.AddAsync(lesson);
            await _db.SaveChangesAsync();
            return lesson;
        }

        public async Task<Lesson?> GetLessonByIdAsync(Guid lessonId)
        {
            return await _db.Lessons.FirstOrDefaultAsync(l => l.LessonId == lessonId);
        }

        public async Task UpdateLessonAsync(Lesson lesson)
        {
            _db.Lessons.Update(lesson);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateCourseAsync(Course course)
        {
            _db.Courses.Update(course);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteCourseAsync(Guid courseId)
        {
            var course = await _db.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
            if (course == null) return;

            // delete lessons and modules explicitly
            var modules = await _db.Modules.Where(m => m.CourseId == courseId).ToListAsync();
            var moduleIds = modules.Select(m => m.ModuleId).ToList();

            var lessons = await _db.Lessons.Where(l => l.CourseId == courseId || (l.ModuleId != null && moduleIds.Contains(l.ModuleId.Value))).ToListAsync();
            if (lessons.Any()) _db.Lessons.RemoveRange(lessons);

            if (modules.Any()) _db.Modules.RemoveRange(modules);

            _db.Courses.Remove(course);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteModuleAsync(Guid moduleId)
        {
            var module = await _db.Modules.FirstOrDefaultAsync(m => m.ModuleId == moduleId);
            if (module == null) return;

            var lessons = await _db.Lessons.Where(l => l.ModuleId == moduleId).ToListAsync();
            if (lessons.Any()) _db.Lessons.RemoveRange(lessons);

            _db.Modules.Remove(module);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteLessonAsync(Guid lessonId)
        {
            var lesson = await _db.Lessons.FirstOrDefaultAsync(l => l.LessonId == lessonId);
            if (lesson == null) return;
            _db.Lessons.Remove(lesson);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(Guid courseId)
        {
            return await _db.Lessons.AsNoTracking().Where(l => l.CourseId == courseId).ToListAsync();
        }
    }
}
