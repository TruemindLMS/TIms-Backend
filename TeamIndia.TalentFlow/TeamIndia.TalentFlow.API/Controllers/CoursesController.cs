using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamIndia.TalentFlow.Application.Dtos;
using TeamIndia.TalentFlow.Application.Interfaces;

namespace TeamIndia.TalentFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseServices _services;
        public CoursesController(ICourseServices services)
        {
            _services = services;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequestDto request)
        {
            var res = await _services.CreateCourseAsync(request);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var res = await _services.GetAllCoursesAsync();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("{courseId:guid}")]
        public async Task<IActionResult> GetById(Guid courseId)
        {
            var res = await _services.GetCourseByIdAsync(courseId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("{courseId:guid}")]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<IActionResult> UpdateCourse(Guid courseId, [FromBody] CreateCourseRequestDto request)
        {
            var res = await _services.UpdateCourseAsync(courseId, request);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("modules/{moduleId:guid}")]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<IActionResult> UpdateModule(Guid moduleId, [FromBody] CreateModuleRequestDto request)
        {
            var res = await _services.UpdateModuleAsync(moduleId, request);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("{courseId:guid}/modules")]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<IActionResult> CreateModule(Guid courseId, [FromBody] CreateModuleRequestDto request)
        {
            var res = await _services.CreateModuleAsync(courseId, request);
            return StatusCode(res.StatusCode, res);
        }

        [HttpDelete("{courseId:guid}")]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<IActionResult> DeleteCourse(Guid courseId)
        {
            var res = await _services.DeleteCourseAsync(courseId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpDelete("modules/{moduleId:guid}")]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<IActionResult> DeleteModule(Guid moduleId)
        {
            var res = await _services.DeleteModuleAsync(moduleId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpDelete("lessons/{lessonId:guid}")]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<IActionResult> DeleteLesson(Guid lessonId)
        {
            var res = await _services.DeleteLessonAsync(lessonId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("lessons/{lessonId:guid}")]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<IActionResult> UpdateLesson(Guid lessonId, [FromBody] CreateLessonRequestDto request)
        {
            var res = await _services.UpdateLessonAsync(lessonId, request);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("{courseId:guid}/modules/{moduleId:guid}/lessons")]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<IActionResult> CreateLesson(Guid courseId, Guid moduleId, [FromBody] CreateLessonRequestDto request)
        {
            var res = await _services.CreateLessonAsync(courseId, moduleId, request);
            return StatusCode(res.StatusCode, res);
        }
    }
}
