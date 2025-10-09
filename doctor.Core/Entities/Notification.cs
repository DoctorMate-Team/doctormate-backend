using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doctor.Core.Entities
{
    public class Notification : BaseEntity<Guid>
    {
        #region Properties
        public Guid UserId { get; set; }
        public string Message { get; set; } = null!;
        public string? Type { get; set; } // booking|cancel|diagnosis|chat|payment
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        #endregion

    }
}
