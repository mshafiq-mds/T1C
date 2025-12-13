using CustomGuid.AspNet.Identity;
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
	public partial class Edit : ProdataPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				string userId = string.Empty;
				if (Request.QueryString["Id"] != null)
				{
					userId = Request.QueryString["Id"].ToString();
					hdnUserId.Value = userId;
				}
				else
				{
					Response.Redirect("~/Administration/User");
				}

				BindControl();
				BindData(userId);
			}
		}

        protected void btnSave_Click(object sender, EventArgs e)
        {
			if (IsValid)
			{
				var userManager = Context.GetOwinContext().GetUserManager<UserManager>();

				bool isSuccess = false;
				bool isSuccessChangePassword = false;
				string userId = hdnUserId.Value;
				Guid guidUserId = Guid.Parse(userId);
				string name = txtName.Text.Trim();
				string email = txtEmail.Text.Trim();
				string username = txtUsername.Text.Trim().ToLower();
				string password = !string.IsNullOrEmpty(txtPassword.Text.Trim()) ? txtPassword.Text.Trim() : string.Empty;
                string ipmsRole = ddlIPMSRole.SelectedValue;
                string ccmsRole = ddlCCMSRole.SelectedValue;
				string ipmsBizArea = ddlIPMSBizArea.SelectedValue;
				string ccmsBizArea = ddlCCMSBizArea.SelectedValue;

                if (!userManager.UserExists(guidUserId, username, email))
				{
					using (var db = new AppDbContext())
					{
						using (var trans = db.Database.BeginTransaction())
						{
							try
							{
								var user = userManager.FindById(guidUserId);
								var oldPasswordHash = user.PasswordHash;

								if (!string.IsNullOrEmpty(password))
								{
									userManager.RemovePassword(guidUserId);
									var resultP = userManager.AddPassword(guidUserId, password);
									if (resultP.Succeeded)
									{
										isSuccessChangePassword = true;
									}
									else
									{
										user.PasswordHash = oldPasswordHash;
										userManager.Update(user);
										lblPasswordErrors.Text = string.Join(" ", resultP.Errors);
										lblPasswordErrors.Visible = true;
									}
								}

								if (string.IsNullOrEmpty(password) || isSuccessChangePassword)
								{
									user.Name = name;
									user.Email = email;
									user.UserName = username;
									user.iPMSRoleCode = ipmsRole;
									user.CCMSRoleCode = ccmsRole;
									user.CCMSBizAreaCode = ccmsBizArea;
									user.iPMSBizAreaCode = ipmsBizArea;

                                    var result = userManager.Update(user);
									if (result.Succeeded)
									{
										for (int i = 0; i < cblRoles.Items.Count; i++)
										{
											if (cblRoles.Items[i].Selected)
											{
												if (!userManager.IsInRole(user.Id, cblRoles.Items[i].Text.Trim()))
												{
													userManager.AddToRole(user.Id, cblRoles.Items[i].Text.Trim());
												}
											}
											else
											{
												if (userManager.IsInRole(user.Id, cblRoles.Items[i].Text.Trim()))
												{
													userManager.RemoveFromRole(user.Id, cblRoles.Items[i].Text.Trim());
												}
											}
										}

										trans.Commit();
										isSuccess = true;
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
								Response.Redirect("~/Administration/User/Edit?Id=" + userId);
							}
						}
					}
				}
				else
				{
					SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "User already exists.");
                    Response.Redirect("~/Administration/User/Edit?Id=" + userId);
                }

				if (isSuccess)
				{
					SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "User updated.");
					Response.Redirect("~/Administration/User");
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

            ddlCCMSRole.DataSource = new Class.IPMSRole().GetIPMSRoles();
            ddlCCMSRole.DataValueField = "Code";
            ddlCCMSRole.DataTextField = "DisplayName";
            ddlCCMSRole.DataBind();
            ddlCCMSRole.Items.Insert(0, new ListItem("", ""));

            ddlIPMSBizArea.DataSource = new Class.IPMSBizArea().GetIPMSBizAreas();
            ddlIPMSBizArea.DataValueField = "Code";
            ddlIPMSBizArea.DataTextField = "DisplayName";
            ddlIPMSBizArea.DataBind();
            ddlIPMSBizArea.Items.Insert(0, new ListItem("", ""));

            ddlCCMSBizArea.DataSource = new Class.IPMSBizArea().GetIPMSBizAreas();
            ddlCCMSBizArea.DataValueField = "Code";
            ddlCCMSBizArea.DataTextField = "DisplayName";
            ddlCCMSBizArea.DataBind();
            ddlCCMSBizArea.Items.Insert(0, new ListItem("", ""));
        }

		private void BindData(string userId = null)
		{
			var userManager = Context.GetOwinContext().GetUserManager<UserManager>();

			if (!string.IsNullOrEmpty(userId))
			{
				var guidUserId = Guid.Parse(userId);
				var user = userManager.FindById(guidUserId);
				var userRoles = userManager.GetRoles(guidUserId);

				txtName.Text = user.Name;
				txtEmail.Text = user.Email;
				txtUsername.Text = user.UserName;
				ddlIPMSRole.SelectedValue = user.iPMSRoleCode;
				ddlCCMSRole.SelectedValue = user.CCMSRoleCode;
				ddlIPMSBizArea.SelectedValue = user.iPMSBizAreaCode;
				ddlCCMSBizArea.SelectedValue = user.CCMSBizAreaCode;

                if (user.IsSuperadmin())
					txtUsername.Attributes.Add("readonly", "readonly");

				if (!userManager.HasPassword(guidUserId))
				{
                    txtUsername.Attributes.Add("readonly", "readonly");
					txtPassword.Attributes.Add("readonly", "readonly");
					txtConfirmPassword.Attributes.Add("readonly", "readonly");
					ddlIPMSBizArea.Attributes.Add("readonly", "readonly");
					ddlIPMSRole.Attributes.Add("readonly", "readonly");
                }

				for (int i = 0; i < cblRoles.Items.Count; i++)
				{
					for (int j = 0; j < userRoles.Count; j++)
					{
						if (cblRoles.Items[i].Text.Trim().Equals(userRoles[j]))
						{
							cblRoles.Items[i].Selected = true;
						}
					}
				}
            }
		}
    }
}