using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.T1C.PO.Upload
{
    public partial class Default : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Bind data on initial load
                BindData(Auth.CCMSBizAreaCodes());
            }
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindData(Auth.CCMSBizAreaCodes());
        }

        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["pageIndex"] = e.NewPageIndex.ToString();
            BindData(Auth.CCMSBizAreaCodes());
        }

        private void BindData(List<string> CCMSBizAreaCodes = null)
        {
            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            // Get all forms with status approved and completed
            var data = new Class.Form().GetForms(bizAreaCodes: CCMSBizAreaCodes, statuses: new List<string> { "Approved", "Completed" });

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

        protected bool IsPoReviewed(object id)
        {
            // 1. Safety check for null
            if (id == null) return false;

            // 2. Parse the GridView ID (object) to a Guid
            // Note: In your Review.aspx.cs, you are using Guid, so we must use Guid here too.
            if (!Guid.TryParse(id.ToString(), out Guid formId)) return false;

            // 3. Query the database
            using (var db = new AppDbContext())
            {
                // We check if ANY record exists that matches the Form ID and the specific Action
                bool isReviewed = db.Approvals.Any(a =>
                    a.ObjectId == formId &&
                    a.ObjectType == "Form" &&
                    a.Action == "Reviewed PO"
                );

                return isReviewed;
            }
        }
    }
}