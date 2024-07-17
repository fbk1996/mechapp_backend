using System;
using System.Collections.Generic;

namespace MechAppBackend.ClientsModels
{
    public partial class TicketsMessage
    {
        public TicketsMessage()
        {
            TicketsFiles = new HashSet<TicketsFile>();
        }

        public long Id { get; set; }
        public long? TicketId { get; set; }
        public string User { get; set; } = null!;
        public string? Message { get; set; }
        public short? IsNotificationSend { get; set; }

        public virtual Ticket? Ticket { get; set; }
        public virtual ICollection<TicketsFile> TicketsFiles { get; set; }
    }
}
