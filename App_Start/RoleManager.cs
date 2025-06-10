using CustomGuid.AspNet.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.Auth;
using Prodata.WebForm.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prodata.WebForm
{
	public class RoleManager : RoleManager<Role, Guid>
	{
		public RoleManager(IRoleStore<Role, Guid> roleStore) : base(roleStore)
		{
		}

		public static RoleManager Create(IdentityFactoryOptions<RoleManager> options, IOwinContext context)
		{
			return new RoleManager(new CustomRoleStore<Role>(context.Get<AppDbContext>()));
		}

        #region Custom method

        /// <summary>
        /// Get list of roles.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<RoleListViewModel> GetRoles(string name = null)
        {
            var query = Roles.Select(r => new RoleListViewModel
            {
                Id = r.Id,
                Name = r.Name
            });

            if (!string.IsNullOrEmpty(name))
            {
                string lowerName = name.ToLower();
                query = query.Where(q => q.Name.ToLower().Contains(lowerName));
            }

            return query.OrderBy(q => q.Name).ToList();
        }

        /// <summary>
        /// Return true if the role exists
        /// </summary>
        /// <param name="id">Role id.</param>
        /// <param name="name">Role name.</param>
        /// <returns></returns>
        public bool RoleExists(Guid id, string name)
        {
            return Roles.FirstOrDefault(r => r.Id != id && r.Name.ToLower().Equals(name.ToLower())) != null ? true : false;
        }
        #endregion
    }
}