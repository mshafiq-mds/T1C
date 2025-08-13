using CustomGuid.AspNet.Identity;
using NPOI.SS.Formula.Functions;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.ViewModels;
using System;
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
            ddlYear.Attributes["data-placeholder"] = currentYear.ToString();
            ddlYear.Items.Clear();
            //ddlYear.Items.Add(new ListItem("-- Select Year --", ""));
            for (int i = currentYear; i >= 2020; i--)
                ddlYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }

        private void BindData(Guid? typeId = null, int? year = null, string bizArea = null)
        {
            int selectedYear = year ?? DateTime.Now.Year;
            int pageIndex = ViewState["pageIndex"] is string indexStr && int.TryParse(indexStr, out int idx) ? idx : 0;

            using (var db = new AppDbContext())
            {
                var query = db.Budgets.ExcludeSoftDeleted()
                    .Join(db.BudgetTypes,
                        b => b.TypeId,
                        t => t.Id,
                        (b, t) => new { b, t.Name })
                    .Where(x => x.b.Date.HasValue && x.b.Date.Value.Year == selectedYear);

                if (typeId.HasValue)
                    query = query.Where(x => x.b.TypeId == typeId.Value);

                if (!string.IsNullOrEmpty(bizArea))
                    query = query.Where(x => x.b.BizAreaCode == bizArea);

                var list = query
                    .OrderBy(x => x.b.Ref)
                    .ToList()
                    .Select((x, i) => new BudgetListViewModel
                    {
                        Id = x.b.Id,
                        No = i + 1,
                        Type = x.Name,
                        BizAreaCode = x.b.BizAreaCode,
                        BizAreaName = x.b.BizAreaName,
                        Date = x.b.Date?.ToString("dd/MM/yyyy") ?? "",
                        Month = x.b.Month?.ToString(),
                        Ref = x.b.Ref,
                        Name = x.b.Name,
                        DisplayName = $"{x.b.Ref} - {x.b.Name}",
                        Details = x.b.Details,
                        Wages = FormatDecimal(x.b.Wages),
                        Purchase = FormatDecimal(x.b.Purchase),
                        Amount = FormatDecimal(x.b.Amount),
                        Vendor = x.b.Vendor,
                        Status = x.b.Status
                    }).ToList();

                gvBudget.DataSource = list;
                gvBudget.PageIndex = pageIndex;
                gvBudget.DataBind();
            }
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
