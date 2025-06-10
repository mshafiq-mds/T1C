using CustomGuid.AspNet.Identity;
using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Administration.User
{
	public partial class Default : ProdataPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            btnAdd.Visible = Auth.User().Can("admin-user-add");

            if (!IsPostBack)
            {
                BindData();
            }
		}

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)((LinkButton)sender).NamingContainer;
            string userId = ((HiddenField)row.FindControl("hdnUserId")).Value;
            Response.Redirect(Request.Url.GetCurrentUrl() + "/Edit?Id=" + userId);
        }

        protected void btnDeleteRecord_Click(object sender, EventArgs e)
        {
            string userId = hdnRecordId.Value;
            var userManager = Context.GetOwinContext().GetUserManager<UserManager>();
            var user = userManager.FindById(Guid.Parse(userId));

            if (user.IsSuperadmin())
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Failed to delete superadmin.");
                Response.Redirect(Request.Url.GetCurrentUrl());
            }

            try
            {
                // Get the user's roles
                var roles = userManager.GetRoles(user.Id);
                if (roles.Count > 0)
                {
                    userManager.RemoveFromRoles(user.Id, roles.ToArray()); // Remove roles first
                }

                // Now delete the user
                var result = userManager.Delete(user);
                if (result.Succeeded)
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Info, "User has been deleted.");
                }
                else
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", result.Errors));
                }
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
            }

            Response.Redirect(Request.Url.GetCurrentUrl());
        }

        protected void gvUser_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["pageIndex"] = e.NewPageIndex.ToString();
            BindData();
        }

        private void BindData()
        {
            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            var userManager = Context.GetOwinContext().GetUserManager<UserManager>();
            var userList = userManager.GetUsers();

            gvUser.DataSource = userList;
            gvUser.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
            gvUser.DataBind();
        }
    }
}