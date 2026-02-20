using FGV.Prodata.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.T1C.Approval
{
    public partial class Default : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }
        // ==================================================================================
        // NEW EXPORT TO EXCEL FUNCTION
        // ==================================================================================
        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            // 1. Fetch Data (Mirroring BindData logic but without paging binding)
            IEnumerable<dynamic> query = new Class.Form().GetForms(bizAreaCodes: Auth.CCMSBizAreaCodes());

            // 2. Filter by Status
            var selectedStatus = ddlStatus.SelectedValue;
            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                if (selectedStatus == "pending-my-action")
                {
                    query = query.Where(d => (bool)d.IsPendingUserAction);
                }
                else
                {
                    query = query.Where(d =>
                        !(bool)d.IsPendingUserAction &&
                        string.Equals((string)d.Status, selectedStatus, StringComparison.OrdinalIgnoreCase));
                }
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

            // 4. Generate HTML for Excel
            var dataList = query.ToList();

            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head><meta http-equiv='content-type' content='application/vnd.ms-excel; charset=utf-8' /></head><body>");
            sb.Append("<table border='1' style='border-collapse:collapse;'>");

            // Header
            sb.Append("<tr style='background-color:#f2f2f2;'>");
            sb.Append("<th>#</th>");
            sb.Append("<th>BA</th>");
            sb.Append("<th>Reference No</th>");
            sb.Append("<th>Date</th>");
            sb.Append("<th>Details</th>");
            sb.Append("<th>Amount (RM)</th>");
            sb.Append("<th>Next Approver</th>");
            sb.Append("<th>Status</th>");
            sb.Append("</tr>");

            // Rows
            int index = 1;
            foreach (dynamic item in dataList)
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

                // Logic for "Next Approver" display text
                string nextApprover = item.NextApprover;
                if (item.Status != null && item.Status.ToString().Equals("Completed", StringComparison.OrdinalIgnoreCase))
                {
                    nextApprover = "Complete";
                }

                sb.Append("<tr>");
                sb.Append($"<td style='text-align:center;'>{index++}</td>");
                sb.Append($"<td>{item.BizAreaDisplayName}</td>");
                sb.Append($"<td style='mso-number-format:\"\\@\";'>{item.Ref}</td>"); // Force text format

                // Handle Date safely
                string dateStr = "";
                if (item.Date != null)
                {
                    // Assuming Date property might be DateTime or string, handle accordingly
                    dateStr = item.Date is DateTime ? ((DateTime)item.Date).ToString("dd/MM/yyyy") : item.Date.ToString();
                }
                sb.Append($"<td>{dateStr}</td>");

                sb.Append($"<td>{item.Details}</td>");

                // Amount Formatting
                string amountStr = item.Amount != null ? string.Format("{0:N2}", item.Amount) : "0.00";
                sb.Append($"<td style='text-align:right;'>{amountStr}</td>");

                sb.Append($"<td>{nextApprover}</td>");
                sb.Append($"<td>{displayStatus}</td>");
                sb.Append("</tr>");
            }

            sb.Append("</table>");
            sb.Append("</body></html>");

            // 5. Send Response
            string fileName = $"T1C_Approval_List_{DateTime.Now:yyyyMMdd_HHmm}.xls";
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

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["pageIndex"] = e.NewPageIndex.ToString();
            BindData();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            BindData();
        }

        private void BindData()
        {
            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            // 1. Fetch Data
            // We cast to IEnumerable to use deferred execution (lazy evaluation) 
            // This prevents creating multiple list copies in memory during filtering steps
            IEnumerable<dynamic> query = new Class.Form().GetForms(bizAreaCodes: Auth.CCMSBizAreaCodes());

            // 2. Filter by Status
            var selectedStatus = ddlStatus.SelectedValue;
            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                if (selectedStatus == "pending-my-action")
                {
                    query = query.Where(d => (bool)d.IsPendingUserAction);
                }
                else
                {
                    query = query.Where(d =>
                        !(bool)d.IsPendingUserAction &&
                        string.Equals((string)d.Status, selectedStatus, StringComparison.OrdinalIgnoreCase));
                }
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
            // .ToList() is called only once here, executing all filters efficiently
            gvData.DataSource = query.ToList();
            gvData.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
            gvData.DataBind();
        }
    }
}

//using FGV.Prodata.Web.UI;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;

//namespace Prodata.WebForm.T1C.Approval
//{
//    public partial class Default : ProdataPage
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                BindData(Auth.CCMSBizAreaCodes());
//            }
//        }

//        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            BindData(Auth.CCMSBizAreaCodes());
//        }

//        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
//        {
//            ViewState["pageIndex"] = e.NewPageIndex.ToString();
//            BindData(Auth.CCMSBizAreaCodes());
//        }

//        protected void btnSearch_Click(object sender, EventArgs e)
//        {
//            BindData(Auth.CCMSBizAreaCodes()); // 🔍 Trigger search
//        }

//        protected void txtSearch_TextChanged(object sender, EventArgs e)
//        {
//            BindData(Auth.CCMSBizAreaCodes()); // 🔍 Live search while typing
//        }

//        protected void btnClear_Click(object sender, EventArgs e)
//        {
//            txtSearch.Text = "";
//            BindData(Auth.CCMSBizAreaCodes());
//        }

//        private void BindData(List<string> CCMSBizAreaCodes = null)
//        {
//            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

//            // Get all forms
//            var data = new Class.Form().GetForms(bizAreaCodes: CCMSBizAreaCodes);

//            // Get selected status
//            var selectedStatus = ddlStatus.SelectedValue;

//            // 1. Filter by Status first
//            // If "Pending My Action" selected
//            if (selectedStatus == "pending-my-action")
//            {
//                data = data.Where(d => d.IsPendingUserAction).ToList();
//            }
//            // Else if any specific status selected (like "Pending", "Approved", etc.)
//            else if (!string.IsNullOrWhiteSpace(selectedStatus))
//            {
//                data = data.Where(d =>
//                    !d.IsPendingUserAction &&
//                    d.Status != null &&
//                    d.Status.Equals(selectedStatus, StringComparison.OrdinalIgnoreCase)
//                ).ToList();
//            }
//            // else (empty) = no filtering, show all

//            // 2. Filter by Search Keyword
//            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
//            {
//                string keyword = txtSearch.Text.Trim().ToLower();

//                data = data.Where(x =>
//                    (x.Ref != null && x.Ref.ToLower().Contains(keyword)) ||
//                    (x.Details != null && x.Details.ToLower().Contains(keyword)) ||
//                    (x.BizAreaDisplayName != null && x.BizAreaDisplayName.ToLower().Contains(keyword)) ||
//                    // Removed FormRequesterName (does not exist in ViewModel)
//                    // Fixed Amount check (Amount is string, so removed HasValue/Value)
//                    (x.Amount != null && x.Amount.ToLower().Contains(keyword))
//                ).ToList();
//            }

//            gvData.DataSource = data;
//            gvData.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
//            gvData.DataBind();
//        }
//    }
//}