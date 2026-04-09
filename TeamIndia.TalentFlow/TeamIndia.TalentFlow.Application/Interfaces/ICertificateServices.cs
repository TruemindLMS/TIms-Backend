using System;
using System.Collections.Generic;
using System.Text;

namespace TeamIndia.TalentFlow.Application.Interfaces
{
    public interface ICertificateService
    {
        Task<byte[]> GenerateCertificatePdfAsync(Guid userId, Guid courseId);
        Task<bool> IsCourseCompletedAsync(Guid userId, Guid courseId);
    }
}