using FGV.Prodata.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Administration.User
{
	public partial class Add : ProdataPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				BindControl();
			}
		}

        protected void btnSave_Click(object sender, EventArgs e)
        {
			if (IsValid)
			{
				var userManager = Context.GetOwinContext().GetUserManager<UserManager>();

				bool isSuccess = false;
				string name = txtName.Text.Trim();
				string email = txtEmail.Text.Trim();
				string username = txtUsername.Text.Trim().ToLower();
				string password = txtPassword.Text.Trim();
				string ipmsRole = ddlIPMSRole.SelectedValue;
				string ipmsBizArea = ddlIPMSBizArea.SelectedValue;


                if (!userManager.UserExists(username, email))
				{
					using (var db = new AppDbContext())
					{
						using (var trans = db.Database.BeginTransaction())
						{
							try
							{
								var user = new Models.Auth.User
								{
									Name = name,
									Email = email,
									UserName = username,
                                    iPMSRoleCode = ipmsRole,
                                    iPMSBizAreaCode = ipmsBizArea
                                };
								var result = userManager.Create(user, password);
								if (result.Succeeded)
								{
									for (int i = 0; i < cblRoles.Items.Count; i++)
									{
										if (cblRoles.Items[i].Selected)
										{
											userManager.AddToRole(user.Id, cblRoles.Items[i].Text.Trim());
										}
									}

									trans.Commit();
									isSuccess = true;
								}
								else
								{
									var passwordErrors = result.Errors.Where(p =>
										p.ToLower().Contains("password") ||
										p.ToLower().Contains("characters") ||
										p.ToLower().Contains("uppercase") ||
										p.ToLower().Contains("digit") ||
										p.ToLower().Contains("special character")
									).ToList();

									if (passwordErrors.Any())
									{
										lblPasswordErrors.Text = string.Join(" ", passwordErrors);
										lblPasswordErrors.Visible = true;
									}
									else
									{
										lblNameErrors.Text = string.Join(" ", result.Errors);
										lblNameErrors.Visible = true;
									}
								}
							}
							catch (Exception ex)
							{
								trans.Rollback();
								SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
								Response.Redirect("/Administration/User/Add");
							}
						}
					}
				}
				else
				{
					SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "User already exists.");
                    Response.Redirect("/Administration/User/Add");
                }

				if (isSuccess)
				{
					SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "New user added.");
					Response.Redirect("/Administration/User");
				}
			}
        }

		private void BindControl()
		{
			var roleManager = Context.GetOwinContext().Get<RoleManager>();

			cblRoles.DataSource = roleManager.GetRoles();
			cblRoles.DataValueField = "Id";
			cblRoles.DataTextField = "Name";
			cblRoles.DataBind();

			ddlIPMSRole.DataSource = new Class.IPMSRole().GetIPMSRoles();
            ddlIPMSRole.DataValueField = "Code";
            ddlIPMSRole.DataTextField = "DisplayName";
            ddlIPMSRole.DataBind();
            ddlIPMSRole.Items.Insert(0, new ListItem("", ""));

			ddlIPMSBizArea.DataSource = new Class.IPMSBizArea().GetIPMSBizAreas();
			ddlIPMSBizArea.DataValueField = "Code";
			ddlIPMSBizArea.DataTextField = "DisplayName";
			ddlIPMSBizArea.DataBind();
			ddlIPMSBizArea.Items.Insert(0, new ListItem("", ""));
        }
    }
}