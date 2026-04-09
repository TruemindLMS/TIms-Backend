using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Dtos.Response;

namespace TeamIndia.TalentFlow.Application.Interfaces
{
    public interface ICertificateService
    {
        Task<BaseResponse<CertificateResponseDto>> GenerateCertificateAsync(Guid courseId, Guid userId);
        Task<BaseResponse<CertificateResponseDto>> GetCertificateAsync(Guid courseId, Guid userId);
    }
}
