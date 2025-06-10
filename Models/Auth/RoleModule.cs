using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Prodata.WebForm.Models.System;

namespace Prodata.WebForm.Models.Auth
{
	public class RoleModule
	{
        [Key, Column(Order = 0)]
        public Guid RoleId { get; set; }

        [Key, Column(Order = 1)]
        public Guid ModuleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; }
    }
}