using FGV.Prodata.Web.UI;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.GlobalBudget
{
    public partial class Default : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btnAdd.Visible = Auth.Can(Auth.Id(), "GlobalBudget-Add");
            if (!IsPostBack)
            {
                BindDropdowns();
                BindData();
            }
        }

        private void BindDropdowns()
        {
            // Bind Years (Current -1 to +5)
            int currentYear = DateTime.Now.Year;
            ddlYear.Items.Clear();
            ddlYear.Items.Add(new ListItem("All Years", ""));
            for (int i = currentYear - 1; i <= currentYear + 5; i++)
            {
                ddlYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
            ddlYear.SelectedValue = DateTime.Now.Year.ToString();

            // Bind Budget Types
            using (var db = new AppDbContext())
            {
                var types = db.BudgetTypes
                    .Where(x => x.DeletedDate == null && x.BudgetCategories == 3)
                    .OrderBy(x => x.Name)
                    .Select(x => new { x.Id, x.Name })
                    .ToList();

                ddlBudgetType.DataSource = types;
                ddlBudgetType.DataBind();
                ddlBudgetType.Items.Insert(0, new ListItem("All Types", ""));
            }
        }
        private void BindData()
        {
            using (var db = new AppDbContext())
            {
                // Base query
                var query = from p in db.Budgets
                            join t in db.BudgetTypes on p.TypeId equals t.Id
                            where p.DeletedDate == null
                               && t.BudgetCategories == 3 // Filter for Pool Budgets
                            select new
                            {
                                p.Id,
                                p.Date,
                                // Explicitly create a 'Year' property for the GridView binding
                                Year = p.Date.HasValue ? p.Date.Value.Year : 0,
                                p.Ref,
                                p.TypeId,
                                BudgetType = t.Name,
                                p.Amount,
                                p.Details,
                                p.CreatedDate
                            };

                // Apply Filters
                if (!string.IsNullOrEmpty(ddlYear.SelectedValue))
                {
                    int y = int.Parse(ddlYear.SelectedValue);
                    // Filter using the computed property if supported, or raw date logic
                    query = query.Where(x => x.Year == y);
                }

                if (!string.IsNullOrEmpty(ddlBudgetType.SelectedValue))
                {
                    Guid tid = Guid.Parse(ddlBudgetType.SelectedValue);
                    query = query.Where(x => x.TypeId == tid);
                }

                // Ordering
                query = query.OrderByDescending(x => x.Year).ThenBy(x => x.BudgetType);

                gvData.DataSource = query.ToList();
                gvData.DataBind();
            }
        }
        //private void BindDataOld()
        //{
        //    using (var db = new AppDbContext())
        //    {
        //        // Base query
        //        var query = from p in db.PoolBudgets
        //                    join t in db.BudgetTypes on p.BudgetTypeId equals t.Id
        //                    where p.DeletedDate == null // Exclude soft-deleted
        //                    select new
        //                    {
        //                        p.Id,
        //                        p.Year,
        //                        p.BudgetTypeId,
        //                        BudgetType = t.Name,
        //                        p.Amount,
        //                        p.Description,
        //                        p.CreatedDate
        //                    };

        //        // Apply Filters
        //        if (!string.IsNullOrEmpty(ddlYear.SelectedValue))
        //        {
        //            int y = int.Parse(ddlYear.SelectedValue);
        //            query = query.Where(x => x.Year == y);
        //        }

        //        if (!string.IsNullOrEmpty(ddlBudgetType.SelectedValue))
        //        {
        //            Guid tid = Guid.Parse(ddlBudgetType.SelectedValue);
        //            query = query.Where(x => x.BudgetTypeId == tid);
        //        }

        //        // Ordering
        //        query = query.OrderByDescending(x => x.Year).ThenBy(x => x.BudgetType);

        //        gvData.DataSource = query.ToList();
        //        gvData.DataBind();
        //    }
        //}

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvData.PageIndex = 0;
            BindData();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ddlYear.SelectedIndex = 0;
            ddlBudgetType.SelectedIndex = 0;
            gvData.PageIndex = 0;
            BindData();
        }

        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvData.PageIndex = e.NewPageIndex;
            BindData();
        }

        protected void btnDeleteRecord_Click(object sender, EventArgs e)
        {
            try
            {
                Guid id = Guid.Parse(hdnRecordId.Value);

                using (var db = new AppDbContext())
                {
                    var item = db.Budgets.Find(id);
                    if (item != null)
                    {
                        // Soft Delete
                        item.DeletedDate = DateTime.Now;
                        item.DeletedBy = Auth.User().Id;

                        db.SaveChanges();

                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Record deleted successfully.");
                        BindData();
                    }
                    else
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Record not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error deleting record: " + ex.Message);
            }
        }
    }
}