using CustomGuid.AspNet.Identity;
using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.MasterData.AdditionalBudgetApprover
{
    public partial class Default : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindLoaFinance();
                BindLoaCogs();
            }
        }

        protected void btnDeleteRecordFinance_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var budgetApprover = db.AdditionalLoaFinanceLimits.Find(Guid.Parse(hdnRecordId.Value));
                    bool isSuccess = db.SoftDelete(budgetApprover);
                    if (isSuccess)
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Info, "Budget approver deleted.");
                    }
                    else
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Failed to delete budget approver.");
                    }
                }
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
            }

            Response.Redirect(Request.Url.GetCurrentUrl());
        }

        protected void btnDeleteRecordCogs_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var budgetApprover = db.AdditionalLoaCogsLimits.Find(Guid.Parse(hdnRecordId.Value));
                    bool isSuccess = db.SoftDelete(budgetApprover);
                    if (isSuccess)
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Info, "Budget approver deleted.");
                    }
                    else
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Failed to delete budget approver.");
                    }
                }
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
            }

            Response.Redirect(Request.Url.GetCurrentUrl());
        }

        protected void btnEdit1_Click(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)((LinkButton)sender).NamingContainer;
            string budgetApproverId = ((HiddenField)row.FindControl("hdnId1")).Value;
            string test = (Request.Url.GetCurrentUrl() + "/FinanceEdit?Id=" + budgetApproverId).ToString();

            Response.Redirect(Request.Url.GetCurrentUrl() + "/FinanceEdit?Id=" + budgetApproverId);
        }
        protected void btnEdit2_Click(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)((LinkButton)sender).NamingContainer;
            string budgetApproverId = ((HiddenField)row.FindControl("hdnId2")).Value;
            string test = (Request.Url.GetCurrentUrl() + "/CogsEdit?Id=" + budgetApproverId).ToString();

            Response.Redirect(Request.Url.GetCurrentUrl() + "/CogsEdit?Id=" + budgetApproverId);
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