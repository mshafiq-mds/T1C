using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData(Auth.CCMSBizAreaCodes());
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            BindData(Auth.CCMSBizAreaCodes());
        }

        private void BindData(List<string> CCMSBizAreaCodes = null)
        {
            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            // 1. Fetch Data
            // Get all forms with status approved and completed
            IEnumerable<dynamic> query = new Class.Form().GetForms(bizAreaCodes: CCMSBizAreaCodes, statuses: new List<string> { "Approved", "Completed" });

            // 2. Filter by Status
            var selectedStatus = ddlStatus.SelectedValue;
            if (!string.IsNullOrEmpty(selectedStatus))
            {
                query = query.Where(d => d.Status != null && d.Status.Equals(selectedStatus, StringComparison.OrdinalIgnoreCase));
            }

            // 3. Filter by Search Keyword
            var keyword = txtSearch.Text?.Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x =>
                    (x.Ref != null && ((string)x.Ref).ToLower().Contains(keyword)) ||
                    (x.Details != null && ((string)x.Details).ToLower().Contains(keyword)) ||
                    (x.BizAreaDisplayName != null && ((string)x.BizAreaDisplayName).ToLower().Contains(keyword)) ||
                    (x.Amount != null && ((string)x.Amount).ToLower().Contains(keyword))
                );
            }

            // 4. Materialize & Bind
            gvData.DataSource = query.ToList();
            gvData.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
            gvData.DataBind();
        }

        private void BindDataOld(List<string> CCMSBizAreaCodes = null)
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
        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            // 1. Fetch Data (Mirroring BindData logic)
            // Use Auth.CCMSBizAreaCodes() as per Page_Load
            var data = new Class.Form().GetForms(bizAreaCodes: Auth.CCMSBizAreaCodes(), statuses: new List<string> { "Approved", "Completed" });

            // 2. Filter logic (Copy from BindData)
            var selectedStatus = ddlStatus.SelectedValue;

            if (!string.IsNullOrEmpty(selectedStatus))
            {
                data = data.Where(d => d.Status != null && d.Status.Equals(selectedStatus, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // 3. Generate HTML for Excel
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head><meta http-equiv='content-type' content='application/vnd.ms-excel; charset=utf-8' /></head><body>");
            sb.Append("<table border='1' style='border-collapse:collapse;'>");

            // Headers
            sb.Append("<tr style='background-color:#f2f2f2;'>");
            sb.Append("<th>#</th>");
            sb.Append("<th>BA</th>");
            sb.Append("<th>Reference No</th>");
            sb.Append("<th>Date</th>");
            sb.Append("<th>Details</th>");
            sb.Append("<th>Amount (RM)</th>");
            sb.Append("<th>Status</th>");
            sb.Append("<th>PO Reviewed</th>");
            sb.Append("</tr>");

            int index = 1;
            // Iterate dynamically
            foreach (dynamic item in data)
            {
                // Logic for "Status" display text
                string displayStatus = item.Status ?? "";

                // Logic for "PO Reviewed"
                bool isReviewed = IsPoReviewed(item.Id);
                string poReviewedText = isReviewed ? "Yes" : "No";

                sb.Append("<tr>");
                sb.Append($"<td style='text-align:center;'>{index++}</td>");
                sb.Append($"<td>{item.BizAreaDisplayName}</td>");
                sb.Append($"<td style='mso-number-format:\"\\@\";'>{item.Ref}</td>"); // Force text format

                // Handle Date
                string dateStr = "";
                if (item.Date != null)
                {
                    dateStr = item.Date is DateTime ? ((DateTime)item.Date).ToString("dd/MM/yyyy") : item.Date.ToString();
                }
                sb.Append($"<td>{dateStr}</td>");

                sb.Append($"<td>{item.Details}</td>");

                // Amount Formatting
                string amountStr = item.Amount != null ? string.Format("{0:N2}", item.Amount) : "0.00";
                sb.Append($"<td style='text-align:right;'>{amountStr}</td>");

                sb.Append($"<td>{displayStatus}</td>");
                sb.Append($"<td style='text-align:center;'>{poReviewedText}</td>");
                sb.Append("</tr>");
            }

            sb.Append("</table>");
            sb.Append("</body></html>");

            // 4. Send Response
            string fileName = $"Upload_PO_List_{DateTime.Now:yyyyMMdd_HHmm}.xls";
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", $"attachment;filename={fileName}");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }
    }
}