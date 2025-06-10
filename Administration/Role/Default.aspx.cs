using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.Auth;
using Prodata.WebForm.Models.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Administration.Role
{
	public partial class Default : ProdataPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            btnAdd.Visible = Auth.User().Can("admin-role-add");

            if (!IsPostBack)
            {
                BindData();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.Url.GetCurrentUrl() + "/Manage?Action=Add");
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)((LinkButton)sender).NamingContainer;
            string roleId = ((HiddenField)row.FindControl("hdnRoleId")).Value;
            Response.Redirect(Request.Url.GetCurrentUrl() + "/Manage?Action=Edit&Id=" + roleId);
        }

        protected void btnDeleteRecord_Click(object sender, EventArgs e)
        {
            string roleId = hdnRecordId.Value;
            var roleManager = Context.GetOwinContext().Get<RoleManager>();
            var role = roleManager.FindById(Guid.Parse(roleId));

            try
            {
                if (role != null)
                {
                    using (var db = new AppDbContext())
                    {
                        var permissions = db.RoleModules.Where(p => p.RoleId == role.Id);
                        if (permissions.Any())
                        {
                            db.RoleModules.RemoveRange(permissions);
                            db.SaveChanges(); // Save all changes at once
                        }
                    }

                    // Delete the role after closing DbContext
                    var result = roleManager.Delete(role);
                    if (result.Succeeded)
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Info, "Role has been deleted.");
                    }
                    else
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", result.Errors));
                    }
                }
                else
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Role not found.");
                }
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
            }
            Response.Redirect(Request.Url.GetCurrentUrl());
        }

        protected void gvRole_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["pageIndex"] = e.NewPageIndex.ToString();
            BindData();
        }

        private void BindData()
		{
            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            var roleManager = Context.GetOwinContext().Get<RoleManager>();
            var roleList = roleManager.GetRoles();
            
            gvRole.DataSource = roleList;
            gvRole.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
            gvRole.DataBind();
        }
    }
}