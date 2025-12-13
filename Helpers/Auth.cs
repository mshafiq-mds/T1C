using CustomGuid.AspNet.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Prodata.WebForm.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prodata.WebForm
{
	public static class Auth
	{
        public static UserManager UserManager()
        {
            return HttpContext.Current?.GetOwinContext()?.GetUserManager<UserManager>();
        }

        public static User User()
        {
            if (HttpContext.Current?.User?.Identity?.IsAuthenticated == true)
            {
                var manager = HttpContext.Current.GetOwinContext().GetUserManager<UserManager>();
                return manager.FindById(HttpContext.Current.User.Identity.GetUserID());
            }
            return null;
        }

        public static Guid Id()
        {
            return User() != null ? User().Id : Guid.Empty;
        }

        public static bool Can(Guid id, string module)
        {
            return UserManager().HasAccess(id, module);
        }

        public static bool Can(this User user, string module)
        {
            if (HttpContext.Current?.User?.Identity?.IsAuthenticated == true)
            {
                return UserManager().HasAccess(user.Id, module);
            }
            return false;
        }

        public static List<string> CCMSBizAreaCodes()
        {
            var ipmsBizArea = new Class.IPMSBizArea();

            if (User() != null && !User().IsSuperadmin())
            {
                return new Class.IPMSBizArea().GetBizAreaCodes(User().CCMSBizAreaCode);
            }
            return new List<string>();
        }
    }
}