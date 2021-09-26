using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiberConnectionClient.Models
{
    public partial class Admin
    {
        public int AdminId { get; set; }
        public string AdminUserName { get; set; }
        public string AdminPassword { get; set; }
    }
}
