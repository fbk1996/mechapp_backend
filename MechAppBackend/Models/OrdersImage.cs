using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class OrdersImage
    {
        public long Id { get; set; }
        public long? OrderId { get; set; }
        public string? Image { get; set; }

        public virtual Order? Order { get; set; }
    }
}
