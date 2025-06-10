using CustomGuid.AspNet.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Account
{
	public partial class Profile : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			var userManager = Context.GetOwinContext().GetUserManager<UserManager>();
			var user = userManager.FindById(Context.User.Identity.GetUserID());
            if (!IsPostBack)
			{
				lblName.Text = user.Name;
				lblEmail.Text = user.Email;
			}
		}
	}
}