using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class OrdersComplaint
    {
        public long Id { get; set; }
        public long? OrderId { get; set; }
        public short? Status { get; set; }
        public string? Description { get; set; }
        public string? SubmitDescription { get; set; }
        public DateTime Date { get; set; }

        public virtual Order? Order { get; set; }
    }
}
