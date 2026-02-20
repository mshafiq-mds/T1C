using FGV.Prodata.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.T1C.PoolBudget.Approval
{
    public partial class Default : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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

        private void BindData(List<string> CCMSBizAreaCodes = null)
        {
            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            // Get all forms
            var data = new Class.Form().GetFormsProcurement(bizAreaCodes: CCMSBizAreaCodes);

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
        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            // 1. Fetch Data (Mirroring BindData logic)
            // Use Class.Form().GetFormsProcurement for PoolBudget
            var data = new Class.Form().GetFormsProcurement(bizAreaCodes: Auth.CCMSBizAreaCodes());

            // 2. Filter logic (Copy from BindData)
            var selectedStatus = ddlStatus.SelectedValue;

            if (selectedStatus == "pending-my-action")
            {
                data = data.Where(d => d.IsPendingUserAction).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                data = data.Where(d =>
                    !d.IsPendingUserAction &&
                    d.Status != null &&
                    d.Status.Equals(selectedStatus, StringComparison.OrdinalIgnoreCase)
                ).ToList();
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
            sb.Append("</tr>");

            int index = 1;
            // Iterate dynamically
            foreach (dynamic item in data)
            {
                // Logic for "Status" display text (matching the Label logic in GridView)
                string displayStatus;
                if ((bool)item.IsPendingUserAction)
                {
                    displayStatus = "Pending My Action";
                }
                else if (item.Status != null && item.Status.ToString().Equals("SentBack", StringComparison.OrdinalIgnoreCase))
                {
                    displayStatus = "Sent Back";
                }
                else
                {
                    displayStatus = item.Status;
                }

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
                sb.Append("</tr>");
            }

            sb.Append("</table>");
            sb.Append("</body></html>");

            // 4. Send Response
            string fileName = $"Others_Budget_Approval_List_{DateTime.Now:yyyyMMdd_HHmm}.xls";
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