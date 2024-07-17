using System;
using System.Collections.Generic;

namespace MechAppBackend.MechAppApModels
{
    public partial class UsersRole
    {
        public long? UserId { get; set; }
        public long? RolesId { get; set; }
        public long Id { get; set; }

        public virtual Role? Roles { get; set; }
        public virtual User? User { get; set; }
    }
}
