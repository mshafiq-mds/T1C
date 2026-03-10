using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.ModelAWO;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.AssetWriteOff
{
    public partial class Default : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            using (var db = new AppDbContext())
            {
                string searchKeyword = txtSearch.Text.Trim().ToLower();
                string statusFilter = ddlStatus.SelectedValue;

                // 1. Get the current user's ID 
                string currentUserBA = Auth.User().CCMSBizAreaCode;

                var query = db.AssetWriteOffs.AsQueryable();

                // 2. Filter so users ONLY see their own requests (or change to BACode based on your business rule)
                query = query.Where(x => x.BACode == currentUserBA);

                // 3. Filter by Search Keyword
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    query = query.Where(x => x.RequestNo.ToLower().Contains(searchKeyword) ||
                                             x.Project.ToLower().Contains(searchKeyword));
                }

                // 4. Filter by Status
                if (!string.IsNullOrEmpty(statusFilter))
                {
                    // To handle both "Pending" and "Submitted" in one filter selection
                    if (statusFilter == "Pending")
                    {
                        query = query.Where(x => x.Status == "Pending" || x.Status == "Submitted");
                    }
                    else
                    {
                        query = query.Where(x => x.Status == statusFilter);
                    }
                }

                var result = query.OrderByDescending(x => x.CreatedDate)
                                  .Select(x => new
                                  {
                                      x.Id,
                                      x.RequestNo,
                                      x.Date,
                                      x.BACode,
                                      x.Project,
                                      x.Status
                                  }).ToList();

                gvAWO.DataSource = result;
                gvAWO.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvAWO.PageIndex = 0;
            BindGrid();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            ddlStatus.SelectedIndex = 0;
            gvAWO.PageIndex = 0;
            BindGrid();
        }

        protected void gvAWO_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAWO.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvAWO_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblStatusBadge = (Label)e.Row.FindControl("lblStatusBadge");
                HyperLink hlView = (HyperLink)e.Row.FindControl("hlView");
                HyperLink hlEdit = (HyperLink)e.Row.FindControl("hlEdit");

                if (lblStatusBadge != null)
                {
                    string status = lblStatusBadge.Text.Trim().ToLower();
                    string cssClass = "badge badge-custom ";

                    // ==========================================
                    // EDIT BUTTON LOGIC
                    // ==========================================
                    if (status == "sentback" || status == "clarification")
                    {
                        if (hlEdit != null) hlEdit.Visible = true;
                        if (hlView != null) hlView.Visible = false; // Hide "View" so they are forced to "Edit"
                    }

                    // ==========================================
                    // BADGE STYLING LOGIC
                    // ==========================================
                    switch (status)
                    {
                        case "submitted":
                        case "pending":
                            cssClass += "badge-primary";
                            lblStatusBadge.Text = "Pending Approval";
                            break; 
                        case "sentback":
                            cssClass += "badge-warning";
                            lblStatusBadge.Text = "Sent Back"; // Clean up display text
                            break;
                        case "approved":
                            cssClass += "badge-success";
                            lblStatusBadge.Text = "Approved";
                            break;
                        case "rejected":
                            cssClass += "badge-danger";
                            lblStatusBadge.Text = "Rejected";
                            break;
                        default:
                            cssClass += "badge-secondary";
                            break;
                    }

                    lblStatusBadge.CssClass = cssClass;
                }
            }
        }
    }
}