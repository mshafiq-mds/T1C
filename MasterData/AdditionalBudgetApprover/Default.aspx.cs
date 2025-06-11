using CustomGuid.AspNet.Identity;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.MasterData.AdditionalBudgetApprover
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindLoaFinance();
                BindLoaCogs();
            }
        }

        protected void btnDeleteRecord_Click(object sender, EventArgs e)
        {
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
        }
        private void BindLoaFinance()
        { 
            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            var list = GetApprovalLoaFinanceLimits();

            gvLoaFinance.DataSource = list;
            gvLoaFinance.PageIndex = Convert.ToInt32(ViewState["pageIndex"]);
            gvLoaFinance.DataBind();
        }
        private void BindLoaCogs()
        {
            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            var list = GetApprovalLoaCogsLimits();

            gvLoaCogs.DataSource = list;
            gvLoaCogs.PageIndex = Convert.ToInt32(ViewState["pageIndex"]);
            gvLoaCogs.DataBind();
        }
        private List<Models.ViewModels.ApprovalLimitListViewModel> GetApprovalLoaFinanceLimits()
        {
            using (var db = new AppDbContext())
            {
                var query = db.AdditionalLoaFinanceLimits
                    .ExcludeSoftDeleted()
                    .OrderBy(x => x.AmountMin)
                    .ThenBy(x => x.AmountMax)
                    .ThenBy(x => x.Order)
                    .ToList();

                return query.Select(x => new Models.ViewModels.ApprovalLimitListViewModel
                {
                    Id = x.Id,
                    ApproverType = x.FinanceApproverType,
                    ApproverCode = x.FinanceApproverCode,
                    ApproverName = x.FinanceApproverName,
                    AmountMin = x.AmountMin.HasValue ? x.AmountMin.Value.ToString("#,##0.00") : string.Empty,
                    AmountMax = x.AmountMax.HasValue ? x.AmountMax.Value.ToString("#,##0.00") : "Unlimited",
                    Section = x.Section,
                    Status = x.Status,
                    Order = x.Order.ToString()
                }).ToList();
            }
        }

        private List<Models.ViewModels.ApprovalLimitListViewModel> GetApprovalLoaCogsLimits()
        {
            using (var db = new AppDbContext())
            {
                var query = db.AdditionalLoaCogsLimits
                    .ExcludeSoftDeleted()
                    .OrderBy(x => x.AmountMin)
                    .ThenBy(x => x.AmountMax)
                    .ThenBy(x => x.Order)
                    .ToList();

                return query.Select(x => new Models.ViewModels.ApprovalLimitListViewModel
                {
                    Id = x.Id,
                    ApproverType = x.CogsApproverType,
                    ApproverCode = x.CogsApproverCode,
                    ApproverName = x.CogsApproverName,
                    AmountMin = x.AmountMin.HasValue ? x.AmountMin.Value.ToString("#,##0.00") : string.Empty,
                    AmountMax = x.AmountMax.HasValue ? x.AmountMax.Value.ToString("#,##0.00") : "Unlimited",
                    Section = x.Section,
                    Status = x.Status,
                    Order = x.Order.ToString()
                }).ToList();
            }
        }


        protected void gvLoaFinance_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["pageIndex"] = e.NewPageIndex.ToString();
            BindLoaFinance();
        }

        protected void gvLoaCogs_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["pageIndex"] = e.NewPageIndex.ToString();
            BindLoaCogs();
        }
    }
}