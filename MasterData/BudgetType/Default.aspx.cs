using CustomGuid.AspNet.Identity;
using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.MasterData.BudgetType
{
	public partial class Default : ProdataPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            btnAdd.Visible = Auth.User().Can("budget-type-add");
            if (!IsPostBack)
            {
                BindData();
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)((LinkButton)sender).NamingContainer;
            string budgetTypeId = ((HiddenField)row.FindControl("hdnId")).Value;
            Response.Redirect(Request.Url.GetCurrentUrl() + "/Edit?Id=" + budgetTypeId);
        }

        protected void btnDeleteRecord_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var budgetType = db.BudgetTypes.Find(Guid.Parse(hdnRecordId.Value));
                    bool isSuccess = db.SoftDelete(budgetType);
                    if (isSuccess)
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Info, "Budget type deleted.");
                    }
                    else
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Failed to delete budget type.");
                    }
                }
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
            }

            Response.Redirect(Request.Url.GetCurrentUrl());
        }

        protected void gvBudgetType_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["pageIndex"] = e.NewPageIndex.ToString();
            BindData();
        }

        private void BindData()
        {
            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            var list = GetBudgetTypes();

            gvBudgetType.DataSource = list;
            gvBudgetType.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
            gvBudgetType.DataBind();
        }

        private List<Models.MasterData.BudgetType> GetBudgetTypes(string code = null, string name = null)
        {
            using (var db = new AppDbContext())
            {
                return db.BudgetTypes
                    .ExcludeSoftDeleted()
                    .Where(b => 
                        (string.IsNullOrEmpty(code) || b.Code.Contains(code)) && 
                        (string.IsNullOrEmpty(name) || b.Name.Contains(name)))
                    .OrderBy(b => b.Code)
                    .ToList();
            }
        }
    }
}