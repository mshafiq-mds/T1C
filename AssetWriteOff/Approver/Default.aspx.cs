using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.ModelAWO;
using Prodata.WebForm.T1C.PO.Review;
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

        private List<AssetWriteOffApprovalLimit> _userLimits;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateYearDropdown();
                BindGrid();
            }
        }

        // Generate the Year options (Current year + 3 years back)
        private void PopulateYearDropdown()
        {
            int currentYear = DateTime.Now.Year;

            ddlYear.Items.Clear();
            ddlYear.Items.Add(new ListItem("All Years", "All"));

            for (int i = 0; i <= 3; i++)
            {
                string yearStr = (currentYear - i).ToString();
                ddlYear.Items.Add(new ListItem(yearStr, yearStr));
            }

            // Default to the current year
            ddlYear.SelectedValue = currentYear.ToString();
        }

        // Shared handler for both Year and Status dropdowns
        protected void ddlFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void BindGrid()
        {
            string currentUserBA = Auth.User().CCMSBizAreaCode;
            string currentUserRole = Auth.User().CCMSRoleCode;

            List<string> accessibleBizAreas = !string.IsNullOrEmpty(currentUserBA)
                ? new Class.IPMSBizArea().GetBizAreaCodes(currentUserBA)
                : new List<string>();

            using (var db = new AppDbContext())
            {
                _userLimits = db.AssetWriteOffApprovalLimits.Where(x => x.AWOApproverCode == currentUserRole).ToList();

                var query = db.AssetWriteOffs.AsQueryable();

                // Hierarchy Filtering Logic
                if (!string.IsNullOrEmpty(currentUserBA))
                {
                    if (accessibleBizAreas.Any())
                    {
                        query = query.Where(x => accessibleBizAreas.Contains(x.BACode));
                    }
                    else
                    {
                        query = query.Where(x => x.BACode == currentUserBA);
                    }
                }

                // APPLY YEAR FILTER
                string yearFilter = ddlYear.SelectedValue;
                if (yearFilter != "All" && int.TryParse(yearFilter, out int selectedYear))
                {
                    query = query.Where(x => x.Date.Year == selectedYear);
                }


                string selectedStatus = ddlStatusFilter.SelectedValue;

                // Fetch the list first
                List<Models.ModelAWO.AssetWriteOff> requests;

                if (selectedStatus == "MyAction")
                {
                    // 1. Get ALL pending items from DB first
                    var allPending = query.Where(r => r.Status == "Pending" || r.Status == "Submitted")
                                          .OrderByDescending(r => r.CreatedDate)
                                          .ToList();

                    // 2. Filter IN MEMORY to only show items this specific user can approve
                    requests = allPending.Where(r => Class.AWOHelper.CheckIfUserCanApprove(r, _userLimits)).ToList();
                }
                else if (selectedStatus == "Pending")
                {
                    requests = query.Where(r => r.Status == "Pending" || r.Status == "Submitted")
                                    .OrderByDescending(r => r.CreatedDate)
                                    .ToList();
                }
                else if (selectedStatus == "Deleted" || selectedStatus == "Rejected")
                {
                    requests = query.Where(r => r.Status == "Deleted" || r.Status == "Rejected")
                                    .OrderByDescending(r => r.CreatedDate)
                                    .ToList();
                }
                else if (selectedStatus != "All")
                {
                    requests = query.Where(r => r.Status == selectedStatus)
                                    .OrderByDescending(r => r.CreatedDate)
                                    .ToList();
                }
                else
                {
                    requests = query.OrderByDescending(r => r.CreatedDate).ToList();
                }

                gvRequests.DataSource = requests.OrderByDescending(x => x.Date).ThenByDescending(x => x.RequestNo);
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