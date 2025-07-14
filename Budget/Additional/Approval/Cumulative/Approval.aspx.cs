using FGV.Prodata.Web.UI;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.Additional.Approval.Cumulative
{
    public partial class Approval : ProdataPage
    {
        private Guid _transferId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string idStr = Request.QueryString["id"];
                string idUserStr = Request.QueryString["userId"];

                if (!string.IsNullOrEmpty(idUserStr) && idUserStr != Auth.User().Id.ToString())
                {
                    RedirectToDefault();
                    return;
                }

                if (Guid.TryParse(idStr, out Guid requestId))
                {
                    hdnTransferId.Value = requestId.ToString();
                    LoadBudgetRequest(requestId);
                    LoadDocument(requestId);
                    LoadHistory(requestId);
                }
                else
                {
                    RedirectToDefault();
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            decimal esCost = string.IsNullOrWhiteSpace(lblAdditionalBudget.Text) ? 0 : Convert.ToDecimal(lblAdditionalBudget.Text);

            if (!Guid.TryParse(hdnTransferId.Value, out Guid transferId))
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Warning, "Invalid transfer ID.");
                return;
            }

            if (HandleApprovalAction(transferId, esCost))
            {
                UpdateStatusTransferTransaction(transferId, 4);
                RedirectToDefault();
            }
        }

        private void RedirectToDefault()
        {
            Response.Redirect("~/Budget/Additional/Approval/Cumulative");
        }

        private bool HandleApprovalAction(Guid transferId, decimal esCost)
        {
            string roleCode = Auth.User().iPMSRoleCode;
            Guid userId = Auth.User().Id;

            using (var db = new AppDbContext())
            {
                var approvalConfig = db.AdditionalCumulativeLimits
                    .Where(x =>
                        x.CumulativeApproverCode == roleCode &&
                        x.DeletedDate == null &&
                        (x.AmountMax == null || esCost <= x.AmountMax))
                    .Select(x => new { x.Section })
                    .FirstOrDefault();

                if (approvalConfig == null)
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Warning, "This role has no approval authority.");
                    return false;
                } 
                var logEntry = new AdditionalBudgetLog
                {
                    BudgetTransferId = transferId,
                    StepNumber = 100,
                    RoleName = roleCode,
                    UserId = userId,
                    ActionType = approvalConfig.Section,
                    ActionDate = DateTime.Now,
                    Status = "Finalized",
                    Remarks = txtRemarks.Text?.Trim()
                };


                db.AdditionalBudgetLog.Add(logEntry);
                db.SaveChanges();

                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Additional Budget approval recorded.");
                return true;
            }
        }

        private void UpdateStatusTransferTransaction(Guid transferId, int status)
        {
            using (var db = new AppDbContext())
            {
                var model = db.AdditionalBudgetRequests.FirstOrDefault(x => x.Id == transferId);
                if (model == null) return;

                model.Status = status;
                db.SaveChanges();

                Emails.EmailsAdditionalBudgetForApprover(transferId, model);
            }
        }

        private void LoadBudgetRequest(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var model = db.AdditionalBudgetRequests.FirstOrDefault(x => x.Id == id);
                if (model == null)
                {
                    RedirectToDefault();
                    return;
                }

                LblBA.Text = model.BA ?? "-";
                LblBAName.Text = new IPMSBizArea().GetNameByCode(model.BA ?? "") ?? "-";
                lblBudgetType.Text = model.BudgetType ?? "-";
                lblProject.Text = model.Project ?? "-";
                lblRefNo.Text = model.RefNo ?? "-";
                lblDate.Text = model.ApplicationDate.ToString("yyyy-MM-dd");
                lblBudgetEstimate.Text = model.EstimatedCost.ToString("N2");
                lblEVisa.Text = string.IsNullOrWhiteSpace(model.EVisaNo) ? "-" : model.EVisaNo;
                lblRequestDetails.Text = string.IsNullOrWhiteSpace(model.RequestDetails) ? "-" : model.RequestDetails;
                lblReason.Text = string.IsNullOrWhiteSpace(model.Reason) ? "-" : model.Reason;
                lblCostCentre.Text = string.IsNullOrWhiteSpace(model.CostCentre) ? "-" : model.CostCentre;
                lblGL.Text = string.IsNullOrWhiteSpace(model.GLCode) ? "-" : model.GLCode;
                lblApprovedBudget.Text = model.ApprovedBudget?.ToString("N2") ?? "-";
                lblNewTotalBudget.Text = model.NewTotalBudget?.ToString("N2") ?? "-";
                lblAdditionalBudget.Text = model.AdditionalBudget?.ToString("N2") ?? "-";
                lblCheckType.Text = model.CheckType ?? "-";
            }
        }

        private void LoadDocument(Guid transferId)
        {
            using (var db = new AppDbContext())
            {
                var documents = db.AdditionalBudgetDocuments
                    .Where(d => d.TransferId == transferId)
                    .OrderByDescending(d => d.UploadedDate)
                    .ToList();

                pnlUploadedDocument.Visible = documents.Any();
                phDocumentList.Controls.Clear();

                foreach (var doc in documents)
                {
                    var panel = new Panel { CssClass = "mb-2 d-flex align-items-center" };

                    var link = new HyperLink
                    {
                        NavigateUrl = $"~/DocumentHandler.ashx?id={doc.Id}&module=AdditionalBudgetDocuments",
                        CssClass = "btn btn-outline-success mr-2",
                        Target = "_blank",
                        Text = "<i class='fas fa-external-link-alt'></i> View"
                    };

                    var fileLabel = new Label
                    {
                        Text = $"<i class='fas fa-file-alt text-success mr-2'></i> {doc.FileName}",
                        CssClass = "text-dark font-weight-bold"
                    };

                    panel.Controls.Add(link);
                    panel.Controls.Add(fileLabel);
                    phDocumentList.Controls.Add(panel);
                }
            }
        }

        private void LoadHistory(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var history = db.AdditionalBudgetLog
                    .Where(x => x.DeletedDate == null && x.BudgetTransferId == id)
                    .OrderByDescending(x => x.ActionDate)
                    .Select(x => new
                    {
                        x.ActionDate,
                        x.ActionType,
                        x.RoleName,
                        x.Status,
                        x.Remarks
                    })
                    .ToList();

                gvHistory.DataSource = history;
                gvHistory.DataBind();
            }
        }
    }
}
