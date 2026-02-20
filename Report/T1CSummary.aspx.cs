using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using Org.BouncyCastle.Asn1.Ocsp;
using Prodata.WebForm.Helpers;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Report
{
    public partial class T1CSummary : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //btnAdd.Visible = Auth.User().Can("t1c-add");

            if (!IsPostBack)
            {
                BindControl();
                BindData(!string.IsNullOrEmpty(ddlYear.SelectedValue) ? int.Parse(ddlYear.SelectedValue) : (int?)null);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Calculate BizAreas from Dropdown
            List<string> accessibleBizAreas = !string.IsNullOrEmpty(ddlBA.SelectedValue)
                ? new Class.IPMSBizArea().GetBizAreaCodes(ddlBA.SelectedValue)
                : new List<string>();

            BindData(
                !string.IsNullOrEmpty(ddlYear.SelectedValue) ? int.Parse(ddlYear.SelectedValue) : (int?)null,
                txtRef.Text.Trim(),
                !string.IsNullOrEmpty(txtStartDate.Text) ? DateTime.Parse(txtStartDate.Text) : (DateTime?)null,
                !string.IsNullOrEmpty(txtEndDate.Text) ? DateTime.Parse(txtEndDate.Text) : (DateTime?)null,
                !string.IsNullOrEmpty(txtMinAmount.Text) ? decimal.Parse(txtMinAmount.Text.Replace(",", "")) : (decimal?)null,
                !string.IsNullOrEmpty(txtMaxAmount.Text) ? decimal.Parse(txtMaxAmount.Text.Replace(",", "")) : (decimal?)null,
                ddlStatus.SelectedValue,
                accessibleBizAreas, // Pass List<string>
                txtKeyword.Text.Trim(),
                budgetTypeId: !string.IsNullOrEmpty(ddlBT.SelectedValue) ? Guid.Parse(ddlBT.SelectedValue) : (Guid?)null // NEW
                );
            divCardSearch.Attributes["class"] = "card card-outline";
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            ExportData("Excel");
        }

        // ==================================================================================
        // NEW PRINT ALL FUNCTION
        // ==================================================================================
        protected void btnPrintAll_Click(object sender, EventArgs e)
        {
            ExportData("Print");
        }

        private void ExportData(string type)
        {
            // 1. Get current filter values
            int? year = !string.IsNullOrEmpty(ddlYear.SelectedValue) ? int.Parse(ddlYear.SelectedValue) : (int?)null;
            string refNo = txtRef.Text.Trim();
            DateTime? startDate = !string.IsNullOrEmpty(txtStartDate.Text) ? DateTime.Parse(txtStartDate.Text) : (DateTime?)null;
            DateTime? endDate = !string.IsNullOrEmpty(txtEndDate.Text) ? DateTime.Parse(txtEndDate.Text) : (DateTime?)null;
            decimal? minAmount = !string.IsNullOrEmpty(txtMinAmount.Text) ? decimal.Parse(txtMinAmount.Text.Replace(",", "")) : (decimal?)null;
            decimal? maxAmount = !string.IsNullOrEmpty(txtMaxAmount.Text) ? decimal.Parse(txtMaxAmount.Text.Replace(",", "")) : (decimal?)null;
            string status = ddlStatus.SelectedValue;
            string keyword = txtKeyword.Text.Trim();
            Guid? budgetTypeId = !string.IsNullOrEmpty(ddlBT.SelectedValue) ? Guid.Parse(ddlBT.SelectedValue) : (Guid?)null;

            // 2. Prepare BizArea Logic
            string selectedBA = ddlBA.SelectedValue;
            List<string> bizAreaCodes = !string.IsNullOrEmpty(selectedBA)
                ? new Class.IPMSBizArea().GetBizAreaCodes(selectedBA)
                : new List<string>();

            if ((bizAreaCodes == null || !bizAreaCodes.Any()) && !string.IsNullOrEmpty(Auth.User().CCMSBizAreaCode))
            {
                bizAreaCodes = new List<string> { Auth.User().CCMSBizAreaCode };
            }

            List<string> statuses = null;
            if (!string.IsNullOrEmpty(status))
            {
                statuses = new List<string> { status };
            }

            var form = new Class.Form();

            // Fetch ALL data (no paging applied here naturally as GetForms returns a list)
            var dataList = form.GetForms(
                year: year,
                bizAreaCodes: bizAreaCodes,
                refNo: refNo,
                startDate: startDate,
                endDate: endDate,
                amountMin: minAmount,
                amountMax: maxAmount,
                statuses: statuses,
                budgetTypeId: budgetTypeId
            );

            // Keyword Filter
            if (!string.IsNullOrEmpty(keyword))
            {
                var k = keyword.ToLower();
                dataList = dataList.Where(x =>
                    (x.Ref != null && x.Ref.ToLower().Contains(k)) ||
                    (x.Details != null && x.Details.ToLower().Contains(k)) ||
                    (x.BizAreaDisplayName != null && x.BizAreaDisplayName.ToLower().Contains(k))
                ).ToList();
            }

            // 3. Generate HTML
            StringBuilder sb = new StringBuilder();

            if (type == "Excel")
            {
                sb.Append("<html><head><meta http-equiv='content-type' content='application/vnd.ms-excel; charset=utf-8' /></head><body>");
            }
            else // Print View
            {
                sb.Append("<html><head>");
                sb.Append("<title>T1C Budget List Print View</title>");
                sb.Append("<style>");
                sb.Append("body { font-family: Arial, sans-serif; font-size: 12px; }");
                sb.Append("table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }");
                sb.Append("th, td { border: 1px solid #000; padding: 5px; text-align: left; }");
                sb.Append("th { background-color: #f2f2f2; }");
                sb.Append(".text-right { text-align: right; }");
                sb.Append(".text-center { text-align: center; }");
                sb.Append(".header { text-align: center; margin-bottom: 20px; }");
                sb.Append("@media print { .no-print { display: none; } }");
                sb.Append("</style>");
                sb.Append("</head><body>");

                // Add Print Header with Buttons
                sb.Append("<div class='header'>");
                sb.Append("<h2>T1C Budget List</h2>");
                sb.Append($"<p>Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}</p>");

                // --- Buttons Section ---
                sb.Append("<div class='no-print' style='margin-bottom: 15px;'>");
                sb.Append("<button class='btn' onclick='window.print()'>Print</button>");
                sb.Append("<button class='btn' onclick='window.location.href=window.location.pathname'>Back</button>");
                sb.Append("</div>");

                sb.Append("</div>");
            }

            sb.Append("<table border='1'>");
            sb.Append("<thead><tr style='background-color:#f2f2f2;'>");
            sb.Append("<th>#</th>");
            sb.Append("<th>BA</th>");
            sb.Append("<th>Reference No</th>");
            sb.Append("<th>Date</th>");
            sb.Append("<th>Details</th>");
            sb.Append("<th>Amount (RM)</th>");
            sb.Append("<th>Next Approver</th>");
            sb.Append("<th>Status</th>");
            sb.Append("</tr></thead><tbody>");

            int index = 1;
            foreach (dynamic item in dataList)
            {
                string statusText = item.Status;
                if (statusText != null && statusText.Equals("SentBack", StringComparison.OrdinalIgnoreCase)) statusText = "Sent Back";

                string nextApprover = item.NextApprover;
                if (item.Status != null && item.Status.ToString().Equals("Completed", StringComparison.OrdinalIgnoreCase)) nextApprover = "Complete";

                sb.Append("<tr>");
                sb.Append($"<td class='text-center'>{index++}</td>");
                sb.Append($"<td>{item.BizAreaDisplayName}</td>");

                // For Excel, force text format. For Print, standard text.
                if (type == "Excel")
                    sb.Append($"<td style='mso-number-format:\"\\@\";'>{item.Ref}</td>");
                else
                    sb.Append($"<td>{item.Ref}</td>");

                string dateStr = "";
                if (item.Date != null) dateStr = item.Date is DateTime ? ((DateTime)item.Date).ToString("dd/MM/yyyy") : item.Date.ToString();

                sb.Append($"<td>{dateStr}</td>");
                sb.Append($"<td>{item.Details}</td>");

                string amountStr = item.Amount != null ? string.Format("{0:N2}", item.Amount) : "0.00";
                sb.Append($"<td class='text-right'>{amountStr}</td>");

                sb.Append($"<td>{nextApprover}</td>");
                sb.Append($"<td>{statusText}</td>");
                sb.Append("</tr>");
            }

            sb.Append("</tbody></table>");
            sb.Append("</body></html>");

            Response.Clear();
            Response.Buffer = true;

            if (type == "Excel")
            {
                string fileName = $"T1C_Budget_List_{DateTime.Now:yyyyMMdd_HHmm}.xls";
                Response.AddHeader("content-disposition", $"attachment;filename={fileName}");
                Response.ContentType = "application/vnd.ms-excel";
            }
            else // Print
            {
                // Open inline for printing
                Response.ContentType = "text/html";
            }

            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }

        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["pageIndex"] = e.NewPageIndex.ToString();

            // Calculate BizAreas from Dropdown for Paging
            List<string> accessibleBizAreas = !string.IsNullOrEmpty(ddlBA.SelectedValue)
                ? new Class.IPMSBizArea().GetBizAreaCodes(ddlBA.SelectedValue)
                : new List<string>();

            BindData(
                !string.IsNullOrEmpty(ddlYear.SelectedValue) ? int.Parse(ddlYear.SelectedValue) : (int?)null,
                txtRef.Text.Trim(),
                !string.IsNullOrEmpty(txtStartDate.Text) ? DateTime.Parse(txtStartDate.Text) : (DateTime?)null,
                !string.IsNullOrEmpty(txtEndDate.Text) ? DateTime.Parse(txtEndDate.Text) : (DateTime?)null,
                !string.IsNullOrEmpty(txtMinAmount.Text) ? decimal.Parse(txtMinAmount.Text.Replace(",", "")) : (decimal?)null,
                !string.IsNullOrEmpty(txtMaxAmount.Text) ? decimal.Parse(txtMaxAmount.Text.Replace(",", "")) : (decimal?)null,
                ddlStatus.SelectedValue,
                accessibleBizAreas,
                txtKeyword.Text.Trim()
                );
        }

        private void BindControl()
        { 
            ddlBA.Items.Clear();
            ddlBT.Items.Clear();

            int startYear = 2020;
            int currentYear = DateTime.Now.Year;

            for (int year = currentYear; year >= startYear; year--)
            {
                ListItem item = new ListItem(year.ToString(), year.ToString());
                if (year == currentYear) item.Selected = true;
                ddlYear.Items.Add(item);
            }

            // Bind Business Areas
            ddlBA.DataSource = new Class.IPMSBizArea().GetIPMSBizAreas();
            ddlBA.DataValueField = "Code";
            ddlBA.DataTextField = "DisplayName";
            ddlBA.DataBind();
            ddlBA.Items.Insert(0, new ListItem("All Business Areas", ""));
             
            var budgetTypes = Class.Functions.GetBudgetTypes();
            BindDropdown(ddlBT, budgetTypes, "ID", "DisplayName");
        }
        private void BindDropdown(ListControl ddl, object dataSource, string dataValueField, string dataTextField)
        {
            if (dataSource is IEnumerable<BudgetType> list)
            {
                list = list.Where(x => x.DeletedDate == null).ToList();
                dataSource = list;
            }

            ddl.DataSource = dataSource;
            ddl.DataValueField = dataValueField;
            ddl.DataTextField = dataTextField;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("All Budget Types", ""));
        }
        protected void btnReset_Click(object sender, EventArgs e)
        { 
            Response.Redirect(Request.RawUrl);
        }
        private void BindData(int? year = null, string refNo = null, DateTime? startDate = null, DateTime? endDate = null, decimal? minAmount = null, decimal? maxAmount = null, string status = null, List<string> bizAreaCodes = null, string keyword = null, Guid? budgetTypeId = null)
        {
            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            if ((bizAreaCodes == null || !bizAreaCodes.Any()) && !string.IsNullOrEmpty(Auth.User().CCMSBizAreaCode))
            {
                bizAreaCodes = new List<string> { Auth.User().CCMSBizAreaCode };
            }

            List<string> statuses = null;
            if (!string.IsNullOrEmpty(status))
            {
                statuses = new List<string> { status };
            }

            var form = new Class.Form();
            var formList = form.GetForms(
                year: year,
                bizAreaCodes: bizAreaCodes,
                refNo: refNo,
                startDate: startDate,
                endDate: endDate,
                amountMin: minAmount,
                amountMax: maxAmount,
                statuses: statuses,
                budgetTypeId: budgetTypeId
            );

            if (!string.IsNullOrEmpty(keyword))
            {
                var k = keyword.ToLower();
                formList = formList.Where(x =>
                    (x.Ref != null && x.Ref.ToLower().Contains(k)) ||
                    (x.Details != null && x.Details.ToLower().Contains(k)) ||
                    (x.BizAreaDisplayName != null && x.BizAreaDisplayName.ToLower().Contains(k))
                ).ToList();
            }

            gvData.DataSource = formList;
            gvData.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
            gvData.DataBind();
        } 
    }
}