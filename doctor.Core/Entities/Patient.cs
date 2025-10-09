using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doctor.Core.Entities
{
    public class Patient : BaseEntity<Guid>
    {
        #region Properties
        public Guid UserId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? BloodType { get; set; }
        public string? EmergencyContact { get; set; } // JSON string
        public string? OpenmrsPatientUuid { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        #endregion

        #region Relation With User
        public User User { get; set; } = null!;
        #endregion

        #region Relation With Appointment
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        #endregion

    }
}
