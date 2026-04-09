using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamIndia.TalentFlow.Domain.Entities
{
    public class Certificate

    {
        [Key]
        public Guid CertificateId { get; set; }
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public DateTime IssueDate { get; set; }
        public string CertificateCode { get; set; } = string.Empty;
        public virtual Course? Course { get; set; }
    }
}
