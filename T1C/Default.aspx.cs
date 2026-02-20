using CustomGuid.AspNet.Identity;
using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using Prodata.WebForm.Helpers;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;   // Required for Excel Export
using System.Text; // Required for Excel Export

namespace Prodata.WebForm.T1C
{
    public partial class Default : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btnAdd.Visible = Auth.User().Can("t1c-add");

            if (!IsPostBack)
            {
                BindControl();
                BindData(!string.IsNullOrEmpty(ddlYear.SelectedValue) ? int.Parse(ddlYear.SelectedValue) : (int?)null);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData(
                !string.IsNullOrEmpty(ddlYear.SelectedValue) ? int.Parse(ddlYear.SelectedValue) : (int?)null,
                txtRef.Text.Trim(),
                !string.IsNullOrEmpty(txtStartDate.Text) ? DateTime.Parse(txtStartDate.Text) : (DateTime?)null,
                !string.IsNullOrEmpty(txtEndDate.Text) ? DateTime.Parse(txtEndDate.Text) : (DateTime?)null,
                !string.IsNullOrEmpty(txtMinAmount.Text) ? decimal.Parse(txtMinAmount.Text.Replace(",", "")) : (decimal?)null,
                !string.IsNullOrEmpty(txtMaxAmount.Text) ? decimal.Parse(txtMaxAmount.Text.Replace(",", "")) : (decimal?)null,
                ddlStatus.SelectedValue
                );
            divCardSearch.Attributes["class"] = "card card-outline";
        }

        // ==================================================================================
        // NEW EXPORT TO EXCEL FUNCTION
        // ==================================================================================
        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            // 1. Get current filter values (same logic as BindData/Search)
            int? year = !string.IsNullOrEmpty(ddlYear.SelectedValue) ? int.Parse(ddlYear.SelectedValue) : (int?)null;
            string refNo = txtRef.Text.Trim();
            DateTime? startDate = !string.IsNullOrEmpty(txtStartDate.Text) ? DateTime.Parse(txtStartDate.Text) : (DateTime?)null;
            DateTime? endDate = !string.IsNullOrEmpty(txtEndDate.Text) ? DateTime.Parse(txtEndDate.Text) : (DateTime?)null;
            decimal? minAmount = !string.IsNullOrEmpty(txtMinAmount.Text) ? decimal.Parse(txtMinAmount.Text.Replace(",", "")) : (decimal?)null;
            decimal? maxAmount = !string.IsNullOrEmpty(txtMaxAmount.Text) ? decimal.Parse(txtMaxAmount.Text.Replace(",", "")) : (decimal?)null;
            string status = ddlStatus.SelectedValue;

            // 2. Fetch the data (Full List, ignored paging)
            var bizAreaCode = Auth.User().CCMSBizAreaCode;
            List<string> statuses = null;
            if (!string.IsNullOrEmpty(status))
            {
                statuses = new List<string> { status };
            }

            var form = new Class.Form();
            // Fetch data using the existing class method
            var dataList = form.GetForms(
                year: year,
                bizAreaCode: bizAreaCode,
                refNo: refNo,
                startDate: startDate,
                endDate: endDate,
                amountMin: minAmount,
                amountMax: maxAmount,
                statuses: statuses
            );

            // 3. Generate HTML for Excel
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head><meta http-equiv='content-type' content='application/vnd.ms-excel; charset=utf-8' /></head><body>");
            sb.Append("<table border='1' style='border-collapse:collapse;'>");

            // Header
            sb.Append("<tr style='background-color:#f2f2f2;'>");
            sb.Append("<th>#</th>");
            sb.Append("<th>Reference No</th>");
            sb.Append("<th>Date</th>");
            sb.Append("<th>Details</th>");
            sb.Append("<th>Amount (RM)</th>");
            sb.Append("<th>Next Approver</th>");
            sb.Append("<th>Status</th>");
            sb.Append("</tr>");

            // Rows
            int index = 1;
            // Use dynamic to access properties regardless of the specific return type of GetForms
            foreach (dynamic item in dataList)
            {
                // Handle Status display logic
                string statusText = item.Status;
                if (statusText != null && statusText.Equals("SentBack", StringComparison.OrdinalIgnoreCase))
                {
                    statusText = "Sent Back";
                }

                // Handle Next Approver logic (Complete vs Name)
                string nextApprover = item.NextApprover;
                if (item.Status != null && item.Status.ToString().Equals("Completed", StringComparison.OrdinalIgnoreCase))
                {
                    nextApprover = "Complete";
                }

                sb.Append("<tr>");
                sb.Append($"<td style='text-align:center;'>{index++}</td>");
                sb.Append($"<td style='mso-number-format:\"\\@\";'>{item.Ref}</td>"); // Force text format for Ref

                // Handle Date (check if null or date object)
                string dateStr = "";
                if (item.Date != null)
                {
                    dateStr = item.Date is DateTime ? ((DateTime)item.Date).ToString("dd/MM/yyyy") : item.Date.ToString();
                }
                sb.Append($"<td>{dateStr}</td>");

                sb.Append($"<td>{item.Details}</td>");

                // Handle Amount
                string amountStr = item.Amount != null ? string.Format("{0:N2}", item.Amount) : "0.00";
                sb.Append($"<td style='text-align:right;'>{amountStr}</td>");

                sb.Append($"<td>{nextApprover}</td>");
                sb.Append($"<td>{statusText}</td>");
                sb.Append("</tr>");
            }

            sb.Append("</table>");
            sb.Append("</body></html>");

            // 4. Send Response
            string fileName = $"T1C_Budget_List_{DateTime.Now:yyyyMMdd_HHmm}.xls";
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

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)((LinkButton)sender).NamingContainer;
            string formId = ((HiddenField)row.FindControl("hdnFormId")).Value;
            Response.Redirect(Request.Url.GetCurrentUrl() + "/Edit?Id=" + formId);
        }

        protected void btnDeleteRecord_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var form = db.Forms.Find(Guid.Parse(hdnRecordId.Value));
                    bool isSuccess = db.SoftDelete(form);
                    if (isSuccess)
                    {
                        // Soft delete transactions related to the form
                        form.SoftDeleteTransactions();

                        // Delete form budgets related to the form
                        foreach (var formBudget in form.FormBudgets.ToList())
                        {
                            db.FormBudgets.Remove(formBudget);
                        }
                        db.SaveChanges();

                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Info, "Form deleted.");
                    }
                    else
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Failed to delete form.");
                    }
                }
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
            }

            Response.Redirect(Request.Url.GetCurrentUrl());
        }

        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["pageIndex"] = e.NewPageIndex.ToString();
            BindData(
                !string.IsNullOrEmpty(ddlYear.SelectedValue) ? int.Parse(ddlYear.SelectedValue) : (int?)null,
                txtRef.Text.Trim(),
                !string.IsNullOrEmpty(txtStartDate.Text) ? DateTime.Parse(txtStartDate.Text) : (DateTime?)null,
                !string.IsNullOrEmpty(txtEndDate.Text) ? DateTime.Parse(txtEndDate.Text) : (DateTime?)null,
                !string.IsNullOrEmpty(txtMinAmount.Text) ? decimal.Parse(txtMinAmount.Text.Replace(",", "")) : (decimal?)null,
                !string.IsNullOrEmpty(txtMaxAmount.Text) ? decimal.Parse(txtMaxAmount.Text.Replace(",", "")) : (decimal?)null,
                ddlStatus.SelectedValue
                );
        }

        private void BindControl()
        {
            int startYear = 2020;
            int currentYear = DateTime.Now.Year;

            for (int year = currentYear; year >= startYear; year--)
            {
                ListItem item = new ListItem(year.ToString(), year.ToString());
                if (year == currentYear)
                {
                    item.Selected = true; // auto select current year
                }
                ddlYear.Items.Add(item);
            }
        }
        private void BindData(int? year = null, string refNo = null, DateTime? startDate = null, DateTime? endDate = null, decimal? minAmount = null, decimal? maxAmount = null, string status = null)
        {
            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            var bizAreaCode = Auth.User().CCMSBizAreaCode;

            // Handle status list creation
            List<string> statuses = null;
            if (!string.IsNullOrEmpty(status))
            {
                statuses = new List<string> { status };
            }

            var form = new Class.Form();
            var formList = form.GetForms(
                year: year,
                bizAreaCode: bizAreaCode,
                refNo: refNo,
                startDate: startDate,
                endDate: endDate,
                amountMin: minAmount,
                amountMax: maxAmount,
                statuses: statuses // Pass status list
            );

            gvData.DataSource = formList;
            gvData.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
            gvData.DataBind();
        }
    }
}
//using CustomGuid.AspNet.Identity;
//using FGV.Prodata.App;
//using FGV.Prodata.Web.UI;
//using Prodata.WebForm.Helpers;
//using Prodata.WebForm.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;

//namespace Prodata.WebForm.T1C
//{
//    public partial class Default : ProdataPage
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            btnAdd.Visible = Auth.User().Can("t1c-add");

//            if (!IsPostBack)
//            {
//                BindControl();
//                BindData(!string.IsNullOrEmpty(ddlYear.SelectedValue) ? int.Parse(ddlYear.SelectedValue) : (int?)null);
//            }
//        }

//        protected void btnSearch_Click(object sender, EventArgs e)
//        {
//            BindData(
//                !string.IsNullOrEmpty(ddlYear.SelectedValue) ? int.Parse(ddlYear.SelectedValue) : (int?)null,
//                txtRef.Text.Trim(),
//                !string.IsNullOrEmpty(txtStartDate.Text) ? DateTime.Parse(txtStartDate.Text) : (DateTime?)null,
//                !string.IsNullOrEmpty(txtEndDate.Text) ? DateTime.Parse(txtEndDate.Text) : (DateTime?)null,
//                !string.IsNullOrEmpty(txtMinAmount.Text) ? decimal.Parse(txtMinAmount.Text.Replace(",", "")) : (decimal?)null,
//                !string.IsNullOrEmpty(txtMaxAmount.Text) ? decimal.Parse(txtMaxAmount.Text.Replace(",", "")) : (decimal?)null,
//                ddlStatus.SelectedValue
//                );
//            divCardSearch.Attributes["class"] = "card card-outline";
//        }

//        protected void btnEdit_Click(object sender, EventArgs e)
//        {
//            GridViewRow row = (GridViewRow)((LinkButton)sender).NamingContainer;
//            string formId = ((HiddenField)row.FindControl("hdnFormId")).Value;
//            Response.Redirect(Request.Url.GetCurrentUrl() + "/Edit?Id=" + formId);
//        }

//        protected void btnDeleteRecord_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                using (var db = new AppDbContext())
//                {
//                    var form = db.Forms.Find(Guid.Parse(hdnRecordId.Value));
//                    bool isSuccess = db.SoftDelete(form);
//                    if (isSuccess)
//                    {
//                        // Soft delete transactions related to the form
//                        form.SoftDeleteTransactions();

//                        // Delete form budgets related to the form
//                        foreach (var formBudget in form.FormBudgets.ToList())
//                        {
//                            db.FormBudgets.Remove(formBudget);
//                        }
//                        db.SaveChanges();

//                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Info, "Form deleted.");
//                    }
//                    else
//                    {
//                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Failed to delete form.");
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
//            }

//            Response.Redirect(Request.Url.GetCurrentUrl());
//        }

//        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
//        {
//            ViewState["pageIndex"] = e.NewPageIndex.ToString();
//            BindData(
//                !string.IsNullOrEmpty(ddlYear.SelectedValue) ? int.Parse(ddlYear.SelectedValue) : (int?)null,
//                txtRef.Text.Trim(),
//                !string.IsNullOrEmpty(txtStartDate.Text) ? DateTime.Parse(txtStartDate.Text) : (DateTime?)null,
//                !string.IsNullOrEmpty(txtEndDate.Text) ? DateTime.Parse(txtEndDate.Text) : (DateTime?)null,
//                !string.IsNullOrEmpty(txtMinAmount.Text) ? decimal.Parse(txtMinAmount.Text.Replace(",", "")) : (decimal?)null,
//                !string.IsNullOrEmpty(txtMaxAmount.Text) ? decimal.Parse(txtMaxAmount.Text.Replace(",", "")) : (decimal?)null,
//                ddlStatus.SelectedValue
//                );
//        }

//        private void BindControl()
//        {
//            int startYear = 2020;
//            int currentYear = DateTime.Now.Year;

//            for (int year = currentYear; year >= startYear; year--)
//            {
//                ListItem item = new ListItem(year.ToString(), year.ToString());
//                if (year == currentYear)
//                {
//                    item.Selected = true; // auto select current year
//                }
//                ddlYear.Items.Add(item);
//            }
//        }
//        private void BindData(int? year = null, string refNo = null, DateTime? startDate = null, DateTime? endDate = null, decimal? minAmount = null, decimal? maxAmount = null, string status = null)
//        {
//            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

//            var bizAreaCode = Auth.User().CCMSBizAreaCode;

//            // Handle status list creation
//            List<string> statuses = null;
//            if (!string.IsNullOrEmpty(status))
//            {
//                statuses = new List<string> { status };
//            }

//            var form = new Class.Form();
//            var formList = form.GetForms(
//                year: year,
//                bizAreaCode: bizAreaCode,
//                refNo: refNo,
//                startDate: startDate,
//                endDate: endDate,
//                amountMin: minAmount,
//                amountMax: maxAmount,
//                statuses: statuses // Pass status list
//            );

//            gvData.DataSource = formList;
//            gvData.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
//            gvData.DataBind();
//        }
//        private void BindDataOld(int? year = null, string refNo = null, DateTime? startDate = null, DateTime? endDate = null, decimal? minAmount = null, decimal? maxAmount = null)
//        {
//            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

//            var bizAreaCode = Auth.User().CCMSBizAreaCode;

//            var form = new Class.Form();
//            var formList = form.GetForms(year: year, bizAreaCode: bizAreaCode, refNo: refNo, startDate: startDate, endDate: endDate, amountMin: minAmount, amountMax: maxAmount);

//            gvData.DataSource = formList;
//            gvData.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
//            gvData.DataBind();
//        }
//    }
//}