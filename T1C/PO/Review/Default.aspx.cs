using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.T1C.PO.Review
{
    public partial class Default : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                BindData(Auth.CCMSBizAreaCodes());
                // Check if there is a message waiting in Session
                if (Session["SweetAlertMessage"] != null)
                {
                    string msg = Session["SweetAlertMessage"].ToString();

                    // Show the alert on this new page
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, msg);

                    // Important: Clear the session so it doesn't show again on refresh
                    Session["SweetAlertMessage"] = null;
                    Session["SweetAlertType"] = null;
                }
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

            // Get all forms
            var data = new Class.Form().GetForms(bizAreaCodes: CCMSBizAreaCodes);

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