using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class AirConditioningReport
    {
        public long Id { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public float? AmountEnd { get; set; }
        public short? Type { get; set; }
        public long? DepartmentId { get; set; }

        public virtual Department? Department { get; set; }
    }
}
