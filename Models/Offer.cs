using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiberConnectionClient.Models
{
    public partial class Offer
    {
        public Offer()
        {
            FiberPlans = new HashSet<FiberPlan>();
        }

        public int OfferId { get; set; }
        public string Voot { get; set; }
        public string Lionplay { get; set; }
        public string Hungamaplay { get; set; }
        public string Ultra { get; set; }
        public string Hotstar { get; set; }
        public string Netflix { get; set; }
        public string Others { get; set; }

        public virtual ICollection<FiberPlan> FiberPlans { get; set; }
    }
}
