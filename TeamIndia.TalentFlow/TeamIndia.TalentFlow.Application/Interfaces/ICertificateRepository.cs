using TeamIndia.TalentFlow.Domain.Entities;

namespace TeamIndia.TalentFlow.Application.Interfaces
{
    public interface ICertificateRepository
    {
        Task<Certificate> AddCertificateAsync(Certificate cert);
        Task<Certificate?> GetCertificateByCourseAndUserAsync(Guid courseId, Guid userId);
        Task<IEnumerable<Certificate>> GetCertificatesForUserAsync(Guid userId);
    }
}
