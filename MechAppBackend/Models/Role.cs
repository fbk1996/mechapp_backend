using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class Role
    {
        public Role()
        {
            UsersRoles = new HashSet<UsersRole>();
        }

        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Permissions { get; set; }

        public virtual ICollection<UsersRole> UsersRoles { get; set; }
    }
}
