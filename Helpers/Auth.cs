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

        public static List<string> IPMSBizAreaCodes()
        {
            var ipmsBizArea = new Class.IPMSBizArea();

            if (User() != null && !User().IsSuperadmin())
            {
                string ipmsRoleCode = User().iPMSRoleCode;
                string baType = ipmsBizArea.GetTypeByCode(User().iPMSBizAreaCode);
                if (baType.Equals("MILL", StringComparison.OrdinalIgnoreCase))
                {
                    return new List<string> { User().iPMSBizAreaCode };
                }
                else
                {
                    if (User().iPMSBizAreaCode.Equals("001") || User().iPMSBizAreaCode.Equals("002") || User().iPMSBizAreaCode.Equals("003"))
                    {
                        var bizAreas = ipmsBizArea.GetIPMSBizAreas("MILL", User().iPMSRoleCode);
                        if (bizAreas != null && bizAreas.Count > 0)
                        {
                            return bizAreas.Select(x => x.Code).ToList();
                        }
                    }
                }
            }
            return new List<string>();
        }
    }
}