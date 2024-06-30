using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class AbsenceRequest
    {
        public AbsenceRequest()
        {
            AbsenceRequestsFiles = new HashSet<AbsenceRequestsFile>();
        }

        public long Id { get; set; }
        public long? UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public short? Status { get; set; }
        public string? AbsenceType { get; set; }
        public string? SubmitDescription { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<AbsenceRequestsFile> AbsenceRequestsFiles { get; set; }
    }
}
