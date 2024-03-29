using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class UsersDepartment
    {
        public long Id { get; set; }
        public long? DepartmentId { get; set; }
        public long? UserId { get; set; }

        public virtual Department? Department { get; set; }
        public virtual User? User { get; set; }
    }
}
