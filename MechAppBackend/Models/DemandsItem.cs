using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class DemandsItem
    {
        public long Id { get; set; }
        public long? DemandId { get; set; }
        public string? Name { get; set; }
        public string? Ean { get; set; }
        public decimal? GrossUnitPrice { get; set; }
        public int? Amount { get; set; }
        public short? Status { get; set; }

        public virtual Demand? Demand { get; set; }
    }
}
