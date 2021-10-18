using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FiberConnectionClient.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Billings = new HashSet<Billing>();
            Statuses = new HashSet<Status>();
        }

        public int CustomerId { get; set; }
        [Required]
        [StringLength(150)]
        public string CustomerName { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        [Required]
        public string CustomerAddress { get; set; }
        [Required]
        [StringLength(maximumLength: 10, ErrorMessage = "Phone number should be of 10 digits", MinimumLength = 10)]
        public string CustomerPhoneNumber { get; set; }
        [Required]
        [StringLength(maximumLength: 16, ErrorMessage = "Aadhar Number should be of 16 digits", MinimumLength = 16)]
        public string CustomerAadharNo { get; set; }
        [Required(ErrorMessage = "Please enter EmailID")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid EmailId")]
        public string CustomerMailId { get; set; }
        [Required]
        [DataType(DataType.Password, ErrorMessage = "Enter Correct Password")]
        public string CustomerPassword { get; set; }

        public virtual ICollection<Billing> Billings { get; set; }
        public virtual ICollection<Status> Statuses { get; set; }
    }
}
