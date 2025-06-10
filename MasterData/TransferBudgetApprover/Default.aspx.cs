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

namespace Prodata.WebForm.MasterData.TransferBudgetApprover
{
    public partial class Default : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btnAdd.Visible = Auth.User().Can("add-transfer-budget-approver");
            if (!IsPostBack)
            {
                BindData();
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)((LinkButton)sender).NamingContainer;
            string budgetApproverId = ((HiddenField)row.FindControl("hdnId")).Value;
            string test = (Request.Url.GetCurrentUrl() + "/Edit?Id=" + budgetApproverId).ToString();
            Response.Redirect("~/"+Request.Url.GetCurrentUrl() + "/Edit?Id=" + budgetApproverId);
        }

        protected void btnDeleteRecord_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var budgetApprover = db.TransferApprovalLimits.Find(Guid.Parse(hdnRecordId.Value));
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

        protected void gvBudgetApprover_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["pageIndex"] = e.NewPageIndex.ToString();
            BindData();
        }

        private void BindData()
        {
            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            var list = GetTransferApprovalLimits();

            gvBudgetApprover.DataSource = list;
            gvBudgetApprover.PageIndex = Convert.ToInt32(ViewState["pageIndex"]);
            gvBudgetApprover.DataBind();
        }

        private List<Models.ViewModels.TransferApprovalLimitListViewModel> GetTransferApprovalLimits()
        {
            using (var db = new AppDbContext())
            {
                var query = db.TransferApprovalLimits
                    .ExcludeSoftDeleted()
                    .OrderBy(x => x.AmountMin)
                    .ThenBy(x => x.AmountMax)
                    .ThenBy(x => x.Order)
                    .ToList();

                return query.Select(x => new Models.ViewModels.TransferApprovalLimitListViewModel
                {
                    Id = x.Id,
                    TransApproverType = x.TransApproverType,
                    TransApproverCode = x.TransApproverCode,
                    TransApproverName = x.TransApproverName,
                    AmountMin = x.AmountMin.HasValue ? x.AmountMin.Value.ToString("#,##0.00") : string.Empty,
                    AmountMax = x.AmountMax.HasValue ? x.AmountMax.Value.ToString("#,##0.00") : "Unlimited",
                    Section = x.Section,
                    Status = x.Status,
                    Order = x.Order.ToString()
                }).ToList();
            }
        }
    }
}