using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models.ViewModels
{
	public class UserListViewModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public string Roles { get; set; }
		public string IPMSRole { get; set; }
		public string CCMSRole { get; set; }
		public string IPMSBizArea { get; set; }
		public string CCMSBizArea { get; set; }
		public bool? UserHQ { get; set; }
    }

	public class UserAccessViewModel
	{
		public string ModuleName { get; set; }
		public string ModuleSlug { get; set; }
		public string ModuleUrl { get; set; }
	}

	public class UserModuleListViewModel
	{
		public Guid Id { get; set; }
		public Guid? ParentId { get; set; }
		public string Name { get; set; }
		public string Slug { get; set; }
		public string Url { get; set; }
		public string Icon { get; set; }
	}
}