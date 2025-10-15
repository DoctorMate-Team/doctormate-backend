using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doctor.Core.Entities
{
    public class Appointment:BaseEntity<Guid>
    {
        #region Property
        public DateTime ScheduledStart { get; set; }
        public DateTime? ScheduledEnd { get; set; }
        public string Status { get; set; } = "pending";
        public string? Reason { get; set; }
        public string? AppointmentType { get; set; } // in_person | online
        public string SyncStatus { get; set; } = "pending"; // pending | synced | failed
        public DateTime? CanceledAt { get; set; }
        public string? OpenmrsAppointmentUuid { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        #endregion
        #region Relation With Patient
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
        #endregion
        #region Relation With Doctor
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;
        #endregion
        #region Relation With Location 
        public Guid? LocationId { get; set; }
        #endregion
        #region Relation With CanceledBy (User)
        public Guid? CanceledBy { get; set; }
        #endregion
    }
}
