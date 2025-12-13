using FGV.Prodata.Web.UI;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

namespace Prodata.WebForm
{
    public partial class Dashboard : ProdataPage
    {
        // Helper class to hold count data
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
            Page.ClientScript.RegisterStartupScript(this.GetType(), "BodyClass", "<script>document.body.classList.add('dashboard-lock');</script>");

            if (!IsPostBack)
            {
                LoadBusinessAreas();
                LoadYears();
                LoadDashboardData();
            }
        }

        private void LoadBusinessAreas()
        {
            try
            {
                var currentUserBA = Auth.User().CCMSBizAreaCode;
                var bizAreas = new Class.IPMSBizArea().GetIPMSBizAreas();
                BindDropdown(ddBA, bizAreas, "Code", "DisplayName");
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
            for (int i = currentYear - 2; i <= currentYear + 1; i++) years.Add(i);
            ddYear.DataSource = years;
            ddYear.DataBind();
            if (ddYear.Items.FindByValue(currentYear.ToString()) != null) ddYear.SelectedValue = currentYear.ToString();
        }

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

                hfModalCategory.Value = category;
                hfModalStatus.Value = statusType;
                txtSearchDetail.Text = string.Empty;

                lblModalTitle.Text = $"{category} - {statusType} List ({ddYear.SelectedValue})";

                BindModalGrid(category, statusType);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openDetailModal();", true);

                // Reload Charts to fix Canvas wipe on UpdatePanel refresh
                LoadDashboardData();
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error opening details: " + ex.Message);
            }
        }

        protected void btnSearchDetail_Click(object sender, EventArgs e)
        {
            BindModalGrid(hfModalCategory.Value, hfModalStatus.Value, txtSearchDetail.Text.Trim());
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openDetailModal();", true);
            LoadDashboardData();
        }

        private void BindModalGrid(string category, string statusType, string searchKeyword = "")
        {
            string selectedBA = ddBA.SelectedValue;
            int selectedYear = int.TryParse(ddYear.SelectedValue, out int y) ? y : DateTime.Now.Year;
            var bizAreaHelper = new Class.IPMSBizArea();

            List<string> targetCodes = new List<string>();
            if (!string.IsNullOrEmpty(selectedBA))
            {
                targetCodes = bizAreaHelper.GetBizAreaCodes(selectedBA);
            }

            // Variable to calculate total
            decimal totalAmount = 0;

            using (var db = new AppDbContext())
            {
                // -----------------------
                // 1. T1C Logic
                // -----------------------
                if (category == "T1C")
                {
                    var query = db.Forms.Where(x => x.CreatedDate.Year == selectedYear);
                    if (targetCodes.Any()) query = query.Where(x => targetCodes.Contains(x.BizAreaCode));
                    if (!string.IsNullOrEmpty(searchKeyword)) query = query.Where(x => x.Ref.Contains(searchKeyword));

                    switch (statusType)
                    {
                        case "Submitted": query = query.Where(x => x.DeletedDate == null); break;
                        case "Review": query = query.Where(x => x.Status == "Pending"); break;
                        case "Resubmit": query = query.Where(x => x.Status == "SentBack"); break;
                        case "Complete": query = query.Where(x => x.Status == "Approved"); break;
                        case "Deleted": query = query.Where(x => x.DeletedDate != null); break;
                        case "Finalized": query = query.Where(x => x.Status == "Completed"); break;
                    }

                    // FIX 1: T1C 'Amount' allows nulls. We check 'Amount' existence and handle nulls.
                    var resultList = query.OrderByDescending(x => x.CreatedDate)
                        .Select(x => new
                        {
                            RefNo = x.Ref,
                            Date = x.CreatedDate,
                            BA = x.BizAreaCode,
                            Status = x.Status,
                            Amount = x.Amount // Forms table has Amount
                        })
                        .ToList();

                    // FIX 2: Handle nullable decimal in Sum (x.Amount ?? 0)
                    totalAmount = resultList.Sum(x => x.Amount ?? 0);

                    // FIX 3: Handle nullable decimal in ToString
                    gvDetails.DataSource = resultList.Select(x => new
                    {
                        x.RefNo,
                        x.Date,
                        BA = x.BA + " - " + bizAreaHelper.GetNameByCode(x.BA),
                        x.Status,
                        Amount = (x.Amount ?? 0).ToString("N2")
                    }).ToList();
                }
                // -----------------------
                // 2. Additional Logic
                // -----------------------
                else if (category == "Additional")
                {
                    var query = db.AdditionalBudgetRequests.Where(x => x.CreatedDate.Year == selectedYear);
                    if (targetCodes.Any()) query = query.Where(x => targetCodes.Contains(x.BA));
                    if (!string.IsNullOrEmpty(searchKeyword)) query = query.Where(x => x.RefNo.Contains(searchKeyword));

                    switch (statusType)
                    {
                        case "Submitted": query = query.Where(x => x.DeletedDate == null); break;
                        case "Review": query = query.Where(x => x.Status == 2); break;
                        case "Resubmit": query = query.Where(x => x.Status == 0); break;
                        case "Complete": query = query.Where(x => x.Status == 3); break;
                        case "Deleted": query = query.Where(x => x.DeletedDate != null); break;
                        case "Finalized": query = query.Where(x => x.Status == 4); break;
                    }

                    // FIX 4: Your 'AdditionalBudgetRequests' table DOES NOT have an 'Amount' column.
                    // I removed 'Amount = x.Amount' so it compiles. 
                    var resultList = query.OrderByDescending(x => x.CreatedDate)
                        .Select(x => new
                        {
                            RequestNo = x.RefNo,
                            Date = x.CreatedDate,
                            BA = x.BA,
                            Status = x.Status
                            // If you find the column name (e.g. TotalAmount), add it here like: RealAmount = x.TotalAmount
                        })
                        .ToList();

                    // Defaults to 0 because column is missing
                    totalAmount = 0;

                    gvDetails.DataSource = resultList.Select(x => new
                    {
                        x.RequestNo,
                        x.Date,
                        BA = x.BA + " - " + bizAreaHelper.GetNameByCode(x.BA),
                        x.Status,
                        Amount = "0.00" // Placeholder
                    }).ToList();
                }
                // -----------------------
                // 3. Transfer Logic
                // -----------------------
                else if (category == "Transfer")
                {
                    var query = db.TransfersTransaction.Where(x => x.CreatedDate.Year == selectedYear);
                    if (targetCodes.Any()) query = query.Where(x => targetCodes.Contains(x.BA));
                    if (!string.IsNullOrEmpty(searchKeyword)) query = query.Where(x => x.RefNo.Contains(searchKeyword));

                    switch (statusType)
                    {
                        case "Submitted": query = query.Where(x => x.DeletedDate == null); break;
                        case "Review": query = query.Where(x => x.status == 2); break;
                        case "Resubmit": query = query.Where(x => x.status == 0); break;
                        case "Complete": query = query.Where(x => x.status == 3); break;
                        case "Deleted": query = query.Where(x => x.DeletedDate != null); break;
                        case "Finalized": query = query.Where(x => x.status == 4); break;
                    }

                    // FIX 5: Your 'TransfersTransaction' table DOES NOT have an 'Amount' column.
                    // I removed 'Amount = x.Amount' so it compiles.
                    var resultList = query.OrderByDescending(x => x.CreatedDate)
                        .Select(x => new
                        {
                            RefNo = x.RefNo,
                            Date = x.CreatedDate,
                            BA = x.BA,
                            Status = x.status
                        })
                        .ToList();

                    // Defaults to 0
                    totalAmount = 0;

                    gvDetails.DataSource = resultList.Select(x => new
                    {
                        x.RefNo,
                        x.Date,
                        BA = x.BA + " - " + bizAreaHelper.GetNameByCode(x.BA),
                        x.Status,
                        Amount = "0.00" // Placeholder
                    }).ToList();
                }

                // Update the Modal Label
                lblModalTotal.Text = "RM" + totalAmount.ToString("N2");
                gvDetails.DataBind();
            }
        }

        private void LoadDashboardData()
        {
            string selectedBA = ddBA.SelectedValue;
            int selectedYear = int.TryParse(ddYear.SelectedValue, out int y) ? y : DateTime.Now.Year;

            List<string> targetCodes = new List<string>();
            if (!string.IsNullOrEmpty(selectedBA))
            {
                targetCodes = new Class.IPMSBizArea().GetBizAreaCodes(selectedBA);
            }

            using (var db = new AppDbContext())
            {
                try
                {
                    // 1. Additional
                    var addQ = db.AdditionalBudgetRequests.Where(x => x.CreatedDate.Year == selectedYear);
                    if (targetCodes.Any()) addQ = addQ.Where(x => targetCodes.Contains(x.BA));

                    var addS = addQ.GroupBy(x => 1).Select(g => new DashboardCounts
                    {
                        Deleted = g.Count(x => x.DeletedDate != null),
                        Submitted = g.Count(x => x.DeletedDate == null),
                        Review = g.Count(x => x.Status == 2),
                        Resubmit = g.Count(x => x.Status == 0),
                        Complete = g.Count(x => x.Status == 3),
                        Finalized = g.Count(x => x.Status == 4)
                    }).FirstOrDefault() ?? new DashboardCounts();
                    UpdateSection(addS, LblAdditionalSubmitted, LblAdditionalReview, LblAdditionalResubmit, LblAdditionalComplete, LblAdditionalDeleted, LblAdditionalFinalized);

                    // 2. Transfer
                    var trQ = db.TransfersTransaction.Where(x => x.CreatedDate.Year == selectedYear);
                    if (targetCodes.Any()) trQ = trQ.Where(x => targetCodes.Contains(x.BA));

                    var trS = trQ.GroupBy(x => 1).Select(g => new DashboardCounts
                    {
                        Deleted = g.Count(x => x.DeletedDate != null),
                        Submitted = g.Count(x => x.DeletedDate == null),
                        Review = g.Count(x => x.status == 2),
                        Resubmit = g.Count(x => x.status == 0),
                        Complete = g.Count(x => x.status == 3),
                        Finalized = g.Count(x => x.status == 4)
                    }).FirstOrDefault() ?? new DashboardCounts();
                    UpdateSection(trS, LblTransferSubmitted, LblTransferReview, LblTransferResubmit, LblTransferComplete, LblTransferDeleted, LblTransferFinalized);

                    // 3. T1C
                    var fmQ = db.Forms.Where(x => x.CreatedDate.Year == selectedYear);
                    if (targetCodes.Any()) fmQ = fmQ.Where(x => targetCodes.Contains(x.BizAreaCode));

                    var fmS = fmQ.GroupBy(x => 1).Select(g => new DashboardCounts
                    {
                        Deleted = g.Count(x => x.DeletedDate != null),
                        Submitted = g.Count(x => x.DeletedDate == null),
                        Review = g.Count(x => x.Status == "Pending"),
                        Resubmit = g.Count(x => x.Status == "SentBack"),
                        Complete = g.Count(x => x.Status == "Approved"),
                        Finalized = g.Count(x => x.Status == "Completed")
                    }).FirstOrDefault() ?? new DashboardCounts();
                    UpdateSection(fmS, LblT1CSubmitted, LblT1CReview, LblT1CResubmit, LblT1CComplete, LblT1CDeleted, LblT1CFinalized);

                    // Render Charts
                    var t1cData = new int[] { fmS.Submitted, fmS.Review, fmS.Resubmit, fmS.Complete, fmS.Deleted, fmS.Finalized };
                    var addData = new int[] { addS.Submitted, addS.Review, addS.Resubmit, addS.Complete, addS.Deleted, addS.Finalized };
                    var transData = new int[] { trS.Submitted, trS.Review, trS.Resubmit, trS.Complete, trS.Deleted, trS.Finalized };

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    string script = $"renderDashboardCharts({js.Serialize(t1cData)}, {js.Serialize(addData)}, {js.Serialize(transData)});";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "InitCharts", script, true);
                }
                catch (Exception ex) { SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error: " + ex.Message); }
            }
        }

        private void UpdateSection(DashboardCounts stats, Label s, Label r, Label rs, Label c, Label d, Label f)
        {
            s.Text = stats.Submitted.ToString(); r.Text = stats.Review.ToString(); rs.Text = stats.Resubmit.ToString();
            c.Text = stats.Complete.ToString(); d.Text = stats.Deleted.ToString(); f.Text = stats.Finalized.ToString();
        }

        private void BindDropdown(ListControl ddl, object dataSource, string val, string txt)
        {
            ddl.DataSource = dataSource; ddl.DataValueField = val; ddl.DataTextField = txt; ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("All Business Areas", ""));
        }
    }
}