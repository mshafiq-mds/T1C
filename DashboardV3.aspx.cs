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
    public partial class DashboardV3 : ProdataPage
    {
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
                LoadDashboardData();

                // FIX: Force the UpdatePanel to refresh the HTML with the new GridView data
                UpdatePanelDashboard.Update();

                // Fire the modal popup
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openDetailModal();", true);
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error opening details: " + ex.Message);
            }
        }

        protected void btnSearchDetail_Click(object sender, EventArgs e)
        {
            BindModalGrid(hfModalCategory.Value, hfModalStatus.Value, txtSearchDetail.Text.Trim());
            LoadDashboardData();

            // FIX: Force the UpdatePanel to refresh
            UpdatePanelDashboard.Update();

            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openDetailModal();", true);
        }

        private void BindModalGrid(string category, string statusType, string searchKeyword = "")
        {
            string selectedBA = ddBA.SelectedValue;
            int selectedYear = int.TryParse(ddYear.SelectedValue, out int y) ? y : DateTime.Now.Year;
            var bizAreaHelper = new Class.IPMSBizArea();
            Dictionary<string, string> bizAreaDict = bizAreaHelper.GetAllBizAreaNames();

            List<string> targetCodes = new List<string>();
            if (!string.IsNullOrEmpty(selectedBA)) targetCodes = bizAreaHelper.GetBizAreaCodes(selectedBA);

            decimal totalAmount = 0;

            using (var db = new AppDbContext())
            {
                if (category == "T1C")
                {
                    var query = db.Forms.Where(x => x.CreatedDate.Year == selectedYear);
                    if (targetCodes.Any()) query = query.Where(x => targetCodes.Contains(x.BizAreaCode));
                    if (!string.IsNullOrEmpty(searchKeyword)) query = query.Where(x => x.Ref.Contains(searchKeyword));

                    switch (statusType)
                    {
                        case "Submitted": query = query.Where(x => x.Status != "Draft" && x.Status != "Rejected" && x.DeletedDate == null); break;
                        case "Under Review": query = query.Where(x => x.Status == "Pending" && x.DeletedDate == null); break;
                        case "Resubmit": query = query.Where(x => x.Status == "SentBack" && x.DeletedDate == null); break;
                        case "Approved": query = query.Where(x => x.Status == "Approved" && x.DeletedDate == null); break;
                        case "Deleted": query = query.Where(x => (x.Status != "Draft" && x.DeletedDate != null) || x.Status == "Rejected"); break;
                        case "Completed": query = query.Where(x => x.Status == "Completed" && x.DeletedDate == null); break;
                    }

                    var resultList = query.OrderByDescending(x => x.CreatedDate).Select(x => new { RefNo = x.Ref, Date = x.CreatedDate, BA = x.BizAreaCode, Status = x.Status, Amount = x.Amount }).ToList();
                    totalAmount = resultList.Sum(x => x.Amount ?? 0);
                    gvDetails.DataSource = resultList.Select(x => new { x.RefNo, x.Date, BA = x.BA + " - " + (bizAreaDict.ContainsKey(x.BA) ? bizAreaDict[x.BA] : "-"), x.Status, Amount = (x.Amount ?? 0).ToString("N2") }).ToList();
                }
                else if (category == "T1COthers")
                {
                    var query = db.FormsProcurement.Where(x => x.CreatedDate.Year == selectedYear);
                    if (targetCodes.Any()) query = query.Where(x => targetCodes.Contains(x.BizAreaCode));
                    if (!string.IsNullOrEmpty(searchKeyword)) query = query.Where(x => x.Ref.Contains(searchKeyword));

                    switch (statusType)
                    {
                        case "Submitted": query = query.Where(x => x.DeletedDate == null); break;
                        case "Under Review": query = query.Where(x => x.Status == "Pending" && x.DeletedDate == null); break;
                        case "Resubmit": query = query.Where(x => x.Status == "SentBack" && x.DeletedDate == null); break;
                        case "Approved": query = query.Where(x => x.Status == "Approved" && x.DeletedDate == null); break;
                        case "Deleted": query = query.Where(x => x.DeletedDate != null); break;
                        case "Completed": query = query.Where(x => x.Status == "Completed" && x.DeletedDate == null); break;
                    }

                    var resultList = query.OrderByDescending(x => x.CreatedDate).Select(x => new { RefNo = x.Ref, Date = x.CreatedDate, BA = x.BizAreaCode, Status = x.Status, Amount = x.Amount }).ToList();
                    totalAmount = resultList.Sum(x => x.Amount ?? 0);
                    gvDetails.DataSource = resultList.Select(x => new { x.RefNo, x.Date, BA = x.BA + " - " + (bizAreaDict.ContainsKey(x.BA) ? bizAreaDict[x.BA] : "-"), x.Status, Amount = (x.Amount ?? 0).ToString("N2") }).ToList();
                }
                else if (category == "Additional")
                {
                    var query = db.AdditionalBudgetRequests.Where(x => x.CreatedDate.Year == selectedYear);
                    if (targetCodes.Any()) query = query.Where(x => targetCodes.Contains(x.BA));
                    if (!string.IsNullOrEmpty(searchKeyword)) query = query.Where(x => x.RefNo.Contains(searchKeyword));

                    switch (statusType)
                    {
                        case "Submitted": query = query.Where(x => x.DeletedDate == null); break;
                        case "Under Review": query = query.Where(x => x.Status == "UnderReview" && x.DeletedDate == null); break;
                        case "Resubmit": query = query.Where(x => x.Status == "Resubmit" && x.DeletedDate == null); break;
                        case "Approved": query = query.Where(x => x.Status == "Completed" && x.DeletedDate == null); break;
                        case "Deleted": query = query.Where(x => x.DeletedDate != null); break;
                        case "Completed": query = query.Where(x => x.Status == "Finalized" && x.DeletedDate == null); break;
                    }

                    var resultList = query.OrderByDescending(x => x.CreatedDate).Select(x => new { RequestNo = x.RefNo, Date = x.CreatedDate, BA = x.BA, Status = x.Status }).ToList();
                    totalAmount = 0;
                    gvDetails.DataSource = resultList.Select(x => new { RefNo = x.RequestNo, x.Date, BA = x.BA + " - " + (bizAreaDict.ContainsKey(x.BA) ? bizAreaDict[x.BA] : "-"), Status = x.Status.ToString(), Amount = 0m.ToString("N2") }).ToList();
                }
                else if (category == "Transfer")
                {
                    var query = db.TransfersTransaction.Where(x => x.CreatedDate.Year == selectedYear);
                    if (targetCodes.Any()) query = query.Where(x => targetCodes.Contains(x.BA));
                    if (!string.IsNullOrEmpty(searchKeyword)) query = query.Where(x => x.RefNo.Contains(searchKeyword));

                    switch (statusType)
                    {
                        case "SUBMITTED": query = query.Where(x => (x.status == "Completed" || x.status == "sentback" || x.status == "Submitted" || x.status == "UnderReview") && x.DeletedDate == null); break;
                        case "BUDGET ALLOCATE": query = query.Where(x => x.status == "Submitted" && x.DeletedDate == null); break;
                        case "UNDER REVIEW": query = query.Where(x => x.status == "UnderReview" && x.DeletedDate == null); break;
                        case "RESUBMIT": query = query.Where(x => x.status == "sentback" && x.DeletedDate == null); break;
                        case "COMPLETED": query = query.Where(x => x.status == "Completed" && x.DeletedDate == null); break;
                        case "DELETED": query = query.Where(x => x.DeletedDate != null || x.status == "Deleted"); break;
                    }

                    var resultList = query.OrderByDescending(x => x.CreatedDate).Select(x => new { RefNo = x.RefNo, Date = x.CreatedDate, BA = x.BA, Status = x.status, Amount = x.FromTransfer }).ToList();
                    totalAmount = resultList.Sum(x => x.Amount ?? 0);
                    gvDetails.DataSource = resultList.Select(x => new { x.RefNo, x.Date, BA = x.BA + " - " + (bizAreaDict.ContainsKey(x.BA) ? bizAreaDict[x.BA] : "-"), Status = x.Status, Amount = (x.Amount ?? 0).ToString("N2") }).ToList();
                }

                lblModalTotal.Text = "RM " + totalAmount.ToString("N2");
                gvDetails.DataBind();
            }
        }

        private void LoadDashboardData()
        {
            string selectedBA = ddBA.SelectedValue;
            int selectedYear = int.TryParse(ddYear.SelectedValue, out int y) ? y : DateTime.Now.Year;

            List<string> targetCodes = new List<string>();
            var bizAreaHelper = new Class.IPMSBizArea();
            Dictionary<string, string> bizAreaDict = bizAreaHelper.GetAllBizAreaNames();

            if (!string.IsNullOrEmpty(selectedBA))
            {
                targetCodes = bizAreaHelper.GetBizAreaCodes(selectedBA);
            }

            using (var db = new AppDbContext())
            {
                var t1cData = new int[] { 0, 0, 0, 0, 0, 0 };
                var t1cOthersData = new int[] { 0, 0, 0, 0, 0, 0 };
                var addData = new int[] { 0, 0, 0, 0, 0, 0 };
                var transData = new int[] { 0, 0, 0, 0, 0, 0 };

                DashboardCounts addS = new DashboardCounts();
                DashboardCounts trS = new DashboardCounts();
                DashboardCounts fmS = new DashboardCounts();
                DashboardCounts fmoS = new DashboardCounts();

                // 1. Additional Budget
                try
                {
                    var addQ = db.AdditionalBudgetRequests.Where(x => x.CreatedDate.Year == selectedYear);
                    if (targetCodes.Any()) addQ = addQ.Where(x => targetCodes.Contains(x.BA));

                    addS = addQ.GroupBy(x => 1).Select(g => new DashboardCounts
                    {
                        Deleted = g.Count(x => x.DeletedDate != null),
                        Submitted = g.Count(x => x.DeletedDate == null),
                        Review = g.Count(x => x.Status.ToString() == "UnderReview" && x.DeletedDate == null),
                        Resubmit = g.Count(x => x.Status.ToString() == "Resubmit" && x.DeletedDate == null),
                        Complete = g.Count(x => x.Status.ToString() == "Completed" && x.DeletedDate == null),
                        Finalized = g.Count(x => x.Status.ToString() == "Finalized" && x.DeletedDate == null)
                    }).FirstOrDefault() ?? new DashboardCounts();

                    UpdateSection(addS, LblAdditionalSubmitted, LblAdditionalReview, LblAdditionalResubmit, LblAdditionalComplete, LblAdditionalDeleted, null);
                    addData = new int[] { addS.Submitted, addS.Review, addS.Resubmit, addS.Complete, addS.Deleted, addS.Finalized };
                }
                catch { }

                // 2. Transfer Transactions 
                try
                {
                    var trQ = db.TransfersTransaction.Where(x => x.CreatedDate.Year == selectedYear);
                    if (targetCodes.Any()) trQ = trQ.Where(x => targetCodes.Contains(x.BA));

                    trS = trQ.GroupBy(x => 1).Select(g => new DashboardCounts
                    {
                        Submitted = g.Count(x => (x.status.ToString() == "Completed" || x.status.ToString() == "sentback" || x.status.ToString() == "Submitted" || x.status.ToString() == "UnderReview") && x.DeletedDate == null),
                        Review = g.Count(x => x.status.ToString() == "Submitted" && x.DeletedDate == null),
                        Resubmit = g.Count(x => x.status.ToString() == "UnderReview" && x.DeletedDate == null),
                        Complete = g.Count(x => x.status.ToString() == "sentback" && x.DeletedDate == null),
                        Deleted = g.Count(x => x.DeletedDate != null || x.status.ToString() == "Deleted"),
                        Finalized = g.Count(x => x.status.ToString() == "Completed" && x.DeletedDate == null)
                    }).FirstOrDefault() ?? new DashboardCounts();

                    UpdateSection(trS, LblTransferSubmitted, LblTransferReview, LblTransferResubmit, LblTransferComplete, LblTransferDeleted, LblTransferFinalized);
                    transData = new int[] { trS.Submitted, trS.Review, trS.Resubmit, trS.Complete, trS.Deleted, trS.Finalized };
                }
                catch { }

                // 3. T1C (Forms)
                try
                {
                    var fmQ = db.Forms.Where(x => x.CreatedDate.Year == selectedYear);
                    if (targetCodes.Any()) fmQ = fmQ.Where(x => targetCodes.Contains(x.BizAreaCode));

                    fmS = fmQ.GroupBy(x => 1).Select(g => new DashboardCounts
                    {
                        Deleted = g.Count(x => (x.Status != "Draft" && x.DeletedDate != null) || x.Status == "Rejected"),
                        Submitted = g.Count(x => x.Status != "Draft" && x.Status != "Rejected" && x.DeletedDate == null),
                        Review = g.Count(x => x.Status == "Pending" && x.DeletedDate == null),
                        Resubmit = g.Count(x => x.Status == "SentBack" && x.DeletedDate == null),
                        Complete = g.Count(x => x.Status == "Approved" && x.DeletedDate == null),
                        Finalized = g.Count(x => x.Status == "Completed" && x.DeletedDate == null)
                    }).FirstOrDefault() ?? new DashboardCounts();

                    UpdateSection(fmS, LblT1CSubmitted, LblT1CReview, LblT1CResubmit, LblT1CComplete, LblT1CDeleted, LblT1CFinalized);
                    t1cData = new int[] { fmS.Submitted, fmS.Review, fmS.Resubmit, fmS.Complete, fmS.Deleted, fmS.Finalized };
                }
                catch { }

                // 4. T1C Others (FormsProcurement)
                try
                {
                    var fmoQ = db.FormsProcurement.Where(x => x.CreatedDate.Year == selectedYear);
                    if (targetCodes.Any()) fmoQ = fmoQ.Where(x => targetCodes.Contains(x.BizAreaCode));

                    fmoS = fmoQ.GroupBy(x => 1).Select(g => new DashboardCounts
                    {
                        Deleted = g.Count(x => x.DeletedDate != null),
                        Submitted = g.Count(x => x.DeletedDate == null),
                        Review = g.Count(x => x.Status == "Pending" && x.DeletedDate == null),
                        Resubmit = g.Count(x => x.Status == "SentBack" && x.DeletedDate == null),
                        Complete = g.Count(x => x.Status == "Approved" && x.DeletedDate == null),
                        Finalized = g.Count(x => x.Status == "Completed" && x.DeletedDate == null)
                    }).FirstOrDefault() ?? new DashboardCounts();

                    UpdateSection(fmoS, LblT1COthersSubmitted, LblT1COthersReview, LblT1COthersResubmit, LblT1COthersComplete, LblT1COthersDeleted, null);
                    t1cOthersData = new int[] { fmoS.Submitted, fmoS.Review, fmoS.Resubmit, fmoS.Complete, fmoS.Deleted, fmoS.Finalized };
                }
                catch { }

                // ----------------------------------------------------
                // TOP KPI CALCULATION
                // ----------------------------------------------------
                try
                {
                    int totalPending =
                        fmS.Review + fmS.Resubmit
                        + fmoS.Review + fmoS.Resubmit
                        + addS.Review + addS.Resubmit
                        + trS.Resubmit + trS.Complete;
                    lblPendingApprovals.Text = totalPending.ToString();

                    double totalCompleted = fmS.Complete + fmoS.Complete + addS.Complete + trS.Finalized +
                                            fmS.Finalized + fmoS.Finalized + addS.Finalized;
                    double totalActive = fmS.Submitted + fmoS.Submitted + addS.Submitted + trS.Submitted;

                    double completionRate = totalActive > 0 ? (totalCompleted / totalActive) * 100 : 0;
                    lblCompletionRate.Text = completionRate.ToString("0.0") + "%";

                    var budgetQuery = db.Forms.Where(x => x.CreatedDate.Year == selectedYear && x.DeletedDate == null && x.Status != "Draft" && x.Status != "Rejected");
                    if (targetCodes.Any()) budgetQuery = budgetQuery.Where(x => targetCodes.Contains(x.BizAreaCode));

                    decimal totalBudget = budgetQuery.Sum(x => x.Amount ?? 0);

                    if (totalBudget >= 1000000)
                        lblTotalBudget.Text = "RM " + (totalBudget / 1000000m).ToString("0.##") + "M";
                    else
                        lblTotalBudget.Text = "RM " + totalBudget.ToString("N0");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("KPI Calculation Error: " + ex.Message);
                    lblTotalBudget.Text = "RM 0";
                    lblCompletionRate.Text = "0%";
                    lblPendingApprovals.Text = "0";
                }

                // ----------------------------------------------------
                // DOUGHNUT CHART LOGIC
                // ----------------------------------------------------
                try
                {
                    var dQ = db.Forms.Where(x => x.CreatedDate.Year == selectedYear && x.DeletedDate == null && x.Status != "Draft" && x.Status != "Rejected");
                    if (targetCodes.Any()) dQ = dQ.Where(x => targetCodes.Contains(x.BizAreaCode));

                    var top10BAs = dQ.GroupBy(x => x.BizAreaCode)
                                     .Select(g => new { BA = g.Key, TotalAmount = g.Sum(x => x.Amount ?? 0) })
                                     .OrderByDescending(x => x.TotalAmount)
                                     .Take(10)
                                     .ToList();

                    List<string> dLabels = new List<string>();
                    List<decimal> dData = new List<decimal>();
                    decimal dTotal = 0;

                    foreach (var item in top10BAs)
                    {
                        string baName = bizAreaDict.ContainsKey(item.BA) ? bizAreaDict[item.BA] : item.BA;
                        dLabels.Add(baName);
                        dData.Add(item.TotalAmount);
                        dTotal += item.TotalAmount;
                    }

                    string centerText = "RM 0";
                    if (dTotal >= 1000000)
                        centerText = "RM " + (dTotal / 1000000m).ToString("0.##") + "M";
                    else if (dTotal > 0)
                        centerText = "RM " + dTotal.ToString("N0");

                    var doughnutObj = new { labels = dLabels, data = dData, centerText = centerText };
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    hfChartDataDoughnut.Value = js.Serialize(doughnutObj);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Doughnut Data Error: " + ex.Message);
                }

                JavaScriptSerializer mainJs = new JavaScriptSerializer();
                hfChartDataT1C.Value = mainJs.Serialize(t1cData);
                hfChartDataT1COthers.Value = mainJs.Serialize(t1cOthersData);
                hfChartDataAdd.Value = mainJs.Serialize(addData);
                hfChartDataTrans.Value = mainJs.Serialize(transData);

                LoadRecentActivity(db, selectedYear, targetCodes);
            }
        }

        private void LoadRecentActivity(AppDbContext db, int year, List<string> targetCodes)
        {
            try
            {
                var query = db.Forms.Where(x => x.CreatedDate.Year == year);
                if (targetCodes.Any()) query = query.Where(x => targetCodes.Contains(x.BizAreaCode));

                var recentData = query.OrderByDescending(x => x.CreatedDate).Take(6).Select(x => new
                {
                    RefNo = x.Ref,
                    Date = x.CreatedDate,
                    BA = x.BizAreaCode,
                    Status = x.Status,
                    Amount = x.Amount ?? 0
                }).ToList();

                gvRecentActivity.DataSource = recentData;
                gvRecentActivity.DataBind();
            }
            catch (Exception) { /* Handled silently for UI safety */ }
        }

        // Changed to protected to ensure inline binding <%# ... %> works safely
        protected string GetStatusBadgeClass(string status)
        {
            string s = (status ?? "").ToLower();
            if (s.Contains("approve") || s.Contains("completed") || s.Contains("finalized")) return "badge-approved";
            if (s.Contains("pending") || s.Contains("review") || s.Contains("underreview")) return "badge-pending";
            if (s.Contains("resubmit") || s.Contains("reject") || s.Contains("deleted") || s.Contains("sentback")) return "badge-resubmit";
            return "badge-default";
        }

        private void UpdateSection(DashboardCounts stats, Label s, Label r, Label rs, Label c, Label d, Label f)
        {
            if (s != null) s.Text = stats.Submitted.ToString();
            if (r != null) r.Text = stats.Review.ToString();
            if (rs != null) rs.Text = stats.Resubmit.ToString();
            if (c != null) c.Text = stats.Complete.ToString();
            if (d != null) d.Text = stats.Deleted.ToString();
            if (f != null) f.Text = stats.Finalized.ToString();
        }

        private void BindDropdown(ListControl ddl, object dataSource, string val, string txt)
        {
            ddl.DataSource = dataSource;
            ddl.DataValueField = val;
            ddl.DataTextField = txt;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("All", ""));
        }
    }
}