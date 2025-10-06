using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doctor.Core.Entities
{
    public class IntegrationLog
    {
        public Guid Id { get; set; }
        public string Endpoint { get; set; } = null!;
        public string Method { get; set; } = null!;
        public string? EntityType { get; set; }
        public Guid? EntityId { get; set; }
        public string? RequestPayload { get; set; } // JSON string
        public string? ResponsePayload { get; set; } // JSON string
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
