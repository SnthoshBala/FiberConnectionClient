using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        [StringLength(150)]
        public string Name { get; set; }
        [Required]
        [Range(21, 100, ErrorMessage = "Age should be between 21 - 100")]
        [DisplayName("Age in Years")]
        public int? Age { get; set; }
        [Required]
        [StringLength(maximumLength: 150, ErrorMessage = "Enter valid Address")]
        public string Address { get; set; }
        [Required]
        [StringLength(maximumLength: 150, ErrorMessage = "Enter valid Address")]
        public string WorkLocation { get; set; }
        [Required]
        [StringLength(maximumLength: 10, ErrorMessage = "Phone number should be of 10 digits", MinimumLength = 10)]
        public string PhoneNumber { get; set; }
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid EmailId")]
        public string EmployeeMail { get; set; }
        [Required]
        [DataType(DataType.Password, ErrorMessage = "Enter Correct Password")]
        public string EmployeePassword { get; set; }
        public virtual ICollection<Status> Statuses { get; set; }
    }
}
