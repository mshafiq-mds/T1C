using CustomGuid.AspNet.Identity;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Prodata.WebForm.Models.Auth
{
	public class User : CustomIdentityUser
	{
        public ClaimsIdentity GenerateUserIdentity(UserManager manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = manager.CreateIdentity(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager manager)
        {
            return Task.FromResult(GenerateUserIdentity(manager));
        }

        public string Name { get; set; }

        [StringLength(20)]
        public string iPMSRoleCode { get; set; }

        [StringLength(20)]
        public string iPMSBizAreaCode { get; set; }

        [StringLength(20)]
        public string CCMSBizAreaCode { get; set; }
        public string CCMSRoleCode { get; set; }
        public bool? UserHQ { get; set; }
        
    }
}