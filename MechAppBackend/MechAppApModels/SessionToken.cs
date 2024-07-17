using System;
using System.Collections.Generic;

namespace MechAppBackend.MechAppApModels
{
    public partial class SessionToken
    {
        public long? UserId { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpireDate { get; set; }
        public long Id { get; set; }
    }
}
