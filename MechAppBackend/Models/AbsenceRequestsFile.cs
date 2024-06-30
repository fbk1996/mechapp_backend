using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class AbsenceRequestsFile
    {
        public long Id { get; set; }
        public long? RequestId { get; set; }
        public string? File { get; set; }

        public virtual AbsenceRequest? Request { get; set; }
    }
}
