using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class Service
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public float? Duration { get; set; }
        public decimal? Price { get; set; }
        public short? IsActive { get; set; }
    }
}
