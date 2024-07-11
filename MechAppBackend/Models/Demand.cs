using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class Demand
    {
        public Demand()
        {
            DemandsItems = new HashSet<DemandsItem>();
        }

        public long Id { get; set; }
        public long? UserId { get; set; }
        public long? DepartmentId { get; set; }
        public DateTime? Date { get; set; }
        public short? Status { get; set; }
        public short? IsDeleted { get; set; }

        public virtual Department? Department { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<DemandsItem> DemandsItems { get; set; }
    }
}
