using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.T1C.PO.Upload
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Bind data on initial load
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

            // Get all forms with status approved and completed
            var data = new Class.Form().GetForms(bizAreaCodes: ipmsBizAreaCodes, statuses: new List<string> { "Approved", "Completed" });

            // Get selected status
            var selectedStatus = ddlStatus.SelectedValue;

            // Filter data based on selected status
            if (!string.IsNullOrEmpty(selectedStatus))
            {
                data = data.Where(d => d.Status != null && d.Status.Equals(selectedStatus, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            gvData.DataSource = data;
            gvData.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
            gvData.DataBind();
        }
    }
}