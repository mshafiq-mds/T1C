using CustomGuid.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prodata.WebForm.Models.ViewModels;
using System.Data.SqlClient;

namespace Prodata.WebForm
{
	public class UserManager : CustomUserManager<User>
	{
        public UserManager(ICustomUserStore<User> store)
            : base(store)
        {
        }

        public static UserManager Create(IdentityFactoryOptions<UserManager> options, IOwinContext context)
        {
            var manager = new UserManager(new CustomUserStore<User>(context.Get<AppDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<User, Guid>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<User, Guid>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<User, Guid>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<User, Guid>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        #region Custom method
        /// <summary>
		/// Get list of users.
		/// </summary>
		/// <param name="filter">filter</param>
		/// <returns></returns>
        public List<UserListViewModel> GetUsers(string filter = null)
        {
            var db = new AppDbContext();

            // Step 1: Query initial data from Users
            var query = from u in db.Users
                        select new
                        {
                            u.Id,
                            u.Name,
                            Username = u.UserName,
                            u.Email,
                            Roles = (from ur in u.Roles
                                     join r in db.Roles on ur.RoleId equals r.Id
                                     orderby r.Name
                                     select r.Name).ToList(),
                            //u.iPMSRoleCode,
                            //u.iPMSBizAreaCode,
                            u.CCMSRoleCode,
                            u.CCMSBizAreaCode,
                            u.UserHQ
                        };

            if (!string.IsNullOrEmpty(filter))
                query = query.Where(q =>
                                    q.Name.Contains(filter) ||
                                    q.Username.Contains(filter) ||
                                    q.Email.Contains(filter) ||
                                    q.Roles.Any(role => role.Contains(filter)));

            // Step 2: Materialize the query (in memory)
            var userData = query
                .OrderBy(q => q.Name)
                .ThenBy(q => q.Username)
                .ToList();

            // Step 3: Get unique role codes and load role names in one SQL call
            var roleCodes = userData
                .Select(u => u.CCMSRoleCode)
                .Where(code => !string.IsNullOrEmpty(code))
                .Distinct()
                .ToList();

            var bizAreaCodes = userData
                .Select(u => u.CCMSBizAreaCode)
                .Where(code => !string.IsNullOrEmpty(code))
                .Distinct()
                .ToList();

            var ipmsRoleDict = new Dictionary<string, string>();
            var ipmsBizAreaDict = new Dictionary<string, string>();

            using (var con = new SqlConn().GetSqlConnection("iPMSConnection"))
            {
                con.Open();

                // Query Roles
                if (roleCodes.Any())
                {
                    var cmd = new SqlCommand();
                    cmd.Connection = con;

                    var roleParams = new List<string>();
                    for (int i = 0; i < roleCodes.Count; i++)
                    {
                        var param = "@role" + i;
                        cmd.Parameters.AddWithValue(param, roleCodes[i]);
                        roleParams.Add(param);
                    }

                    cmd.CommandText = $"SELECT code, name FROM Role WHERE code IN ({string.Join(",", roleParams)})";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var code = reader["code"]?.ToString();
                            var name = reader["name"]?.ToString();
                            if (!string.IsNullOrEmpty(code))
                                ipmsRoleDict[code] = name ?? string.Empty;
                        }
                    }
                }

                // Query Biz Areas
                if (bizAreaCodes.Any())
                {
                    var cmd = new SqlCommand();
                    cmd.Connection = con;

                    var areaParams = new List<string>();
                    for (int i = 0; i < bizAreaCodes.Count; i++)
                    {
                        var param = "@area" + i;
                        cmd.Parameters.AddWithValue(param, bizAreaCodes[i]);
                        areaParams.Add(param);
                    }

                    cmd.CommandText = $"SELECT code, description FROM BizArea WHERE code IN ({string.Join(",", areaParams)})";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var code = reader["code"]?.ToString();
                            var name = reader["description"]?.ToString();
                            if (!string.IsNullOrEmpty(code))
                                ipmsBizAreaDict[code] = name ?? string.Empty;
                        }
                    }
                }
            }

            // Step 4: Project to ViewModel using cached dictionaries
            var result = userData.Select(q => new UserListViewModel
            {
                Id = q.Id,
                Name = q.Name,
                Username = q.Username,
                Email = q.Email,
                Roles = string.Join(", ", q.Roles),
                CCMSRole = !string.IsNullOrEmpty(q.CCMSRoleCode) && ipmsRoleDict.TryGetValue(q.CCMSRoleCode, out var roleName)
                    ? q.CCMSRoleCode + " - " + roleName
                    : string.Empty,
                //IPMSRole = !string.IsNullOrEmpty(q.iPMSRoleCode) && ipmsRoleDict.TryGetValue(q.iPMSRoleCode, out var iroleName)
                //    ? q.iPMSRoleCode + " - " + iroleName
                //    : string.Empty,
                //IPMSBizArea = !string.IsNullOrEmpty(q.CCMSBizAreaCode) && ipmsBizAreaDict.TryGetValue(q.CCMSBizAreaCode, out var areaName)
                //    ? q.CCMSBizAreaCode + " - " + areaName
                //    : string.Empty
                CCMSBizArea = !string.IsNullOrEmpty(q.CCMSBizAreaCode) && ipmsBizAreaDict.TryGetValue(q.CCMSBizAreaCode, out var areaName)
                    ? q.CCMSBizAreaCode + " - " + areaName
                    : string.Empty,
                UserHQ = q.UserHQ
            })
            .Distinct()
            .ToList();

            return result;
        }

        /// <summary>
        /// Return true if the user exists.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool UserExists(string username = null, string email = null)
        {
            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(email))
                return true;

            return Users.Any(u =>
                (!string.IsNullOrEmpty(username) && u.UserName.ToLower() == username.ToLower()) ||
                (!string.IsNullOrEmpty(email) && u.Email.ToLower() == email.ToLower()));
        }

        /// <summary>
        /// Return true if the user exists.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool UserExists(Guid? id = null, string username = null, string email = null)
        {
            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(email))
                return true;

            return Users.Any(u =>
                (id == null || u.Id != id) && // Ignore the user with the given ID
                (!string.IsNullOrEmpty(username) && u.UserName.ToLower() == username.ToLower() ||
                !string.IsNullOrEmpty(email) && u.Email.ToLower() == email.ToLower()));
        }

        /// <summary>
        /// Get list of modules that a user can access.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<UserAccessViewModel> GetAccess(Guid id)
        {
            var userRoles = this.GetRoles(id);
            var userAccess = new List<UserAccessViewModel>();

            using (var db = new AppDbContext())
            {
                var modules = (from m in db.Modules
                               join rm in db.RoleModules on m.Id equals rm.ModuleId
                               join r in db.Roles on rm.RoleId equals r.Id
                               select new
                               {
                                   ModuleName = m.Name,
                                   ModuleSlug = m.Slug,
                                   ModuleUrl = m.Url,
                                   RoleId = r.Id,
                                   RoleName = r.Name
                               }).ToList();

                foreach (var m in modules)
                {
                    foreach (var role in userRoles)
                    {
                        if (m.RoleName.Equals(role))
                        {
                            userAccess.Add(new UserAccessViewModel
                            {
                                ModuleName = m.ModuleName,
                                ModuleSlug = m.ModuleSlug,
                                ModuleUrl = !string.IsNullOrEmpty(m.ModuleUrl) ? m.ModuleUrl.Contains("~") ? m.ModuleUrl.Replace("~", "") : m.ModuleUrl : string.Empty
                            });
                        }
                    }
                }
            }

            return userAccess.Distinct().ToList();
        }

        /// <summary>
        /// Return true if user can access the module.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        public bool HasAccess(Guid id, string module)
        {
            var userAccess = GetAccess(id);
            var hasAccess = false;

            var user = this.FindById(id);
            if (user.IsSuperadmin())
            {
                return true;
            }

            foreach (var ua in userAccess)
            {
                if (ua.ModuleName.Equals(module.Trim()) || ua.ModuleSlug.Equals(module.Trim()) || ua.ModuleUrl.Equals(module.Trim()))
                {
                    hasAccess = true;
                    break;
                }
            }
            return hasAccess;
        }
        #endregion
    }
}