using FGV.Prodata.Web.UI;
using Org.BouncyCastle.Asn1.Ocsp;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.Additional.Approval.Cumulative
{
    public partial class Approval : ProdataPage
    {
        private Guid _transferId;
        private decimal esCost;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string idStr = Request.QueryString["id"];
                string idUserStr = Request.QueryString["userId"];

                // Redirect only if userId exists and does not match the current user
                if (!string.IsNullOrEmpty(idUserStr) && idUserStr != Auth.User().Id.ToString())
                {
                    Response.Redirect("~/Budget/Additional/Approval/Cumulative");
                }

                if (Guid.TryParse(idStr, out Guid requestId))
                {
                    hdnTransferId.Value = requestId.ToString();
                    LoadBudgetRequest(requestId);
                    LoadDocument(requestId);
                    Loadhistory(requestId);
                }
                else
                {
                    Response.Redirect("~/Budget/Additional/Approval/Cumulative");
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            esCost = string.IsNullOrWhiteSpace(lblAdditionalBudget.Text) ? 0 : Convert.ToDecimal(lblAdditionalBudget.Text);
            HandleApprovalAction("Resubmit");
            UpdateStatusTransferTransaction(0);
            Response.Redirect("~/Budget/Additional/Approval/Cumulative");
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            esCost = string.IsNullOrWhiteSpace(lblAdditionalBudget.Text) ? 0 : Convert.ToDecimal(lblAdditionalBudget.Text);
            HandleApprovalAction("Approved");
            UpdateStatusTransferTransaction(FindNextStatus());
            Response.Redirect("~/Budget/Additional/Approval/Cumulative");
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
                    int currentorder = db.AdditionalBudgetLog.Where(x => x.BudgetTransferId == _transferId).OrderByDescending(x => x.ActionDate).Select(x => x.StepNumber).FirstOrDefault();
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
            //Nota
            //status == 0 ? "Resubmit" :
            //status == 1 ? "Submitted" :
            //status == 2 ? "Under Review" :
            //status == 3 ? "Completed" :
            if (Guid.TryParse(Request.QueryString["Id"], out _transferId))
            {
                using (var db = new AppDbContext())
                {
                    var model = db.AdditionalBudgetRequests.FirstOrDefault(x => x.Id == _transferId);
                    if (model == null)
                    {
                        return;
                    }

                    model.Status = status;

                    db.SaveChanges();
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

            string roleCode = Auth.User().iPMSRoleCode;
            Guid userId = Auth.User().Id;

            using (var db = new AppDbContext())
            {
                // Fetch approval config for current role once
                var approvalConfig = db.AdditionalLoaCogsLimits
                                        .Where(x =>
                                            x.CogsApproverCode == roleCode &&
                                            x.DeletedDate == null &&
                                            x.AmountMin <= esCost &&
                                            (x.AmountMax == null || esCost <= x.AmountMax))
                                        .Select(x => new { x.Order, x.Section })
                                        .FirstOrDefault();

                int approvalStep = approvalConfig?.Order ?? 0;
                string section = approvalConfig?.Section ?? "Unknown";

                // Block if role has no approval authority and trying to approve
                //if (status == "Approved" && approvalStep == 0)
                if (section == "Unknown")
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Warning, "This role has no approval authority.");
                    Response.Redirect("~/Budget/Additional/Approval/Cumulative");
                    return;
                }

                // Reset to step 0 for resubmission
                if (status == "Resubmit")
                {
                    approvalStep = -1;
                }

                var logEntry = new AdditionalBudgetLog
                {
                    BudgetTransferId = transferId,
                    StepNumber = approvalStep,
                    RoleName = roleCode,
                    UserId = userId,
                    ActionType = section,
                    ActionDate = DateTime.Now,
                    Status = status,
                    Remarks = txtRemarks.Text?.Trim()
                };

                db.AdditionalBudgetLog.Add(logEntry);
                db.SaveChanges();

                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Additional Budget approval recorded.");
            }
        }
        private void LoadBudgetRequest(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var model = db.AdditionalBudgetRequests.FirstOrDefault(x => x.Id == id);
                if (model == null)
                {
                    Response.Redirect("~/Budget/Additional/Approval/Cumulative");
                    return;
                }

                LblBA.Text = model.BA ?? "-";
                LblBAName.Text = new Class.IPMSBizArea().GetNameByCode(model.BA ?? "") ?? "-";

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

                lblApprovedBudget.Text = model.ApprovedBudget.HasValue ? model.ApprovedBudget.Value.ToString("N2") : "-";
                lblNewTotalBudget.Text = model.NewTotalBudget.HasValue ? model.NewTotalBudget.Value.ToString("N2") : "-";
                lblAdditionalBudget.Text = model.AdditionalBudget.HasValue ? model.AdditionalBudget.Value.ToString("N2") : "-";

                lblCheckType.Text = model.CheckType;

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

                if (documents.Any())
                {
                    pnlUploadedDocument.Visible = true;
                    phDocumentList.Controls.Clear();
                    foreach (var doc in documents)
                    {
                        var panel = new Panel { CssClass = "mb-2 d-flex align-items-center" };

                        var link = new HyperLink
                        {
                            NavigateUrl = "~/DocumentHandler.ashx?id=" + doc.Id + "&module=AdditionalBudgetDocuments",
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
                var query = db.AdditionalBudgetLog
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