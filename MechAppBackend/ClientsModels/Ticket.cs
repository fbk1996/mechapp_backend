using System;
using System.Collections.Generic;

namespace MechAppBackend.ClientsModels
{
    public partial class Ticket
    {
        public Ticket()
        {
            TicketsMessages = new HashSet<TicketsMessage>();
        }

        public long Id { get; set; }
        public DateTime? Date { get; set; }
        public string? SubscriptionId { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; } = null!;
        public string ClientLastname { get; set; } = null!;
        public string ClientEmail { get; set; } = null!;
        public string? ClientPhone { get; set; }
        public long? OwnerId { get; set; }
        public string Title { get; set; } = null!;
        public short? Status { get; set; }

        public virtual ICollection<TicketsMessage> TicketsMessages { get; set; }
    }
}
