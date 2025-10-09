using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doctor.Core.Entities
{
    public class Diagnosis : BaseEntity<Guid>
    {
        #region Properties
        public Guid MedicalRecordId { get; set; }
        public Guid? AppointmentId { get; set; }
        public Guid DiagnosedBy { get; set; }
        public string? DiagnosisText { get; set; }
        public string? IcdCode { get; set; }
        public string? Severity { get; set; }
        public string? OpenmrsObsUuid { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        #endregion

    }
}
