using FGV.Prodata.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.T1C.PoolBudget.Approval
{
    public partial class Default : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData(Auth.IPMSBizAreaCodes());
            }
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindData(Auth.IPMSBizAreaCodes());
        }

        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["pageIndex"] = e.NewPageIndex.ToString();
            BindData(Auth.IPMSBizAreaCodes());
        }

        private void BindData(List<string> ipmsBizAreaCodes = null)
        {
            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            // Get all forms
            var data = new Class.Form().GetFormsProcurement(bizAreaCodes: ipmsBizAreaCodes);

            // Get selected status
            var selectedStatus = ddlStatus.SelectedValue;

            // If "Pending My Action" selected
            if (selectedStatus == "pending-my-action")
            {
                data = data.Where(d => d.IsPendingUserAction).ToList();
            }
            // Else if any specific status selected (like "Pending", "Approved", etc.)
            else if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                data = data.Where(d =>
                    !d.IsPendingUserAction &&
                    d.Status != null &&
                    d.Status.Equals(selectedStatus, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }
            // else (empty) = no filtering, show all

            gvData.DataSource = data;
            gvData.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
            gvData.DataBind();
        }
    }
}