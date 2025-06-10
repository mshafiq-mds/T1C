using Microsoft.Ajax.Utilities;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using CustomGuid.AspNet.Identity;

namespace Prodata.WebForm.Class
{
	public class Module
	{
		public List<Models.System.Module> GetModules(Guid? parentId = null, bool? isActive = null, bool? isMenu = null)
		{
			using (var db = new AppDbContext())
			{
				var query = from m in db.Modules select m;

				if (parentId.HasValue)
					query = query.Where(q => q.ParentId == parentId);
				else
					query = query.Where(q => !q.ParentId.HasValue);

				if (isActive.HasValue)
					query = query.Where(q => q.IsActive == isActive);

				if (isMenu.HasValue)
					query = query.Where(q => q.IsMenu == isMenu);

				return query.AsQueryable()
					.OrderBy(q => q.Order)
					.ThenBy(q => q.Name)
					.ToList();
			}
		}

		public List<UserModuleListViewModel> GetModules(Guid userId, Guid? parentId = null, bool? isActive = null, bool? isMenu = null)
		{
            var dt = new DataTable();
            var userModules = new List<UserModuleListViewModel>();

            try
            {
                using (var db = new AppDbContext()) // Ensures disposal of DbContext
                using (var con = new SqlConn().GetSqlConnection()) // Ensures disposal of SQL connection
                {
                    StringBuilder sql = new StringBuilder();
                    using (SqlCommand cmd = new SqlCommand())
                    using (SqlDataAdapter da = new SqlDataAdapter())
                    {
                        // Get user
                        var user = db.Users.FirstOrDefault(u => u.Id == userId);
                        bool isSuperadmin = user != null && user.IsSuperadmin(); // Handle null user case

                        // Build SQL query
                        sql.AppendLine("SELECT DISTINCT m.Id, m.ParentId, m.Name, m.Slug, m.Url, m.Icon, m.[Order]");
                        sql.AppendLine("FROM Modules m");

                        if (!isSuperadmin)
                        {
                            sql.AppendLine("INNER JOIN RoleModules ra ON ra.ModuleId = m.Id");
                            sql.AppendLine("INNER JOIN UserRoles ur ON ur.RoleId = ra.RoleId AND ur.UserId = @UserId");
                            sql.AppendLine("INNER JOIN Roles r ON r.Id = ur.RoleId");
                        }

                        sql.AppendLine(parentId.HasValue ? "WHERE m.ParentId = @ParentId" : "WHERE m.ParentId IS NULL");

                        if (isActive.HasValue) sql.AppendLine("AND m.IsActive = @IsActive");
                        if (isMenu.HasValue) sql.AppendLine("AND m.IsMenu = @IsMenu");

                        sql.AppendLine("ORDER BY m.[Order], m.Name");

                        // Configure command
                        cmd.CommandText = sql.ToString();
                        cmd.Connection = con;

                        if (!isSuperadmin) cmd.Parameters.AddWithValue("@UserId", userId);
                        if (parentId.HasValue) cmd.Parameters.AddWithValue("@ParentId", parentId.Value);
                        if (isActive.HasValue) cmd.Parameters.AddWithValue("@IsActive", isActive.Value ? 1 : 0);
                        if (isMenu.HasValue) cmd.Parameters.AddWithValue("@IsMenu", isMenu.Value ? 1 : 0);

                        da.SelectCommand = cmd;

                        // Execute query
                        con.Open();
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw; // Preserves the original stack trace
            }

            // Process DataTable
            foreach (DataRow row in dt.Rows)
            {
                userModules.Add(new UserModuleListViewModel
                {
                    Id = Guid.Parse(row["Id"].ToString()),
                    ParentId = !string.IsNullOrEmpty(row["ParentId"].ToString()) ? Guid.Parse(row["ParentId"].ToString()) : Guid.Empty,
                    Name = row["Name"].ToString(),
                    Slug = row["Slug"].ToString(),
                    Url = row["Url"].ToString(),
                    Icon = row["Icon"].ToString()
                });
            }

            return userModules;
        }

		public Models.System.Module Get(string param)
		{
			using (var db = new AppDbContext())
			{
				var module = new Models.System.Module();
				if (Guid.TryParse(param, out Guid parsedGuid))
				{
					module = db.Modules.FirstOrDefault(m => m.Id == parsedGuid);
				}
				else
				{
					module = db.Modules.FirstOrDefault(m =>
						m.Name.Replace(" ", "").Equals(param.Replace(" ", "")) ||
						m.Slug.ToLower().Equals(param.ToLower()));
				}

				if (module == null)
					module = GetByUrl(param);

				return module;
			}
		}

		public string GetName(string param)
		{
			return Get(param) != null ? Get(param).Name : string.Empty;
		}

		private Models.System.Module GetByUrl(string url)
		{
			using (var db = new AppDbContext())
			{
				if (!FGV.Prodata.App.Setting.AppPath().Equals("/"))
					url = url.Replace(FGV.Prodata.App.Setting.AppPath(), "");
                if (url.Contains("Default")) 
					url.Replace("Default", "");
                if (url.Contains("default")) 
					url.Replace("default", "");
                if (url.Contains(".aspx")) 
					url.Replace(".aspx", "");

				return db.Modules.FirstOrDefault(m => m.Url.Replace("~", "").Equals(url));
            }
		}

		public bool IsParent(Guid id, string page)
		{
			var module = Get(page);
			if (module != null)
			{
				if (module.ParentId.HasValue)
				{
					if (module.ParentId.Equals(id))
						return true;
					else
					{
						var parentId = module.ParentId;
						module = Get(parentId.Value.ToString());
						if (module != null)
						{
							if (module.ParentId.HasValue && module.ParentId.Equals(id))
								return true;
						}
					}
				}
			}
			return false;
		}

		public bool HasChildren(Guid id, bool? isMenu = null)
		{
			using (var db = new AppDbContext())
			{
				if (isMenu.HasValue)
					return db.Modules.FirstOrDefault(m => m.ParentId.HasValue && m.ParentId.Value == id && m.IsMenu == isMenu) != null;
				else
					return db.Modules.FirstOrDefault(m => m.ParentId.HasValue && m.ParentId.Value == id) != null;
			}
		}

		public bool HasParent(string param)
		{
			return !string.IsNullOrEmpty(param)
				? Get(param) != null
					? !string.IsNullOrEmpty(Get(param).ParentId.ToString())
						? true
						: false
					: false
				: false;
		}

		public Models.System.Module GetParent(string param)
		{
			if (HasParent(param))
			{
				Guid parentId = Get(param) != null ? Get(param).ParentId.Value : Guid.Empty;
				if (parentId != Guid.Empty)
					return Get(parentId.ToString()) ?? new Models.System.Module();
			}
			return new Models.System.Module();
		}
	}
}