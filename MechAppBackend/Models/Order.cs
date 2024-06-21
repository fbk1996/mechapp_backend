using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class Order
    {
        public Order()
        {
            CheckLists = new HashSet<CheckList>();
            Estimates = new HashSet<Estimate>();
            OrdersComplaints = new HashSet<OrdersComplaint>();
            OrdersImages = new HashSet<OrdersImage>();
        }

        public long Id { get; set; }
        public long? ClientId { get; set; }
        public long? VehicleId { get; set; }
        public long? DepartmentId { get; set; }
        public string? ClientDiagnose { get; set; }
        public short? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public short? SendStartNotification { get; set; }
        public short? SendDoneNotification { get; set; }
        public short? SendThanksNotification { get; set; }

        public virtual User? Client { get; set; }
        public virtual Department? Department { get; set; }
        public virtual UsersVehicle? Vehicle { get; set; }
        public virtual ICollection<CheckList> CheckLists { get; set; }
        public virtual ICollection<Estimate> Estimates { get; set; }
        public virtual ICollection<OrdersComplaint> OrdersComplaints { get; set; }
        public virtual ICollection<OrdersImage> OrdersImages { get; set; }
    }
}
