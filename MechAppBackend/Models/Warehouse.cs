using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class Warehouse
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Ean { get; set; }
        public int? Amount { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? Stand { get; set; }
        public string? PlaceNumber { get; set; }
        public long? DepartmentId { get; set; }

        public virtual Department? Department { get; set; }
    }
}
