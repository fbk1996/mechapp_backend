using System;
using System.Collections.Generic;

namespace MechAppBackend.MechAppApModels
{
    public partial class User
    {
        public User()
        {
            UsersRoles = new HashSet<UsersRole>();
        }

        public long Id { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
        public string Salt { get; set; } = null!;
        public bool? IsDeleted { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        public virtual ICollection<UsersRole> UsersRoles { get; set; }
    }
}
