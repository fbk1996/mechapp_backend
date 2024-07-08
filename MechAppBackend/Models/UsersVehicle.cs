using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class UsersVehicle
    {
        public UsersVehicle()
        {
            AirConditioningEntries = new HashSet<AirConditioningEntry>();
            Orders = new HashSet<Order>();
        }

        public long Id { get; set; }
        public long? Owner { get; set; }
        public string? Producer { get; set; }
        public string? Model { get; set; }
        public string? ProduceDate { get; set; }
        public long? Mileage { get; set; }
        public string? Vin { get; set; }
        public string? EngineNumber { get; set; }
        public string? RegistrationNumber { get; set; }
        public int? EnginePower { get; set; }
        public string? EngineCapacity { get; set; }
        public string? FuelType { get; set; }
        public short? IsDeleted { get; set; }

        public virtual User? OwnerNavigation { get; set; }
        public virtual ICollection<AirConditioningEntry> AirConditioningEntries { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
