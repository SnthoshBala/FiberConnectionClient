using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FiberConnectionClient.Models
{
    public partial class FiberPlan
    {
        public FiberPlan()
        {
            Billings = new HashSet<Billing>();
            Statuses = new HashSet<Status>();
        }

        public int PlanId { get; set; }
        [Required]
        [StringLength(150)]
        public string PlanName { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public string PlanPrice { get; set; }
        [Required]

        public string PlanSpeed { get; set; }
        [Required]
        public string Validity { get; set; }
        [Required]
        public int? OfferId { get; set; }
        [Required]
        public string Image { get; set; }

        public virtual Offer Offer { get; set; }
        public virtual ICollection<Billing> Billings { get; set; }
        public virtual ICollection<Status> Statuses { get; set; }
    }
}
