using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class ValidationCode
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public string? Code { get; set; }
        public DateTime? Expire { get; set; }

        public virtual User? User { get; set; }
    }
}
