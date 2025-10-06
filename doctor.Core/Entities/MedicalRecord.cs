using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doctor.Core.Entities
{
    public class MedicalRecord
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? RecordType { get; set; } // diagnosis | lab_result | imaging
        public string? Status { get; set; }
        public Guid? RecordedBy { get; set; }
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
        public string? OpenmrsEncounterUuid { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Patient Patient { get; set; } = null!;
    }
}
