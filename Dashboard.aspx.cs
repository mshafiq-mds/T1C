using FGV.Prodata.Web.UI;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization; // Required for Chart JSON

namespace Prodata.WebForm
{
    public partial class Dashboard : ProdataPage
    {
        // Simple class to hold data for one section
        private class DashboardCounts
        {
            public int Submitted { get; set; }
            public int Review { get; set; }
            public int Resubmit { get; set; }
            public int Complete { get; set; }
            public int Deleted { get; set; }
            public int Finalized { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Add class for styling
            Page.ClientScript.RegisterStartupScript(this.GetType(), "BodyClass",
                "<script>document.body.classList.add('dashboard-lock');</script>");

            if (!IsPostBack)
            {
                LoadBusinessAreas(); // Fix: This method is defined below
                LoadYears();         // Fix: This method is defined below
                LoadDashboardData(); // Loads cards and charts
            }
        }

        // ==========================================================
        // 1. DROPDOWN LOADING METHODS (Fixes your error)
        // ==========================================================
        private void LoadBusinessAreas()
        {
            try
            {
                var currentUserBA = Auth.User().iPMSBizAreaCode;
                var bizAreas = new Class.IPMSBizArea().GetIPMSBizAreas();

                BindDropdown(ddBA, bizAreas, "Code", "DisplayName");

                // Select user's BA by default if they have one
                if (!string.IsNullOrEmpty(currentUserBA) && ddBA.Items.FindByValue(currentUserBA) != null)
                {
                    ddBA.SelectedValue = currentUserBA;
                }
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error loading Business Areas: " + ex.Message);
            }
        }

        private void LoadYears()
        {
            int currentYear = DateTime.Now.Year;
            var years = new List<int>();

            // Range: 2 years back -> 1 year forward
            for (int i = currentYear - 2; i <= currentYear + 1; i++)
            {
                years.Add(i);
            }

            ddYear.DataSource = years;
            ddYear.DataBind();

            // Default to current year
            if (ddYear.Items.FindByValue(currentYear.ToString()) != null)
                ddYear.SelectedValue = currentYear.ToString();
        }

        // ==========================================================
        // 2. EVENT HANDLERS (Filters & Cards)
        // ==========================================================
        protected void btnClearBA_Click(object sender, EventArgs e)
        {
            ddBA.SelectedIndex = 0;
            LoadDashboardData();
        }

        protected void ddBA_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDashboardData();
        }

        protected void ddYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDashboardData();
        }

        protected void Card_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                string[] args = btn.CommandArgument.Split('|');

                if (args.Length != 2) return;

                string category = args[0];
                string statusType = args[1];

                // Store state for Search
                hfModalCategory.Value = category;
                hfModalStatus.Value = statusType;

                // Clear previous search
                txtSearchDetail.Text = string.Empty;

                lblModalTitle.Text = $"{category} - {statusType} List ({ddYear.SelectedValue})";

                // Load data into grid
                BindModalGrid(category, statusType);

                // Open Modal via JS
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openDetailModal();", true);
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error opening details: " + ex.Message);
            }
        }

        protected void btnSearchDetail_Click(object sender, EventArgs e)
        {
            // Retrieve state
            string category = hfModalCategory.Value;
            string status = hfModalStatus.Value;
            string keyword = txtSearchDetail.Text.Trim();

            // Reload grid with search filter
            BindModalGrid(category, status, keyword);

            // Keep modal open
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openDetailModal();", true);
        }

        // ==========================================================
        // 3. CORE LOGIC: Loading Dashboard Stats & Charts
        // ==========================================================
        private void LoadDashboardData()
        {
            string selectedBA = ddBA.SelectedValue;
            int selectedYear = int.TryParse(ddYear.SelectedValue, out int y) ? y : DateTime.Now.Year;

            using (var db = new AppDbContext())
            {
                try
                {
                    // --- A. Additional Budget ---
                    var addBudgetQuery = db.AdditionalBudgetRequests.Where(x => x.CreatedDate.Year == selectedYear);
                    if (!string.IsNullOrEmpty(selectedBA)) addBudgetQuery = addBudgetQuery.Where(x => x.BA == selectedBA);

                    var addStats = addBudgetQuery.GroupBy(x => 1).Select(g => new DashboardCounts
                    {
                        Deleted = g.Count(x => x.DeletedDate != null),
                        Submitted = g.Count(x => x.DeletedDate == null),
                        Review = g.Count(x => x.Status == 2),
                        Resubmit = g.Count(x => x.Status == 0),
                        Complete = g.Count(x => x.Status == 3),
                        Finalized = g.Count(x => x.Status == 4)
                    }).FirstOrDefault() ?? new DashboardCounts();

                    UpdateSection(addStats, LblAdditionalSubmitted, LblAdditionalReview, LblAdditionalResubmit, LblAdditionalComplete, LblAdditionalDeleted, LblAdditionalFinalized);

                    // --- B. Transfers ---
                    var transferQuery = db.TransfersTransaction.Where(x => x.CreatedDate.Year == selectedYear);
                    if (!string.IsNullOrEmpty(selectedBA)) transferQuery = transferQuery.Where(x => x.BA == selectedBA);

                    var transStats = transferQuery.GroupBy(x => 1).Select(g => new DashboardCounts
                    {
                        Deleted = g.Count(x => x.DeletedDate != null),
                        Submitted = g.Count(x => x.DeletedDate == null),
                        Review = g.Count(x => x.status == 2),
                        Resubmit = g.Count(x => x.status == 0),
                        Complete = g.Count(x => x.status == 3),
                        Finalized = g.Count(x => x.status == 4)
                    }).FirstOrDefault() ?? new DashboardCounts();

                    UpdateSection(transStats, LblTransferSubmitted, LblTransferReview, LblTransferResubmit, LblTransferComplete, LblTransferDeleted, LblTransferFinalized);

                    // --- C. T1C Forms ---
                    var formQuery = db.Forms.Where(x => x.CreatedDate.Year == selectedYear);
                    if (!string.IsNullOrEmpty(selectedBA)) formQuery = formQuery.Where(x => x.BizAreaCode == selectedBA);

                    var formStats = formQuery.GroupBy(x => 1).Select(g => new DashboardCounts
                    {
                        Deleted = g.Count(x => x.DeletedDate != null),
                        Submitted = g.Count(x => x.DeletedDate == null),
                        Review = g.Count(x => x.Status == "Pending"),
                        Resubmit = g.Count(x => x.Status == "SentBack"),
                        Complete = g.Count(x => x.Status == "Approved"),
                        Finalized = g.Count(x => x.Status == "Completed")
                    }).FirstOrDefault() ?? new DashboardCounts();

                    UpdateSection(formStats, LblT1CSubmitted, LblT1CReview, LblT1CResubmit, LblT1CComplete, LblT1CDeleted, LblT1CFinalized);

                    // --- D. Render Charts (Serialize Data for JS) ---
                    var t1cData = new int[] { formStats.Submitted, formStats.Review, formStats.Resubmit, formStats.Complete, formStats.Deleted, formStats.Finalized };
                    var addData = new int[] { addStats.Submitted, addStats.Review, addStats.Resubmit, addStats.Complete, addStats.Deleted, addStats.Finalized };
                    var transData = new int[] { transStats.Submitted, transStats.Review, transStats.Resubmit, transStats.Complete, transStats.Deleted, transStats.Finalized };

                    JavaScriptSerializer js = new JavaScriptSerializer();

                    // Call the JS function defined in ASPX with the data arrays
                    string script = $"renderDashboardCharts({js.Serialize(t1cData)}, {js.Serialize(addData)}, {js.Serialize(transData)});";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "InitCharts", script, true);

                }
                catch (Exception ex)
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error loading dashboard: " + ex.Message);
                }
            }
        }

        // ==========================================================
        // 4. HELPER METHODS
        // ==========================================================
        private void BindModalGrid(string category, string statusType, string searchKeyword = "")
        {
            string selectedBA = ddBA.SelectedValue;
            int selectedYear = int.TryParse(ddYear.SelectedValue, out int y) ? y : DateTime.Now.Year;

            using (var db = new AppDbContext())
            {
                if (category == "T1C")
                {
                    var query = db.Forms.Where(x => x.CreatedDate.Year == selectedYear);
                    if (!string.IsNullOrEmpty(selectedBA)) query = query.Where(x => x.BizAreaCode == selectedBA);

                    if (!string.IsNullOrEmpty(searchKeyword))
                        query = query.Where(x => x.Ref.Contains(searchKeyword));

                    switch (statusType)
                    {
                        case "Submitted": query = query.Where(x => x.DeletedDate == null); break;
                        case "Review": query = query.Where(x => x.Status == "Pending"); break;
                        case "Resubmit": query = query.Where(x => x.Status == "SentBack"); break;
                        case "Complete": query = query.Where(x => x.Status == "Approved"); break;
                        case "Deleted": query = query.Where(x => x.DeletedDate != null); break;
                        case "Finalized": query = query.Where(x => x.Status == "Completed"); break;
                    }

                    gvDetails.DataSource = query.Select(x => new {
                        RefNo = x.Ref,
                        Date = x.CreatedDate,
                        BA = x.BizAreaCode,
                        Status = x.Status,
                        Amount = x.Amount
                    }).OrderByDescending(x => x.Date).ToList();
                }
                else if (category == "Additional")
                {
                    var query = db.AdditionalBudgetRequests.Where(x => x.CreatedDate.Year == selectedYear);
                    if (!string.IsNullOrEmpty(selectedBA)) query = query.Where(x => x.BA == selectedBA);

                    if (!string.IsNullOrEmpty(searchKeyword))
                        query = query.Where(x => x.RefNo.Contains(searchKeyword));

                    switch (statusType)
                    {
                        case "Submitted": query = query.Where(x => x.DeletedDate == null); break;
                        case "Review": query = query.Where(x => x.Status == 2); break;
                        case "Resubmit": query = query.Where(x => x.Status == 0); break;
                        case "Complete": query = query.Where(x => x.Status == 3); break;
                        case "Deleted": query = query.Where(x => x.DeletedDate != null); break;
                        case "Finalized": query = query.Where(x => x.Status == 4); break;
                    }

                    gvDetails.DataSource = query.Select(x => new {
                        RequestNo = x.RefNo,
                        Date = x.CreatedDate,
                        BA = x.BA,
                        Status = x.Status
                    }).OrderByDescending(x => x.Date).ToList();
                }
                else if (category == "Transfer")
                {
                    var query = db.TransfersTransaction.Where(x => x.CreatedDate.Year == selectedYear);
                    if (!string.IsNullOrEmpty(selectedBA)) query = query.Where(x => x.BA == selectedBA);

                    if (!string.IsNullOrEmpty(searchKeyword))
                        query = query.Where(x => x.RefNo.Contains(searchKeyword));

                    switch (statusType)
                    {
                        case "Submitted": query = query.Where(x => x.DeletedDate == null); break;
                        case "Review": query = query.Where(x => x.status == 2); break;
                        case "Resubmit": query = query.Where(x => x.status == 0); break;
                        case "Complete": query = query.Where(x => x.status == 3); break;
                        case "Deleted": query = query.Where(x => x.DeletedDate != null); break;
                        case "Finalized": query = query.Where(x => x.status == 4); break;
                    }

                    gvDetails.DataSource = query.Select(x => new {
                        RefNo = x.RefNo,
                        Date = x.CreatedDate,
                        BA = x.BA,
                        Status = x.status
                    }).OrderByDescending(x => x.Date).ToList();
                }

                gvDetails.DataBind();
            }
        }

        private void UpdateSection(DashboardCounts stats, Label submit, Label review, Label resubmit, Label complete, Label deleted, Label finalized)
        {
            submit.Text = stats.Submitted.ToString();
            review.Text = stats.Review.ToString();
            resubmit.Text = stats.Resubmit.ToString();
            complete.Text = stats.Complete.ToString();
            deleted.Text = stats.Deleted.ToString();
            finalized.Text = stats.Finalized.ToString();
        }

        private void BindDropdown(ListControl ddl, object dataSource, string dataValueField, string dataTextField)
        {
            ddl.DataSource = dataSource;
            ddl.DataValueField = dataValueField;
            ddl.DataTextField = dataTextField;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("All Business Areas", ""));
        }
    }
}