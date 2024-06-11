using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class AppSetting
    {
        public long Id { get; set; }
        public float? Rwhprice { get; set; }
        public float? Margin { get; set; }
        public float? MarginLoyalCustomer { get; set; }
        public float? RwhdiscountLoyalCustomer { get; set; }
        public float? MarginIndividual { get; set; }
        public int? LogsRetention { get; set; }
    }
}
