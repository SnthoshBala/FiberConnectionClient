using System;
using System.Collections.Generic;
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
        public string PlanName { get; set; }
        public string PlanPrice { get; set; }
        public string PlanSpeed { get; set; }
        public string Validity { get; set; }
        public int? OfferId { get; set; }
        public string Image { get; set; }

        public virtual Offer Offer { get; set; }
        public virtual ICollection<Billing> Billings { get; set; }
        public virtual ICollection<Status> Statuses { get; set; }
    }
}
