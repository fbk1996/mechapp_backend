using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class Estimate
    {
        public Estimate()
        {
            EstimateParts = new HashSet<EstimatePart>();
            EstimateServices = new HashSet<EstimateService>();
        }

        public long Id { get; set; }
        public long? OrderId { get; set; }
        public decimal? TotalPartsPrice { get; set; }
        public decimal? TotalServicesPrice { get; set; }
        public decimal? TotalPrice { get; set; }

        public virtual Order? Order { get; set; }
        public virtual ICollection<EstimatePart> EstimateParts { get; set; }
        public virtual ICollection<EstimateService> EstimateServices { get; set; }
    }
}
