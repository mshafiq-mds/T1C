using CustomGuid.AspNet.Identity;
using NPOI.SS.Formula.Functions;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.UploadFGVPISB
{
    public partial class View : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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


        //private void BindData(Guid? typeId = null, int? year = null, string bizArea = null)
        //{
        //    int selectedYear = year ?? DateTime.Now.Year;
        //    int pageIndex = ViewState["pageIndex"] is string indexStr && int.TryParse(indexStr, out int idx) ? idx : 0;

        //    using (var db = new AppDbContext())
        //    {
        //        var query = db.Budgets.ExcludeSoftDeleted()
        //            .Join(db.BudgetTypes,
        //                b => b.TypeId,
        //                t => t.Id,
        //                (b, t) => new { b, t.Name })
        //            .Where(x => x.b.Date.HasValue && x.b.Date.Value.Year == selectedYear && x.b.DeletedDate == null);

        //        if (typeId.HasValue)
        //            query = query.Where(x => x.b.TypeId == typeId.Value);

        //        if (!string.IsNullOrEmpty(bizArea))
        //            query = query.Where(x => x.b.BizAreaCode == bizArea);

        //        var list = query
        //            .OrderBy(x => x.b.Ref)
        //            .ToList()
        //            .Select((x, i) => new BudgetListViewModel
        //            {
        //                Id = x.b.Id,
        //                No = i + 1,
        //                Type = x.Name,
        //                BizAreaCode = x.b.BizAreaCode,
        //                BizAreaName = x.b.BizAreaName,
        //                Date = x.b.Date?.ToString("dd/MM/yyyy") ?? "",
        //                Month = x.b.Month?.ToString(),
        //                Ref = x.b.Ref,
        //                Name = x.b.Name,
        //                DisplayName = $"{x.b.Ref} - {x.b.Name}",
        //                Details = x.b.Details,
        //                Wages = FormatDecimal(x.b.Wages),
        //                Purchase = FormatDecimal(x.b.Purchase),
        //                Amount = FormatDecimal(x.b.Amount),
        //                Vendor = x.b.Vendor,
        //                Status = x.b.Status
        //            }).ToList();

        //        gvBudget.DataSource = list;
        //        gvBudget.PageIndex = pageIndex;
        //        gvBudget.DataBind();
        //    }
        //}
        //private void BindData(Guid? typeId = null, int? year = null, string bizArea = null)
        //{
        //    int selectedYear = year ?? DateTime.Now.Year;
        //    int pageIndex = int.TryParse(ViewState["pageIndex"]?.ToString(), out int idx) ? idx : 0;

        //    using (var db = new AppDbContext())
        //    {
        //        var budgets = db.Budgets
        //            .ExcludeSoftDeleted()
        //            .Where(b => b.Date.HasValue && b.Date.Value.Year == selectedYear && b.DeletedDate == null);

        //        if (typeId.HasValue)
        //            budgets = budgets.Where(b => b.TypeId == typeId.Value);

        //        if (!string.IsNullOrEmpty(bizArea))
        //            budgets = budgets.Where(b => b.BizAreaCode == bizArea);

        //        // Step 1: Project without formatting (still in database)
        //        var query = from b in budgets
        //                    join t in db.BudgetTypes on b.TypeId equals t.Id
        //                    orderby b.Ref
        //                    select new
        //                    {
        //                        b.Id,
        //                        Type = t.Name,
        //                        b.BizAreaCode,
        //                        b.BizAreaName,
        //                        b.Date,
        //                        b.Month,
        //                        b.Ref,
        //                        b.Name,
        //                        b.Details,
        //                        b.Wages,
        //                        b.Purchase,
        //                        b.Amount,
        //                        b.Vendor,
        //                        b.Status
        //                    };

        //        // Step 2: Materialize to memory, then format values
        //        var list = query
        //            .AsEnumerable()
        //            .Select(b => new BudgetListViewModel
        //            {
        //                Id = b.Id,
        //                Type = b.Type,
        //                BizAreaCode = b.BizAreaCode,
        //                BizAreaName = b.BizAreaName,
        //                Date = b.Date.HasValue ? b.Date.Value.ToString("dd/MM/yyyy") : "",
        //                Month = b.Month.HasValue ? b.Month.ToString() : "",
        //                Ref = b.Ref,
        //                Name = b.Name,
        //                DisplayName = $"{b.Ref} - {b.Name}",
        //                Details = b.Details,
        //                Wages = FormatDecimal(b.Wages),
        //                Purchase = FormatDecimal(b.Purchase),
        //                Amount = FormatDecimal(b.Amount),
        //                Vendor = b.Vendor,
        //                Status = b.Status
        //            }).ToList();

        //        gvBudget.PageIndex = pageIndex;
        //        gvBudget.DataSource = list;
        //        gvBudget.DataBind();
        //    }
        //}
        private void BindData(Guid? typeId = null, int? year = null, string bizArea = null)
        {
            int selectedYear = year ?? DateTime.Now.Year;
            int pageIndex = int.TryParse(ViewState["pageIndex"]?.ToString(), out int idx) ? idx : 0;

            using (var db = new AppDbContext())
            {
                // 🔎 Base query (No formatting)
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

                // 📌 Step 2: Get Utilized Transactions
                var utilizedMap = db.Transactions.ExcludeSoftDeleted()
                    .Where(t =>
                        (t.FromId.HasValue && budgetIds.Contains(t.FromId.Value) && t.FromType.ToLower() == "budget") ||
                        (t.ToId.HasValue && budgetIds.Contains(t.ToId.Value) && t.ToType.ToLower() == "budget"))
                    .Where(t => t.Status.ToLower() != "rejected")
                    .GroupBy(t => t.FromType.ToLower() == "budget" ? t.FromId : t.ToId)
                    .ToDictionary(
                        g => g.Key.Value,
                        g => g.Sum(t =>
                        {
                            decimal amt = t.Amount ?? 0m;
                            return t.FromType.ToLower() == "budget" ? amt : -amt;
                        })
                    );

                // 📌 Step 3: Create GridView list including BUDGET + BALANCE
                var list = query
                .OrderBy(x => x.BizAreaCode)       // <-- 👍 Sort here
                .ThenBy(x => x.Ref)
                .Select(q =>
                {
                    decimal budget = q.Amount ?? 0m;
                    decimal utilized = utilizedMap.ContainsKey(q.Id) ? utilizedMap[q.Id] : 0m;
                    decimal balance = budget - utilized;

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

                        // 🟦 Shows Original Budget
                        Amount = FormatDecimal(budget),

                        // 🟨 Extra Column for Balance
                        Balance = FormatDecimal(balance),

                        Wages = FormatDecimal(q.Wages),
                        Purchase = FormatDecimal(q.Purchase),
                        Vendor = q.Vendor,
                        Status = q.Status
                    };
                })               // (Optional) secondary sorting
                .ToList();
                Session["BudgetList"] = list;

                gvBudget.PageIndex = pageIndex;
                gvBudget.DataSource = list;
                gvBudget.DataBind();
            }
        }

        protected void gvBudget_Sorting(object sender, GridViewSortEventArgs e)
        {
            // Get existing list from Session or re-bind
            var list = Session["BudgetList"] as List<BudgetListViewModel>;
            if (list == null)
            {
                BindData();
                list = Session["BudgetList"] as List<BudgetListViewModel>;
            }

            // Determine sort direction
            string sortDirection = "ASC";
            if (ViewState["SortExpression"]?.ToString() == e.SortExpression)
            {
                sortDirection = ViewState["SortDirection"]?.ToString() == "ASC" ? "DESC" : "ASC";
            }

            ViewState["SortExpression"] = e.SortExpression;
            ViewState["SortDirection"] = sortDirection;

            // Use LINQ to sort
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

            ViewState["pageIndex"] = 0; // Reset to first page if needed
            BindData(); // No filters applied
        }


    }
}
