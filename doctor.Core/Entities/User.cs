using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace doctor.Core.Entities
{
    public class User : BaseEntity<Guid>
    {
        #region Properties
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string Role { get; set; } = null!; // patient | doctor | admin
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        #endregion
        #region Relation With Patient
        public Patient? Patient { get; set; }
        #endregion
        #region Relation With Doctor
        public Doctor? Doctor { get; set; }
        #endregion
        #region Relation With Notification
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        #endregion

    }
}
