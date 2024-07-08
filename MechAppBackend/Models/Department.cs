using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class Department
    {
        public Department()
        {
            AirConditioningEntries = new HashSet<AirConditioningEntry>();
            AirConditioningReports = new HashSet<AirConditioningReport>();
            Orders = new HashSet<Order>();
            UsersDepartments = new HashSet<UsersDepartment>();
            Warehouses = new HashSet<Warehouse>();
        }

        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Postcode { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }

        public virtual ICollection<AirConditioningEntry> AirConditioningEntries { get; set; }
        public virtual ICollection<AirConditioningReport> AirConditioningReports { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<UsersDepartment> UsersDepartments { get; set; }
        public virtual ICollection<Warehouse> Warehouses { get; set; }
    }
}
