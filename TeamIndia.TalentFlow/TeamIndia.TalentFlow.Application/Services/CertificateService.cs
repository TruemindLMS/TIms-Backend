using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Dtos.Response;
using TeamIndia.TalentFlow.Application.Interfaces;
using TeamIndia.TalentFlow.Domain.Entities;

namespace TeamIndia.TalentFlow.Application.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly ICertificateRepository _repo;
        private readonly ICourseRepository _courseRepo;
        private readonly IProgressRepository _progressRepo;
        private readonly IUserRepository _userRepo;
        private readonly IEmailService _emailService;

        public CertificateService(ICertificateRepository repo, ICourseRepository courseRepo, IProgressRepository progressRepo, IUserRepository userRepo, IEmailService emailService)
        {
            _repo = repo;
            _courseRepo = courseRepo;
            _progressRepo = progressRepo;
            _userRepo = userRepo;
            _emailService = emailService;
        }

        public async Task<BaseResponse<CertificateResponseDto>> GenerateCertificateAsync(Guid courseId, Guid userId)
        {
            try
            {
                // ensure course exists
                var course = await _courseRepo.GetCourseWithDetailsAsync(courseId);
                if (course == null) return BaseResponse<CertificateResponseDto>.Fail("Course not found", null, 404);

                // check if user has completed all lessons
                var total = await _progressRepo.GetTotalLessonsAsync(courseId);
                var completed = await _progressRepo.GetCompletedLessonsAsync(courseId, userId);
                if (total == 0 || completed < total) return BaseResponse<CertificateResponseDto>.Fail("Course not completed", null, 400);

                // check if certificate already exists
                var existing = await _repo.GetCertificateByCourseAndUserAsync(courseId, userId);
                if (existing != null)
                {
                    var dtoExisting = new CertificateResponseDto
                    {
                        CertificateId = existing.CertificateId,
                        CourseId = existing.CourseId,
                        UserId = existing.UserId,
                        IssuedOnUtc = existing.IssuedOnUtc,
                        FileUrl = existing.FileUrl,
                        HtmlContent = existing.HtmlContent
                    };

                    return BaseResponse<CertificateResponseDto>.Ok(dtoExisting, "Already issued", 200);
                }

                // get user info
                var user = await _userRepo.GetByIdAsync(userId);
                var userName = user != null ? (string.IsNullOrWhiteSpace(user.FullName) ? user.Email : user.FullName) : userId.ToString();

                var completionDate = await _progressRepo.GetCourseCompletionDateAsync(courseId, userId);
                var html = await RenderCertificateHtmlAsync(course.Title, userId, completionDate);

                string? fileUrl = null;

                var cert = new Certificate
                {
                    CertificateId = Guid.NewGuid(),
                    CourseId = courseId,
                    UserId = userId,
                    IssuedOnUtc = DateTime.UtcNow,
                    HtmlContent = html,
                    FileUrl = fileUrl
                };

                await _repo.AddCertificateAsync(cert);

                var dto = new CertificateResponseDto
                {
                    CertificateId = cert.CertificateId,
                    CourseId = cert.CourseId,
                    UserId = cert.UserId,
                    IssuedOnUtc = cert.IssuedOnUtc,
                    FileUrl = cert.FileUrl,
                    HtmlContent = cert.HtmlContent
                };

                try
                {
                    if (user != null && !string.IsNullOrWhiteSpace(user.Email))
                    {
                        var subject = $"Your certificate for {course.Title}";

                        var basePath = AppContext.BaseDirectory;
                        var templatesRoot = System.IO.Path.Combine(basePath, "Resources", "Emails");
                        var filePath = System.IO.Path.Combine(templatesRoot, "certificate.html");
                        string emailHtml = "";
                        if (System.IO.File.Exists(filePath))
                        {
                            emailHtml = await System.IO.File.ReadAllTextAsync(filePath);
                            emailHtml = emailHtml.Replace("{CourseTitle}", System.Net.WebUtility.HtmlEncode(course.Title));
                            emailHtml = emailHtml.Replace("{UserName}", System.Net.WebUtility.HtmlEncode(userName));
                            emailHtml = emailHtml.Replace("{IssuedOn}", System.Net.WebUtility.HtmlEncode(DateTime.UtcNow.ToString("yyyy-MM-dd")));
                            emailHtml = emailHtml.Replace("{CertificateHtml}", html);
                            emailHtml = emailHtml.Replace("{SupportEmail}", "support@talentflow.com");
                        }

                        // generate PDF from HTML using PuppeteerSharp
                        byte[] pdfBytes = Array.Empty<byte>();
                        try
                        {
                            var browserFetcher = new PuppeteerSharp.BrowserFetcher();
                            await browserFetcher.DownloadAsync(PuppeteerSharp.BrowserFetcher.DefaultChromiumRevision);
                            var launchOptions = new PuppeteerSharp.LaunchOptions { Headless = true };
                            using (var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(launchOptions))
                            using (var page = await browser.NewPageAsync())
                            {
                                await page.SetContentAsync(html);
                                pdfBytes = await page.PdfDataAsync();
                            }
                        }
                        catch
                        {
                        }

                        await _emailService.SendEmailWithAttachmentAsync(user.Email, subject, emailHtml, "certificate.pdf", pdfBytes);
                    }
                }
                catch
                {
                }

                return BaseResponse<CertificateResponseDto>.Ok(dto, "Generated", 201);
            }
            catch (Exception ex)
            {
                return BaseResponse<CertificateResponseDto>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        public async Task<BaseResponse<CertificateResponseDto>> GetCertificateAsync(Guid courseId, Guid userId)
        {
            try
            {
                var cert = await _repo.GetCertificateByCourseAndUserAsync(courseId, userId);
                if (cert == null) return BaseResponse<CertificateResponseDto>.Fail("Not found", null, 404);

                var dto = new CertificateResponseDto
                {
                    CertificateId = cert.CertificateId,
                    CourseId = cert.CourseId,
                    UserId = cert.UserId,
                    IssuedOnUtc = cert.IssuedOnUtc,
                    FileUrl = cert.FileUrl,
                    HtmlContent = cert.HtmlContent
                };

                return BaseResponse<CertificateResponseDto>.Ok(dto, "OK", 200);
            }
            catch (Exception ex)
            {
                return BaseResponse<CertificateResponseDto>.Fail("An error occurred", new[] { ex.Message }, 500);
            }
        }

        private async Task<string> RenderCertificateHtmlAsync(string courseTitle, Guid userId, DateTime? completionDate = null)
        {
            // Try to load template file from disk (Resources/certificate.html in app output)
            var basePath = AppContext.BaseDirectory;
            var filePath = System.IO.Path.Combine(basePath, "Resources", "certificate.html");
            string template = null;

            if (System.IO.File.Exists(filePath))
            {
                template = await System.IO.File.ReadAllTextAsync(filePath);
            }
            else
            {
                // fallback to embedded resource
                var assembly = typeof(CertificateService).Assembly;
                var resourceName = "TeamIndia.TalentFlow.API.Resources.certificate.html";
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    using var reader = new System.IO.StreamReader(stream);
                    template = await reader.ReadToEndAsync();
                }
            }

            if (string.IsNullOrWhiteSpace(template))
            {
                // final fallback simple template
                var fallbackUser = (await _userRepo.GetByIdAsync(userId))?.FullName ?? userId.ToString();
                return $"<html><body style=\"font-family:Arial,sans-serif;text-align:center;padding:40px;\"><h1 style=\"color:#0b5ed7;\">Certificate of Completion</h1><p style=\"font-size:18px;\"><strong>{System.Net.WebUtility.HtmlEncode(fallbackUser)}</strong></p><p style=\"font-size:16px;\">has completed the course <strong>{System.Net.WebUtility.HtmlEncode(courseTitle)}</strong></p><p style=\"margin-top:20px;color:#666;\">Issued on {DateTime.UtcNow:yyyy-MM-dd}</p></body></html>";
            }

            // replace tokens
            template = template.Replace("{CourseTitle}", System.Net.WebUtility.HtmlEncode(courseTitle));
            var user = await _userRepo.GetByIdAsync(userId);
            var userName = user != null ? (string.IsNullOrWhiteSpace(user.FullName) ? user.Email : user.FullName) : userId.ToString();
            template = template.Replace("{UserName}", System.Net.WebUtility.HtmlEncode(userName));
            template = template.Replace("{IssuedOn}", System.Net.WebUtility.HtmlEncode(DateTime.UtcNow.ToString("yyyy-MM-dd")));
            template = template.Replace("{CompletedOn}", System.Net.WebUtility.HtmlEncode((completionDate ?? DateTime.UtcNow).ToString("yyyy-MM-dd")));
            template = template.Replace("{CertificateHtml}", "");

            return template;
        }
    }
}
