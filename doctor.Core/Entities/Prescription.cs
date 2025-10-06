using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doctor.Core.Entities
{
    public class Prescription
    {
        public Guid Id { get; set; }
        public Guid DiagnosisId { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public string DrugName { get; set; } = null!;
        public string? Dosage { get; set; }
        public string? Instructions { get; set; }
        public int? DurationDays { get; set; }
        public string? OpenmrsOrderUuid { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
