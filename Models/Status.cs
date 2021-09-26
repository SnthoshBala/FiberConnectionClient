using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiberConnectionClient.Models
{
    public partial class Status
    {
        public int StatusId { get; set; }
        public string Status1 { get; set; }
        public int? EmployeeId { get; set; }
        public int? CustomerId { get; set; }
        public int? PlanId { get; set; }
        public int? BillingNumber { get; set; }
        public double? PlanPrice { get; set; }
        public string PlanName { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeePhonenumber { get; set; }

        public virtual Billing BillingNumberNavigation { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual FiberPlan Plan { get; set; }
    }
}
