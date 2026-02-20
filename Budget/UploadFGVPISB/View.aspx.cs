using CustomGuid.AspNet.Identity;
using FGV.Prodata.Web.UI;
using NPOI.SS.Formula.Functions;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;   // Required for Excel Export
using System.Text; // Required for Excel Export

namespace Prodata.WebForm.Budget.UploadFGVPISB
{
    public partial class View : ProdataPage
    {

        protected bool CanEdit { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            var user = Auth.User();

            // Check 1: User must be Admin/HQ (CCMSBizAreaCode is empty)
            bool isHQ = user != null && string.IsNullOrEmpty(user.CCMSBizAreaCode);

            // Check 2: User must have specific 'edit-budget' permission
            bool hasPermission = Auth.Can(Auth.Id(), "edit-budget");

            // Combine checks: Must be HQ AND have permission
            CanEdit = isHQ && hasPermission;

            foreach (DataControlField col in gvBudget.Columns)
            {
                if (col.HeaderText == "Action")
                {
                    col.Visible = CanEdit;
                    break;
                }
            }
            if (!IsPostBack)
            {
                BindYearDropdown();
                BindDropdown(ddlBT, Functions.GetBudgetTypes(), "ID", "DisplayName");
                BindDropdown(ddlBA, new Class.IPMSBizArea().GetIPMSBizAreas(), "Code", "DisplayName");
                BindData();
            }
        }

        private void BindDropdown(ListControl ddl, object dataSource, string valueField, string textField)
        {
            ddl.DataSource = dataSource;
            ddl.DataValueField = valueField;
            ddl.DataTextField = textField;
            ddl.DataBind();
            //ddl.Items.Insert(0, new ListItem("-- Select --", ""));
        }

        private void BindYearDropdown()
        {
            int currentYear = DateTime.Now.Year;

            ddlYear.Items.Clear();

            // Populate dropdown (current year +1 down to -3)
            for (int i = currentYear + 1; i >= currentYear - 3; i--)
                ddlYear.Items.Add(new ListItem(i.ToString(), i.ToString()));

            // Auto-select current year
            ddlYear.SelectedValue = currentYear.ToString();
        }

        // ==================================================================================
        // NEW EXPORT TO EXCEL FUNCTION
        // ==================================================================================
        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            // 1. Get Data
            var list = GetBudgetList(GetSelectedTypeId(), GetSelectedYear(), GetSelectedBizArea());

            // 2. Build HTML
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head><meta http-equiv='content-type' content='application/vnd.ms-excel; charset=utf-8' /></head><body>");
            sb.Append("<table border='1' style='border-collapse:collapse;'>");

            // Header
            sb.Append("<tr style='background-color:#f2f2f2;'>");
            sb.Append("<th>#</th>");
            sb.Append("<th>No. Rujukan</th>");
            sb.Append("<th>BA</th>");
            sb.Append("<th>Projek</th>");
            sb.Append("<th>Butir-butir Kerja</th>");
            sb.Append("<th>Bulan</th>");
            sb.Append("<th>Vendor</th>");
            sb.Append("<th>Upah (RM)</th>");
            sb.Append("<th>Belian Alat Ganti (RM)</th>");
            sb.Append("<th>Budget (RM)</th>");
            sb.Append("<th>Balance (RM)</th>");
            sb.Append("</tr>");

            int index = 1;
            foreach (var item in list)
            {
                sb.Append("<tr>");
                sb.Append($"<td style='text-align:center;'>{index++}</td>");
                sb.Append($"<td style='mso-number-format:\"\\@\";'>{item.Ref}</td>"); // Text format
                sb.Append($"<td>{item.BizAreaCode}</td>");
                sb.Append($"<td>{item.BizAreaName}</td>");
                sb.Append($"<td>{item.Details}</td>");
                sb.Append($"<td style='text-align:center;'>{item.Month}</td>");
                sb.Append($"<td>{item.Vendor}</td>");

                // Numbers
                sb.Append($"<td style='text-align:right;'>{item.Wages}</td>");
                sb.Append($"<td style='text-align:right;'>{item.Purchase}</td>");
                sb.Append($"<td style='text-align:right;'>{item.Amount}</td>");
                sb.Append($"<td style='text-align:right;'>{item.Balance}</td>");
                sb.Append("</tr>");
            }

            sb.Append("</table>");
            sb.Append("</body></html>");

            // 3. Send Response
            string fileName = $"Budget_List_{DateTime.Now:yyyyMMdd_HHmm}.xls";
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

        private void BindData(Guid? typeId = null, int? year = null, string bizArea = null)
        {
            int pageIndex = int.TryParse(ViewState["pageIndex"]?.ToString(), out int idx) ? idx : 0;

            // Use the shared method to get data
            var list = GetBudgetList(typeId, year, bizArea);

            Session["BudgetList"] = list;

            gvBudget.PageIndex = pageIndex;
            gvBudget.DataSource = list;
            gvBudget.DataBind();
        }

        // Shared method for binding and exporting
        private List<BudgetListViewModel> GetBudgetList(Guid? typeId = null, int? year = null, string bizArea = null)
        {
            int selectedYear = year ?? DateTime.Now.Year;

            using (var db = new AppDbContext())
            {
                // 🔎 Base query
                var budgets = db.Budgets
                    .ExcludeSoftDeleted()
                    .Where(b => b.Date.HasValue && b.Date.Value.Year == selectedYear);

                if (typeId.HasValue)
                    budgets = budgets.Where(b => b.TypeId == typeId.Value);

                if (!string.IsNullOrEmpty(bizArea))
                    budgets = budgets.Where(b => b.BizAreaCode == bizArea);

                var query = (from b in budgets
                             join t in db.BudgetTypes on b.TypeId equals t.Id
                             orderby b.Ref
                             select new
                             {
                                 b.Id,
                                 b.TypeId,
                                 Type = t.Name,
                                 b.BizAreaCode,
                                 b.BizAreaName,
                                 b.Date,
                                 b.Month,
                                 b.Ref,
                                 b.Name,
                                 b.Details,
                                 b.Wages,
                                 b.Purchase,
                                 b.Amount,
                                 b.Vendor,
                                 b.Status
                             }).ToList();

                // 📌 Step 1: Extract Budget IDs
                var budgetIds = query.Select(q => q.Id).ToList();

                // 📌 Step 2: Get Standard Utilized Transactions (POs, Actuals, Outgoing Transfers)
                var utilizedMap = db.Transactions.ExcludeSoftDeleted()
                    .Where(t =>
                        (t.FromId.HasValue && budgetIds.Contains(t.FromId.Value) && t.FromType.ToLower() == "budget") ||
                        (t.ToId.HasValue && budgetIds.Contains(t.ToId.Value) && t.ToType.ToLower() == "budget"))
                    .Where(t => t.Status.ToLower() != "rejected")
                    .ToList()
                    .GroupBy(t => t.FromType.ToLower() == "budget" ? t.FromId : t.ToId)
                    .ToDictionary(
                        g => g.Key.Value,
                        g => g.Sum(t =>
                        {
                            decimal amt = t.Amount ?? 0m;
                            return t.FromType.ToLower() == "budget" ? amt : -amt;
                        })
                    );

                // 📌 Step 2.5: Get Incoming Budget Transfers (ADD to Amount)
                var transfersInMap = db.Transactions.ExcludeSoftDeleted()
                    .Where(t => t.ToId.HasValue && budgetIds.Contains(t.ToId.Value))
                    .Where(t => t.FromType == "Budget" && t.ToType == "Budget")
                    .Where(t => t.Status.ToLower() != "rejected")
                    .GroupBy(t => t.ToId.Value)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(t => t.Amount ?? 0m)
                    );

                // 📌 Step 3: Create list
                var list = query
                .OrderBy(x => x.BizAreaCode)
                .ThenBy(x => x.Ref)
                .Select(q =>
                {
                    decimal baseBudget = q.Amount ?? 0m;
                    decimal transferIn = transfersInMap.ContainsKey(q.Id) ? transfersInMap[q.Id] : 0m;
                    decimal totalBudget = baseBudget + transferIn;
                    decimal utilized = utilizedMap.ContainsKey(q.Id) ? utilizedMap[q.Id] : 0m;
                    decimal balance = totalBudget - utilized;

                    return new BudgetListViewModel
                    {
                        Id = q.Id,
                        Type = q.Type,
                        BizAreaCode = q.BizAreaCode,
                        BizAreaName = q.BizAreaName,
                        Date = q.Date.HasValue ? q.Date.Value.ToString("dd/MM/yyyy") : "",
                        Month = q.Month.HasValue ? q.Month.ToString() : "",
                        Ref = q.Ref,
                        Name = q.Name,
                        DisplayName = $"{q.Ref} - {q.Name}",
                        Details = q.Details,
                        Amount = FormatDecimal(totalBudget), // Adjusted Budget
                        Balance = FormatDecimal(balance),    // Calculated Balance
                        Wages = FormatDecimal(q.Wages),
                        Purchase = FormatDecimal(q.Purchase),
                        Vendor = q.Vendor,
                        Status = q.Status
                    };
                })
                .ToList();

                return list;
            }
        }

        protected void gvBudget_Sorting(object sender, GridViewSortEventArgs e)
        {
            var list = Session["BudgetList"] as List<BudgetListViewModel>;
            if (list == null)
            {
                BindData();
                list = Session["BudgetList"] as List<BudgetListViewModel>;
            }

            string sortDirection = "ASC";
            if (ViewState["SortExpression"]?.ToString() == e.SortExpression)
            {
                sortDirection = ViewState["SortDirection"]?.ToString() == "ASC" ? "DESC" : "ASC";
            }

            ViewState["SortExpression"] = e.SortExpression;
            ViewState["SortDirection"] = sortDirection;

            if (sortDirection == "ASC")
                list = list.OrderBy(x => GetPropertyValue(x, e.SortExpression)).ToList();
            else
                list = list.OrderByDescending(x => GetPropertyValue(x, e.SortExpression)).ToList();

            gvBudget.DataSource = list;
            gvBudget.DataBind();
        }

        private object GetPropertyValue(object obj, string propName)
        {
            return obj.GetType().GetProperty(propName)?.GetValue(obj, null);
        }

        private string FormatDecimal(decimal? value) => value?.ToString("N2") ?? "";

        protected void ddlFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindData(GetSelectedTypeId(), GetSelectedYear(), GetSelectedBizArea());
        }

        protected void gvBudget_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["pageIndex"] = e.NewPageIndex.ToString();
            BindData(GetSelectedTypeId(), GetSelectedYear(), GetSelectedBizArea());
        }

        private Guid? GetSelectedTypeId() =>
            Guid.TryParse(ddlBT.SelectedValue, out Guid typeId) ? typeId : (Guid?)null;

        private int GetSelectedYear() =>
            int.TryParse(ddlYear.SelectedValue, out int year) ? year : DateTime.Now.Year;

        private string GetSelectedBizArea() =>
            string.IsNullOrWhiteSpace(ddlBA.SelectedValue) ? null : ddlBA.SelectedValue;

        protected void btnClearFilter_Click(object sender, EventArgs e)
        {
            ddlBT.ClearSelection();
            ddlYear.ClearSelection();
            ddlBA.ClearSelection();

            ViewState["pageIndex"] = 0;
            BindData();
        }
    }
}