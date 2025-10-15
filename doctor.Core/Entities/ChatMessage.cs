using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doctor.Core.Entities
{
    public class ChatMessage : BaseEntity<Guid>
    {
        #region Properties
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid? AppointmentId { get; set; }
        public string Message { get; set; } = null!;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        #endregion
    }
}
