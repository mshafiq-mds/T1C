using CustomGuid.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models.Auth
{
	public class Role : CustomIdentityRole
	{
		public virtual ICollection<RoleModule> RoleModules { get; set; }
	}
}