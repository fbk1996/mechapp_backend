using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class UsersRole
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public long? RoleId { get; set; }

        public virtual Role? Role { get; set; }
        public virtual User? User { get; set; }
    }
}
