using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class Department
    {
        public Department()
        {
            Orders = new HashSet<Order>();
            UsersDepartments = new HashSet<UsersDepartment>();
        }

        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Postcode { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<UsersDepartment> UsersDepartments { get; set; }
    }
}
