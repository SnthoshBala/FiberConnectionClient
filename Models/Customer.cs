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
        public string CustomerName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string CustomerAddress { get; set; }
        [StringLength(maximumLength: 10, ErrorMessage = "Invalid Number", MinimumLength = 10)]
        public string CustomerPhoneNumber { get; set; }
        [StringLength(maximumLength: 16, ErrorMessage = "Invalid Number", MinimumLength = 16)]
        public string CustomerAadharNo { get; set; }
        [EmailAddress]
        public string CustomerMailId { get; set; }
        public string CustomerPassword { get; set; }

        public virtual ICollection<Billing> Billings { get; set; }
        public virtual ICollection<Status> Statuses { get; set; }
    }
}
