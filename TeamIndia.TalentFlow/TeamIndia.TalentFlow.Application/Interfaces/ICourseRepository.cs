using TeamIndia.TalentFlow.Domain.Entities;

namespace TeamIndia.TalentFlow.Application.Interfaces
{
    public interface ICourseRepository
    {
        Task<Course> AddCourseAsync(Course course);
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseWithDetailsAsync(Guid courseId);
        Task UpdateCourseAsync(Course course);


        Task<Module> AddModuleAsync(Module module);
        Task<IEnumerable<Module>> GetModulesByCourseIdAsync(Guid courseId);
        Task<Module?> GetModuleByIdAsync(Guid moduleId);
        Task UpdateModuleAsync(Module module);

        Task<Lesson> AddLessonAsync(Lesson lesson);
        Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(Guid courseId);
        Task<Lesson?> GetLessonByIdAsync(Guid lessonId);
        Task UpdateLessonAsync(Lesson lesson);
        Task<bool> IsUserEnrolledAsync(Guid courseId, Guid userId);
        Task<Module> AddModuleToCourseAsync(Module module);
        Task<Lesson> AddLessonToModuleAsync(Lesson lesson);
        Task DeleteCourseAsync(Guid courseId);
        Task DeleteModuleAsync(Guid moduleId);
        Task DeleteLessonAsync(Guid lessonId);
    }
}
