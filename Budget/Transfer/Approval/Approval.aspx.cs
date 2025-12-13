using Antlr.Runtime.Misc;
using FGV.Prodata.Web.UI;
using Org.BouncyCastle.Asn1.Ocsp;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.Transfer.Approval
{
    public partial class Approval : ProdataPage
    {
        private Guid _transferId;
        private decimal esCost;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string idUserStr = Request.QueryString["userId"];

                // Redirect only if userId exists and does not match the current user
                if (!string.IsNullOrEmpty(idUserStr) && idUserStr != Auth.User().Id.ToString())
                {
                    Response.Redirect("~/Budget/Transfer/Approval");
                }

                if (Guid.TryParse(Request.QueryString["Id"], out _transferId))
                {
                    hdnTransferId.Value = _transferId.ToString();
                    LoadTransfer(_transferId);
                    LoadDocument(_transferId);
                    Loadhistory(_transferId);
                }
                else
                {
                    Response.Redirect("~/Budget/Transfer/Approval");
                }
            }
        }

        private void LoadTransfer(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var transfer = db.TransfersTransaction.FirstOrDefault(x => x.Id == id);
                if (transfer == null)
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Transfer not found.");
                    Response.Redirect("~/Budget/Transfer/Approval");
                    return;
                }

                // Header
                lblRefNo.Text = transfer.RefNo;
                lblProject.Text = transfer.Project;
                lblDate.Text = transfer.Date.ToString("yyyy-MM-dd");
                lblEstimatedCost.Text = transfer.EstimatedCost.ToString("F2");
                esCost = transfer.EstimatedCost;
                lblEVisa.Text = transfer.EVisaNo;
                lblBudgetType.Text = transfer.BudgetType;
                lblBA.Text = transfer.BA;

                // From Budget
                Guid FromBudgetTypeGuid = transfer.FromBudgetType;
                var FromBudgetType = db.BudgetTypes
                    .Where(x => x.Id == FromBudgetTypeGuid)
                    .Select(x => x.Name)
                    .FirstOrDefault();
                lblFromBudgetType.Text = FromBudgetType ?? "Unknown";
                lblFromBA.Text = transfer.FromBA;
                lblFromBudget.Text = (transfer.FromBudget ?? 0).ToString("F2");
                lblFromBalance.Text = (transfer.FromBalance ?? 0).ToString("F2");
                lblFromTransfer.Text = (transfer.FromTransfer ?? 0).ToString("F2");
                lblFromAfter.Text = (transfer.FromAfter ?? 0).ToString("F2");
                lblFromGL.Text = transfer.FromGL;

                // To Budget
                Guid ToBudgetTypeGuid = transfer.ToBudgetType;
                var ToBudgetType = db.BudgetTypes
                    .Where(x => x.Id == ToBudgetTypeGuid)
                    .Select(x => x.Name)
                    .FirstOrDefault();
                lblToBudgetType.Text = ToBudgetType ?? "Unknown";
                lblToBA.Text = transfer.ToBA;
                lblToBudget.Text = (transfer.ToBudget ?? 0).ToString("F2");
                lblToBalance.Text = (transfer.ToBalance ?? 0).ToString("F2");
                lblToTransfer.Text = (transfer.ToTransfer ?? 0).ToString("F2");
                lblToAfter.Text = (transfer.ToAfter ?? 0).ToString("F2");
                lblToGL.Text = transfer.ToGL;

                // Justification
                litJustification.Text = Server.HtmlEncode(transfer.Justification)?.Replace("\n", "<br/>");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            esCost = string.IsNullOrWhiteSpace(lblEstimatedCost.Text) ? 0 : Convert.ToDecimal(lblEstimatedCost.Text);
            HandleApprovalAction("Resubmit");
            UpdateStatusTransferTransaction(0);
            Response.Redirect("~/Budget/Transfer/Approval");
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            esCost = string.IsNullOrWhiteSpace(lblEstimatedCost.Text) ? 0 : Convert.ToDecimal(lblEstimatedCost.Text);
            HandleApprovalAction("Approved");
            UpdateStatusTransferTransaction(FindNextStatus());
            Response.Redirect("~/Budget/Transfer/Approval");
        }

        private int FindNextStatus()
        {
            //Nota
            //status == 0 ? "Resubmit" :
            //status == 1 ? "Submitted" :
            //status == 2 ? "Under Review" :
            //status == 3 ? "Completed" :
            int status = 2;
            if (Guid.TryParse(Request.QueryString["Id"], out _transferId))
            {
                using (var db = new AppDbContext())
                {
                    int currentorder = db.TransferApprovalLog.Where(x => x.BudgetTransferId == _transferId).OrderByDescending(x => x.ActionDate).Select(x => x.StepNumber).FirstOrDefault();
                    int currentLimitorder = db.TransferApprovalLimits
                        .Where(x => x.DeletedDate == null &&
                                    x.AmountMin <= esCost &&
                                    (x.AmountMax == null || esCost <= x.AmountMax))
                        .OrderByDescending(x => x.Order).Select(x => x.Order).FirstOrDefault().GetValueOrDefault();
                    if (currentorder == currentLimitorder)
                        status = 3;
                }
            }
            return status;
        }

        private void UpdateStatusTransferTransaction(int status)
        {
            if (Guid.TryParse(Request.QueryString["Id"], out _transferId))
            {
                using (var db = new AppDbContext())
                {
                    var model = db.TransfersTransaction.FirstOrDefault(x => x.Id == _transferId);
                    if (model == null)
                    {
                        return;
                    }

                    model.status = status;
                    db.SaveChanges();

                    string action = hdnAction.Value?.ToLower();

                    if (action != "approve" || status == 3) //resubmit or complete 
                    {
                        Emails.EmailsReqTransferBudgetForApprover(_transferId, model);
                    }
                    else if (action == "approve") //next approve  
                    {
                        Emails.EmailsReqTransferBudgetForApprover(_transferId, model, Auth.User().CCMSRoleCode);
                    }

                }
            }
        }

        private void HandleApprovalAction(string status)
        {
            string action = hdnAction.Value?.ToLower();
            if (string.IsNullOrEmpty(action) || (action != "approve" && action != "resubmit"))
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Warning, "Invalid action.");
                return;
            }

            if (!Guid.TryParse(hdnTransferId.Value, out Guid transferId))
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Warning, "Invalid transfer ID.");
                return;
            }

            string roleCode = Auth.User().CCMSRoleCode;
            Guid userId = Auth.User().Id;

            using (var db = new AppDbContext())
            {
                // Fetch approval config for current role once
                var approvalConfig = db.TransferApprovalLimits
                                        .Where(x =>
                                            x.TransApproverCode == roleCode &&
                                            x.DeletedDate == null &&
                                            x.AmountMin <= esCost &&
                                            (x.AmountMax == null || esCost <= x.AmountMax))
                                        .Select(x => new { x.Order, x.Section })
                                        .FirstOrDefault();

                int approvalStep = approvalConfig?.Order ?? 0;
                string section = approvalConfig?.Section ?? "Unknown";

                if (section == "Unknown")
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Warning, "This role has no approval authority.");
                    Response.Redirect("~/Budget/Transfer/Approval");
                    return;
                }

                // Reset to step 0 for resubmission
                if (status == "Resubmit")
                {
                    approvalStep = -1;
                }

                var logEntry = new TransferApprovalLog
                {
                    BudgetTransferId = transferId,
                    StepNumber = approvalStep,
                    RoleName = roleCode,
                    UserId = userId,
                    ActionType = section,
                    ActionDate = DateTime.Now,
                    Status = status,
                    // UPDATED: Using hidden field instead of TextBox
                    Remarks = hdnRemarks.Value?.Trim()
                };

                db.TransferApprovalLog.Add(logEntry);
                db.SaveChanges();

                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Transfer Budget approval recorded.");
            }
        }

        private void LoadDocument(Guid transferId)
        {
            using (var db = new AppDbContext())
            {
                var documents = db.TransferDocuments
                    .Where(d => d.TransferId == transferId)
                    .OrderByDescending(d => d.UploadedDate)
                    .ToList();

                if (documents.Any())
                {
                    pnlUploadedDocument.Visible = true;
                    phDocumentList.Controls.Clear();

                    foreach (var doc in documents)
                    {
                        var panel = new Panel { CssClass = "mb-2 d-flex align-items-center" };

                        var link = new HyperLink
                        {
                            NavigateUrl = "~/DocumentHandler.ashx?id=" + doc.Id + "&module=TransferDocuments",
                            CssClass = "btn btn-outline-success mr-2",
                            Target = "_blank",
                            Text = "<i class='fas fa-external-link-alt'></i> View"
                        };

                        var fileLabel = new Label
                        {
                            Text = $"<i class='fas fa-file-alt text-success mr-2'></i> {doc.FileName}",
                            CssClass = "text-dark font-weight-bold",
                            EnableViewState = false
                        };

                        panel.Controls.Add(link);
                        panel.Controls.Add(fileLabel);
                        phDocumentList.Controls.Add(panel);
                    }
                }
                else
                {
                    pnlUploadedDocument.Visible = false;
                }
            }
        }

        private void Loadhistory(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var query = db.TransferApprovalLog
                            .Where(x => x.DeletedDate == null && x.BudgetTransferId == id);

                var transfers = query
                    .OrderByDescending(x => x.ActionDate)
                    .Select(x => new
                    {
                        x.Id,
                        x.ActionDate,
                        x.ActionType,
                        x.RoleName,
                        x.Status,
                        x.Remarks
                    })
                    .ToList();

                gvHistory.DataSource = transfers;
                gvHistory.DataBind();
            }
        }
    }
}