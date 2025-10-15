using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace doctor.Core.Entities.Identity
{
    public class User : IdentityUser<Guid>
    {
        #region Properties
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
        #region Relation With AuditLog
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        #endregion

    }
}
