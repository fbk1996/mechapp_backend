using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class Log
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public DateTime? Date { get; set; }
        public string? Action { get; set; }
        public string? Description { get; set; }

        public virtual User? User { get; set; }
    }
}
