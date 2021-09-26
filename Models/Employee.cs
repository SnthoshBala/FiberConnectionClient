using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiberConnectionClient.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Statuses = new HashSet<Status>();
        }

        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public string Address { get; set; }
        public string WorkLocation { get; set; }
        public string PhoneNumber { get; set; }
        public string EmployeeMail { get; set; }
        public string EmployeePassword { get; set; }

        public virtual ICollection<Status> Statuses { get; set; }
    }
}
