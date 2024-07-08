using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class User
    {
        public User()
        {
            AbsenceRequests = new HashSet<AbsenceRequest>();
            AirConditioningEntries = new HashSet<AirConditioningEntry>();
            Logs = new HashSet<Log>();
            Orders = new HashSet<Order>();
            UsersDepartments = new HashSet<UsersDepartment>();
            UsersRoles = new HashSet<UsersRole>();
            UsersTokens = new HashSet<UsersToken>();
            UsersVehicles = new HashSet<UsersVehicle>();
            ValidationCodes = new HashSet<ValidationCode>();
        }

        public long Id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public string? Lastname { get; set; }
        public string? Nip { get; set; }
        public string? Phone { get; set; }
        public string? Postcode { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Color { get; set; }
        public string? AppRole { get; set; }
        public int? LoginAttempts { get; set; }
        public string? Salt { get; set; }
        public short? IsFirstLogin { get; set; }
        public string? CompanyName { get; set; }
        public string? Icon { get; set; }
        public short? IsLoyalCustomer { get; set; }
        public short? IsDeleted { get; set; }

        public virtual ICollection<AbsenceRequest> AbsenceRequests { get; set; }
        public virtual ICollection<AirConditioningEntry> AirConditioningEntries { get; set; }
        public virtual ICollection<Log> Logs { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<UsersDepartment> UsersDepartments { get; set; }
        public virtual ICollection<UsersRole> UsersRoles { get; set; }
        public virtual ICollection<UsersToken> UsersTokens { get; set; }
        public virtual ICollection<UsersVehicle> UsersVehicles { get; set; }
        public virtual ICollection<ValidationCode> ValidationCodes { get; set; }
    }
}
