using Microsoft.EntityFrameworkCore;
using TeamIndia.TalentFlow.Application.Interfaces;
using TeamIndia.TalentFlow.Domain.Entities;
using TeamIndia.TalentFlow.Infrastructure.DbContext;

namespace TeamIndia.TalentFlow.Infrastructure.Repositories
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly ApplicationDbContext _db;

        public CertificateRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Certificate> AddCertificateAsync(Certificate cert)
        {
            await _db.Certificates.AddAsync(cert);
            await _db.SaveChangesAsync();
            return cert;
        }

        public async Task<Certificate?> GetCertificateByCourseAndUserAsync(Guid courseId, Guid userId)
        {
            return await _db.Certificates.FirstOrDefaultAsync(c => c.CourseId == courseId && c.UserId == userId);
        }

        public async Task<IEnumerable<Certificate>> GetCertificatesForUserAsync(Guid userId)
        {
            return await _db.Certificates.Where(c => c.UserId == userId).ToListAsync();
        }
    }
}
