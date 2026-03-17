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
                btnAddNew.Visible = Auth.User().Can("asset-write-off-add");
                PopulateYearDropdown();
                BindGrid();
            }
        }

        // ==========================================
        // NEW: Populate Year Dropdown (Current + 3 years back)
        // ==========================================
        private void PopulateYearDropdown()
        {
            int currentYear = DateTime.Now.Year;

            ddlYear.Items.Clear();
            ddlYear.Items.Add(new ListItem("All Years", "All")); // Optional catch-all

            for (int i = 0; i <= 3; i++)
            {
                string yearString = (currentYear - i).ToString();
                ddlYear.Items.Add(new ListItem(yearString, yearString));
            }

            // Set default value to the current year
            ddlYear.SelectedValue = currentYear.ToString();
        }

        private void BindGrid()
        {
            using (var db = new AppDbContext())
            {
                string searchKeyword = txtSearch.Text.Trim().ToLower();
                string statusFilter = ddlStatus.SelectedValue;
                string yearFilter = ddlYear.SelectedValue;

                // 1. Get the current user's ID 
                string currentUserBA = Auth.User().CCMSBizAreaCode;

                var query = db.AssetWriteOffs.AsQueryable();

                // 2. Filter so users ONLY see their own requests
                if (!string.IsNullOrEmpty(currentUserBA))
                    query = query.Where(x => x.BACode == currentUserBA);

                // 3. Filter by Year
                if (yearFilter != "All" && int.TryParse(yearFilter, out int selectedYear))
                {
                    query = query.Where(x => x.Date.Year == selectedYear);
                }

                // 4. Filter by Search Keyword
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    query = query.Where(x => x.RequestNo.ToLower().Contains(searchKeyword) ||
                                             x.Project.ToLower().Contains(searchKeyword));
                }

                // 5. Filter by Status
                if (!string.IsNullOrEmpty(statusFilter))
                {
                    if (statusFilter == "Pending")
                    {
                        query = query.Where(x => x.Status == "Pending" || x.Status == "Submitted");
                    }
                    else if (statusFilter == "Rejected")
                    {
                        query = query.Where(x => x.Status == "Rejected" || x.Status == "Deleted");
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
                                      x.Status,
                                      x.NextApprover
                                  })
                                  .OrderByDescending(x => x.Date)
                                  .ThenByDescending(x => x.RequestNo)
                                  .ToList();

                gvAWO.DataSource = result;
                gvAWO.DataBind();
            }
        }

        protected void btnProcessDelete_Click(object sender, EventArgs e)
        {
            Guid writeOffId;
            if (!Guid.TryParse(hfDeleteId.Value, out writeOffId)) return;

            string remark = hfDeleteRemark.Value.Trim();

            using (var db = new AppDbContext())
            {
                var master = db.AssetWriteOffs.Find(writeOffId);

                // Only allow deletion if the status is exactly "Submitted"
                if (master != null && master.Status == "Submitted")
                {
                    // 1. Update Master
                    master.Status = "Deleted";
                    master.DeletedBy = Auth.User().Id;
                    master.DeletedDate = DateTime.Now;

                    // 2. Insert Audit Log
                    var log = new AssetWriteOffApprovalLog
                    {
                        Id = Guid.NewGuid(),
                        WriteOffId = master.Id,
                        StepNumber = master.CurrentApprovalLevel,
                        RoleName = Auth.User().CCMSRoleCode,
                        UserId = Auth.User().Id,
                        ActionType = "Delete",
                        ActionDate = DateTime.Now,
                        Status = "Deleted",
                        Remarks = string.IsNullOrEmpty(remark) ? "Deleted by creator" : remark,
                        CreatedBy = Auth.User().Id,
                        CreatedDate = DateTime.Now
                    };

                    db.AssetWriteOffApprovalLogs.Add(log);
                    db.SaveChanges();

                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Request has been successfully deleted.");

                    // Rebind grid to hide the deleted item
                    BindGrid();
                }
                else
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "This request cannot be deleted in its current state.");
                }
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

            // Reset Year to Current Year on Clear
            ddlYear.SelectedValue = DateTime.Now.Year.ToString();

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
                LinkButton btnDelete = (LinkButton)e.Row.FindControl("btnDelete");

                if (lblStatusBadge != null)
                {
                    string status = lblStatusBadge.Text.Trim().ToLower();
                    string cssClass = "badge badge-custom ";

                    // ==========================================
                    // BUTTON VISIBILITY LOGIC
                    // ==========================================
                    if (status == "sentback" || status == "clarification")
                    {
                        if (hlEdit != null) hlEdit.Visible = true;
                        if (hlView != null) hlView.Visible = false;
                    }

                    if (status == "submitted")
                    {
                        if (btnDelete != null) btnDelete.Visible = true;
                    }

                    // ==========================================
                    // BADGE STYLING LOGIC
                    // ==========================================
                    switch (status)
                    {
                        case "submitted":
                            cssClass += "badge-soft-info";
                            lblStatusBadge.Text = "New Approval";
                            break;

                        case "pending":
                            cssClass += "badge-soft-primary";
                            lblStatusBadge.Text = "Pending Approval";
                            break;

                        case "sentback":
                        case "clarification":
                            cssClass += "badge-soft-warning";
                            lblStatusBadge.Text = "Sent Back";
                            break;

                        case "approved":
                            cssClass += "badge-soft-success";
                            lblStatusBadge.Text = "Approved";
                            break;

                        case "rejected":
                            cssClass += "badge-soft-danger";
                            lblStatusBadge.Text = "Rejected";
                            break;

                        case "deleted":
                            cssClass += "badge-soft-secondary";
                            lblStatusBadge.Text = "Deleted";
                            break;

                        default:
                            cssClass += "badge-soft-secondary";
                            lblStatusBadge.Text = "Unknown";
                            break;
                    }
                    //switch (status)
                    //{
                    //    case "submitted":
                    //        cssClass += "badge-primary";
                    //        lblStatusBadge.Text = "New Approval";
                    //        break;
                    //    case "pending":
                    //        cssClass += "badge-primary";
                    //        lblStatusBadge.Text = "Pending Approval";
                    //        break;
                    //    case "sentback":
                    //    case "clarification":
                    //        cssClass += "badge-warning";
                    //        lblStatusBadge.Text = "Sent Back";
                    //        break;
                    //    case "approved":
                    //        cssClass += "badge-success";
                    //        lblStatusBadge.Text = "Approved";
                    //        break;
                    //    case "rejected":
                    //        cssClass += "badge-danger";
                    //        lblStatusBadge.Text = "Rejected";
                    //        break;
                    //    case "deleted":
                    //        cssClass += "badge-dark";
                    //        lblStatusBadge.Text = "Deleted";
                    //        break;
                    //    default:
                    //        cssClass += "badge-secondary";
                    //        break;
                    //}

                    lblStatusBadge.CssClass = cssClass;
                }
            }
        }
    }
}