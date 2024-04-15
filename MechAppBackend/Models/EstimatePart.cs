using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class EstimatePart
    {
        public long Id { get; set; }
        public long? EstimateId { get; set; }
        public string? Name { get; set; }
        public string? Ean { get; set; }
        public int? Amount { get; set; }
        public decimal? GrossUnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }

        public virtual Estimate? Estimate { get; set; }
    }
}
