using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doctor.Core.Entities
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime ScheduledStart { get; set; }
        public DateTime? ScheduledEnd { get; set; }
        public string Status { get; set; } = "pending";
        public string? Reason { get; set; }
        public Guid? LocationId { get; set; }
        public string? AppointmentType { get; set; } // in_person | online
        public string SyncStatus { get; set; } = "pending"; // pending | synced | failed
        public DateTime? CanceledAt { get; set; }
        public Guid? CanceledBy { get; set; }
        public string? OpenmrsAppointmentUuid { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Patient Patient { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;
    }
}
