using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FiberConnectionClient.Models
{
    public partial class Admin
    {
        public int AdminId { get; set; }
        [Required]
        [StringLength(150)]
        public string AdminUserName { get; set; }
        [Required]
        [DataType(DataType.Password, ErrorMessage = "Enter Correct Password")]
        public string AdminPassword { get; set; }
    }
}
