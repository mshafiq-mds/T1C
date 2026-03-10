using FGV.Prodata.Web.UI; // Assuming you are using ProdataPage and SweetAlert
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.ModelAWO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.AssetWriteOff.Approver
{
    public partial class Default : ProdataPage
    {
        private string CurrentUserRole => Auth.User().CCMSRoleCode;
        private string CurrentUserBA => Auth.User().CCMSBizAreaCode;

        // Cache the rules in memory to prevent N+1 DB Queries
        private List<AssetWriteOffApprovalLimit> _userLimits;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void BindGrid()
        {
            // 1. Evaluate the user BA outside of the LINQ query to prevent the NotSupportedException
            string currentUserBA = Auth.User().CCMSBizAreaCode;

            // Assuming CurrentUserRole is also a property calling Auth.User(), evaluate it here safely too:
            string currentUserRole = Auth.User().CCMSRoleCode;

            List<string> accessibleBizAreas = !string.IsNullOrEmpty(currentUserBA)
                ? new Class.IPMSBizArea().GetBizAreaCodes(currentUserBA)
                : new List<string>();

            using (var db = new AppDbContext())
            {
                // Use the evaluated string variables
                _userLimits = db.AssetWriteOffApprovalLimits.Where(x => x.AWOApproverCode == currentUserRole).ToList();

                var query = db.AssetWriteOffs.AsQueryable();

                // 2. Corrected Hierarchy Filtering Logic
                if (!string.IsNullOrEmpty(currentUserBA))
                {
                    if (accessibleBizAreas.Any())
                    {
                        // If the user is a Zone/Wilayah manager, show all matching child mills
                        query = query.Where(x => accessibleBizAreas.Contains(x.BACode));
                    }
                    else
                    {
                        // Otherwise, restrict them to just their exact BA
                        query = query.Where(x => x.BACode == currentUserBA);
                    }
                }

                // 3. Apply Status Filters
                string selectedStatus = ddlStatusFilter.SelectedValue;
                if (selectedStatus == "Pending")
                {
                    query = query.Where(r => r.Status == "Pending" || r.Status == "Submitted");
                }
                else if (selectedStatus != "All")
                {
                    query = query.Where(r => r.Status == selectedStatus);
                }

                var requests = query.OrderByDescending(r => r.CreatedDate).ToList();

                gvRequests.DataSource = requests;
                gvRequests.DataBind();
            }
        }

        protected void gvRequests_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HyperLink hlReview = (HyperLink)e.Row.FindControl("hlReview");
                HyperLink hlViewOnly = (HyperLink)e.Row.FindControl("hlViewOnly");

                var request = (Models.ModelAWO.AssetWriteOff)e.Row.DataItem;

                bool isPending = (request.Status == "Pending" || request.Status == "Submitted");
                bool canApprove = false;

                if (isPending)
                { 
                    canApprove = Class.AWOHelper.CheckIfUserCanApprove(request, _userLimits);
                }

                if (canApprove)
                {
                    hlReview.Visible = true;
                    hlViewOnly.Visible = false;
                }
                else
                {
                    hlReview.Visible = false;
                    hlViewOnly.Visible = true;
                }
            }
        }
         
    }
}