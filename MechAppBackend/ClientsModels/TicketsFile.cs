using System;
using System.Collections.Generic;

namespace MechAppBackend.ClientsModels
{
    public partial class TicketsFile
    {
        public long Id { get; set; }
        public long? MessageId { get; set; }
        public string? File { get; set; }
        public string? Name { get; set; }

        public virtual TicketsMessage? Message { get; set; }
    }
}
