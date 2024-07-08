using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class AirConditioningEntry
    {
        public long Id { get; set; }
        public long? DepartmentId { get; set; }
        public long? ClientId { get; set; }
        public long? VehicleId { get; set; }
        public DateTime? Date { get; set; }
        public short? Status { get; set; }
        public short? Type { get; set; }
        public float? AmountGained { get; set; }
        public float? AmountGived { get; set; }
        public short? IsDeleted { get; set; }

        public virtual User? Client { get; set; }
        public virtual Department? Department { get; set; }
        public virtual UsersVehicle? Vehicle { get; set; }
    }
}
