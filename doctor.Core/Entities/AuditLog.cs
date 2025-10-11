using doctor.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doctor.Core.Entities
{
    public class AuditLog : BaseEntity<Guid>
    {
        #region Properties
        public Guid UserId { get; set; }
        public string Action { get; set; } = null!;
        public string Entity { get; set; } = null!;
        public Guid? EntityId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime LogTime { get; set; } = DateTime.UtcNow;
        public string? Response { get; set; } // JSON string
        #endregion
        #region Relation With User
        public User User { get; set; } = null!;
        #endregion
    }
}
