using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Dtos;
using TeamIndia.TalentFlow.Application.Dtos.Response;
using TeamIndia.TalentFlow.Application.Interfaces;
using TeamIndia.TalentFlow.Domain.Entities;

namespace TeamIndia.TalentFlow.Application.Services
{
    public class CourseServices : ICourseServices
    {
        private readonly ICourseRepository _repo;
        public CourseServices(ICourseRepository repo)
        {
            _repo = repo;
        }

        public async Task<BaseResponse<ModuleResponseDto>> CreateModuleAsync(Guid courseId, CreateModuleRequestDto request)
        {
            try
            {
                var course = await _repo.GetCourseWithDetailsAsync(courseId);
                if (course == null) return BaseResponse<ModuleResponseDto>.Fail("Course not found", null, 404);

                var module = new Module { ModuleId = Guid.NewGuid(), CourseId = courseId, Title = request.Title.Trim(), OrderIndex = request.OrderIndex };
                await _repo.AddModuleToCourseAsync(module);

                // create any lessons included
                var lessons = new List<Lesson>();
                if (request.Lessons != null)
                {
                    foreach (var l in request.Lessons)
                    {
                        var lesson = new Lesson { LessonId = Guid.NewGuid(), CourseId = courseId, ModuleId = module.ModuleId, Title = l.Title.Trim(), Content = l.Content };
                        lessons.Add(lesson);
                        await _repo.AddLessonToModuleAsync(lesson);
                    }
                }

                var dto = new ModuleResponseDto { ModuleId = module.ModuleId, Title = module.Title, OrderIndex = module.OrderIndex, Lessons = lessons.Select(ll => new LessonResponseDto { LessonId = ll.LessonId, Title = ll.Title, Content = ll.Content, ModuleId = ll.ModuleId }).ToList() };
                return BaseResponse<ModuleResponseDto>.Ok(dto, "Module created", 201);
            }
            catch (Exception ex)
            {
                return BaseResponse<ModuleResponseDto>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        public async Task<BaseResponse<LessonResponseDto>> CreateLessonAsync(Guid courseId, Guid moduleId, CreateLessonRequestDto request)
        {
            try
            {
                var module = await _repo.GetModuleByIdAsync(moduleId);
                if (module == null || module.CourseId != courseId) return BaseResponse<LessonResponseDto>.Fail("Module not found for course", null, 404);

                var lesson = new Lesson { LessonId = Guid.NewGuid(), CourseId = courseId, ModuleId = moduleId, Title = request.Title.Trim(), Content = request.Content };
                await _repo.AddLessonToModuleAsync(lesson);

                var dto = new LessonResponseDto { LessonId = lesson.LessonId, Title = lesson.Title, Content = lesson.Content, ModuleId = lesson.ModuleId };
                return BaseResponse<LessonResponseDto>.Ok(dto, "Lesson created", 201);
            }
            catch (Exception ex)
            {
                return BaseResponse<LessonResponseDto>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }
        public async Task<BaseResponse<CourseResponseDto>> CreateCourseAsync(CreateCourseRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Title))
                    return BaseResponse<CourseResponseDto>.Fail("Title is required", null, 400);

                var course = new Course { CourseId = Guid.NewGuid(), Title = request.Title.Trim(), Description = request.Description };
                var modules = new List<Module>();
                var lessons = new List<Lesson>();

                if (request.Modules != null)
                {
                    foreach (var m in request.Modules)
                    {
                        var module = new Module { ModuleId = Guid.NewGuid(), CourseId = course.CourseId, Title = m.Title.Trim(), OrderIndex = m.OrderIndex };
                        modules.Add(module);
                        course.Modules.Add(module);

                        if (m.Lessons != null)
                        {
                            foreach (var l in m.Lessons)
                            {
                                var lesson = new Lesson { LessonId = Guid.NewGuid(), CourseId = course.CourseId, ModuleId = module.ModuleId, Title = l.Title.Trim(), Content = l.Content };
                                lessons.Add(lesson);
                            }
                        }
                    }
                }

                await _repo.AddCourseAsync(course);
                foreach (var mod in modules) await _repo.AddModuleAsync(mod);
                foreach (var les in lessons) await _repo.AddLessonAsync(les);

                var dto = new CourseResponseDto { CourseId = course.CourseId, Title = course.Title, Description = course.Description };
                dto.Modules = modules.Select(m => new ModuleResponseDto { ModuleId = m.ModuleId, Title = m.Title, OrderIndex = m.OrderIndex, Lessons = lessons.Where(l => l.ModuleId == m.ModuleId).Select(ll => new LessonResponseDto { LessonId = ll.LessonId, Title = ll.Title, Content = ll.Content, ModuleId = ll.ModuleId }).ToList() }).ToList();

                return BaseResponse<CourseResponseDto>.Ok(dto, "Course created", 201);
            }
            catch (Exception ex)
            {
                return BaseResponse<CourseResponseDto>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        public async Task<BaseResponse<IEnumerable<CourseResponseDto>>> GetAllCoursesAsync()
        {
            try
            {
                var courses = await _repo.GetAllCoursesAsync();
                var dtos = courses.Select(c => new CourseResponseDto { CourseId = c.CourseId, Title = c.Title, Description = c.Description }).ToList();
                return BaseResponse<IEnumerable<CourseResponseDto>>.Ok(dtos, "OK", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse<IEnumerable<CourseResponseDto>>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        public async Task<BaseResponse<CourseResponseDto?>> GetCourseByIdAsync(Guid courseId)
        {
            try
            {
                var course = await _repo.GetCourseWithDetailsAsync(courseId);
                if (course == null) return BaseResponse<CourseResponseDto?>.Fail("Course not found", null, 404);

                var dto = new CourseResponseDto
                {
                    CourseId = course.CourseId,
                    Title = course.Title,
                    Description = course.Description,
                    Modules = course.Modules.Select(m => new ModuleResponseDto { ModuleId = m.ModuleId, Title = m.Title, OrderIndex = m.OrderIndex, Lessons = m.Lessons.Select(l => new LessonResponseDto { LessonId = l.LessonId, Title = l.Title, Content = l.Content, ModuleId = l.ModuleId }).ToList() }).ToList()
                };

                return BaseResponse<CourseResponseDto?>.Ok(dto, "OK", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse<CourseResponseDto?>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        public async Task<BaseResponse<CourseResponseDto>> UpdateCourseAsync(Guid courseId, CreateCourseRequestDto request)
        {
            try
            {
                var course = await _repo.GetCourseWithDetailsAsync(courseId);
                if (course == null) return BaseResponse<CourseResponseDto>.Fail("Course not found", null, 404);

                course.Title = request.Title ?? course.Title;
                course.Description = request.Description ?? course.Description;

                await _repo.UpdateCourseAsync(course);

                var dto = new CourseResponseDto { CourseId = course.CourseId, Title = course.Title, Description = course.Description, Modules = course.Modules.Select(m => new ModuleResponseDto { ModuleId = m.ModuleId, Title = m.Title, OrderIndex = m.OrderIndex, Lessons = m.Lessons.Select(l => new LessonResponseDto { LessonId = l.LessonId, Title = l.Title, Content = l.Content, ModuleId = l.ModuleId }).ToList() }).ToList() };

                return BaseResponse<CourseResponseDto>.Ok(dto, "Course updated", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse<CourseResponseDto>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        public async Task<BaseResponse<ModuleResponseDto>> UpdateModuleAsync(Guid moduleId, CreateModuleRequestDto request)
        {
            try
            {
                var module = await _repo.GetModuleByIdAsync(moduleId);
                if (module == null) return BaseResponse<ModuleResponseDto>.Fail("Module not found", null, 404);

                module.Title = request.Title ?? module.Title;
                module.OrderIndex = request.OrderIndex;

                await _repo.UpdateModuleAsync(module);

                var dto = new ModuleResponseDto { ModuleId = module.ModuleId, Title = module.Title, OrderIndex = module.OrderIndex, Lessons = module.Lessons.Select(l => new LessonResponseDto { LessonId = l.LessonId, Title = l.Title, Content = l.Content, ModuleId = l.ModuleId }).ToList() };

                return BaseResponse<ModuleResponseDto>.Ok(dto, "Module updated", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse<ModuleResponseDto>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        public async Task<BaseResponse<LessonResponseDto>> UpdateLessonAsync(Guid lessonId, CreateLessonRequestDto request)
        {
            try
            {
                var lesson = await _repo.GetLessonByIdAsync(lessonId);
                if (lesson == null) return BaseResponse<LessonResponseDto>.Fail("Lesson not found", null, 404);

                lesson.Title = request.Title ?? lesson.Title;
                lesson.Content = request.Content ?? lesson.Content;

                await _repo.UpdateLessonAsync(lesson);

                var dto = new LessonResponseDto { LessonId = lesson.LessonId, Title = lesson.Title, Content = lesson.Content, ModuleId = lesson.ModuleId };
                return BaseResponse<LessonResponseDto>.Ok(dto, "Lesson updated", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse<LessonResponseDto>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        public async Task<BaseResponse> DeleteCourseAsync(Guid courseId)
        {
            try
            {
                var existing = await _repo.GetCourseWithDetailsAsync(courseId);
                if (existing == null) return BaseResponse.Fail("Course not found", null, 404);

                await _repo.DeleteCourseAsync(courseId);
                return BaseResponse.Ok("Course deleted", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        public async Task<BaseResponse> DeleteModuleAsync(Guid moduleId)
        {
            try
            {
                var existing = await _repo.GetModuleByIdAsync(moduleId);
                if (existing == null) return BaseResponse.Fail("Module not found", null, 404);

                await _repo.DeleteModuleAsync(moduleId);
                return BaseResponse.Ok("Module deleted", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        public async Task<BaseResponse> DeleteLessonAsync(Guid lessonId)
        {
            try
            {
                var existing = await _repo.GetLessonByIdAsync(lessonId);
                if (existing == null) return BaseResponse.Fail("Lesson not found", null, 404);

                await _repo.DeleteLessonAsync(lessonId);
                return BaseResponse.Ok("Lesson deleted", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        
    }
}
