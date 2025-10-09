using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doctor.Core.Entities
{
    public class Doctor : BaseEntity<Guid>
    {
        #region Properties
        public Guid UserId { get; set; }
        public string? Specialty { get; set; }
        public string? Qualifications { get; set; }
        public string? LicenseNumber { get; set; }
        public decimal? ConsultationFee { get; set; }
        public string? OpenmrsProviderUuid { get; set; }
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
