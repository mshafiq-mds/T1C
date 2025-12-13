using FGV.Prodata.Web.UI;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.Transfer
{
    public partial class Resubmit : ProdataPage
    {
        private Guid _transferId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Security Check: Ensure the logged-in user matches the userId in QueryString
                string idUserStr = Request.QueryString["userId"];
                if (!string.IsNullOrEmpty(idUserStr) && idUserStr != Auth.User().Id.ToString())
                {
                    Response.Redirect("~/Budget/Transfer/Default");
                }

                // Parse ID and Load Data
                if (Guid.TryParse(Request.QueryString["Id"], out _transferId))
                {
                    LoadTransfer(_transferId);
                    LoadDocument(_transferId);
                    Loadhistory(_transferId);
                }
                else
                {
                    Response.Redirect("~/Budget/Transfer/Default");
                }
            }
        }

        #region Loading Data Methods

        private void LoadTransfer(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var transfer = db.TransfersTransaction.FirstOrDefault(x => x.Id == id);
                if (transfer == null)
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Transfer record not found.");
                    Response.Redirect("~/Budget/Transfer/Default");
                    return;
                }

                // Bind Header Info
                txtRefNo.Text = transfer.RefNo;
                txtProject.Text = transfer.Project;
                txtDate.Text = transfer.Date.ToString("yyyy-MM-dd");
                txtEstimatedCost.Text = transfer.EstimatedCost.ToString("F2");
                txtEVisa.Text = transfer.EVisaNo;
                txtWorkDetails.Text = transfer.WorkDetails;
                txtJustification.Text = transfer.Justification;
                LblBA.Text = transfer.BA;

                // Bind Radio Buttons
                rdoOpex.Checked = transfer.BudgetType == "OPEX";
                rdoCapex.Checked = transfer.BudgetType == "CAPEX";

                // Bind From Budget Info
                Guid FromBudgetTypeGuid = transfer.FromBudgetType;
                var FromBudgetTypeName = db.BudgetTypes
                    .Where(x => x.Id == FromBudgetTypeGuid)
                    .Select(x => x.Name)
                    .FirstOrDefault();

                txtFromBudgetType.Text = FromBudgetTypeName ?? "Unknown";
                ddFromBA.Text = transfer.FromBA;
                txtFromBudget.Text = (transfer.FromBudget ?? 0).ToString("F2");
                txtFromBalance.Text = (transfer.FromBalance ?? 0).ToString("F2");
                txtFromTransfer.Text = (transfer.FromTransfer ?? 0).ToString("F2");
                txtFromAfter.Text = (transfer.FromAfter ?? 0).ToString("F2");
                txtFromGL.Text = transfer.FromGL;

                // Bind To Budget Info
                Guid ToBudgetTypeGuid = transfer.ToBudgetType;
                var ToBudgetTypeName = db.BudgetTypes
                    .Where(x => x.Id == ToBudgetTypeGuid)
                    .Select(x => x.Name)
                    .FirstOrDefault();

                txtToBudgetType.Text = ToBudgetTypeName ?? "Unknown";
                ddToBA.Text = transfer.ToBA;
                txtToBudget.Text = (transfer.ToBudget ?? 0).ToString("F2");
                txtToBalance.Text = (transfer.ToBalance ?? 0).ToString("F2");
                txtToTransfer.Text = (transfer.ToTransfer ?? 0).ToString("F2");
                txtToAfter.Text = (transfer.ToAfter ?? 0).ToString("F2");
                txtToGL.Text = transfer.ToGL;
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

                // Store the most recent RoleName to know who sent it back (for email purposes)
                hdncurentRoleApprover.Value = transfers.Any() ? transfers.First().RoleName : "";

                if (transfers.Any())
                {
                    pnHistoryApproval.Visible = true;
                }
                gvHistory.DataSource = transfers;
                gvHistory.DataBind();
            }
        }

        #endregion

        #region Submission Logic

        // This method is triggered by the hidden button (btnConfirmSubmit) 
        // after the user clicks "Yes" in the SweetAlert
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // 1. Server-Side Validation (Fallback)
            if (string.IsNullOrWhiteSpace(txtResubmit.Text))
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Warning, "Remarks are required for resubmission.");
                return;
            }

            // 2. Parse ID
            if (Guid.TryParse(Request.QueryString["Id"], out _transferId))
            {
                using (var db = new AppDbContext())
                {
                    var model = db.TransfersTransaction.FirstOrDefault(x => x.Id == _transferId);
                    if (model == null)
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Transfer record not found.");
                        return;
                    }

                    // 3. Update Transaction Status
                    // Status 2 = "Submitted" / "Under Review" (Depending on your workflow enum)
                    model.status = 2;
                    model.UpdatedBy = Auth.User().Id;
                    model.UpdatedDate = DateTime.Now;

                    db.SaveChanges();

                    // 4. Handle File Upload (If user selected a new file)
                    if (fuDocument.HasFile)
                    {
                        try
                        {
                            using (var binaryReader = new System.IO.BinaryReader(fuDocument.PostedFile.InputStream))
                            {
                                byte[] fileData = binaryReader.ReadBytes(fuDocument.PostedFile.ContentLength);

                                var newDoc = new TransferDocument
                                {
                                    Id = Guid.NewGuid(),
                                    TransferId = _transferId,
                                    FileName = fuDocument.FileName,
                                    ContentType = fuDocument.PostedFile.ContentType,
                                    FileData = fileData,
                                    UploadedBy = Auth.Id(),
                                    UploadedDate = DateTime.Now
                                };
                                db.TransferDocuments.Add(newDoc);
                                db.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error uploading file: " + ex.Message);
                            return;
                        }
                    }

                    // 5. Log Approval History
                    string roleCode = Auth.User().CCMSRoleCode;
                    Guid userId = Auth.User().Id;

                    // Determine the step number to return to (usually the step before it was rejected)
                    // Or retrieve the last active step
                    int currentLevelApproval = db.TransferApprovalLog
                            .Where(w => w.BudgetTransferId == _transferId && w.StepNumber != -1)
                            .OrderByDescending(w => w.CreatedDate)
                            .Select(w => w.StepNumber)
                            .FirstOrDefault();

                    var logEntry = new TransferApprovalLog
                    {
                        BudgetTransferId = _transferId,
                        StepNumber = currentLevelApproval,
                        RoleName = roleCode,
                        UserId = userId,
                        ActionType = "Resubmit",
                        ActionDate = DateTime.Now,
                        Status = "Submitted",
                        Remarks = txtResubmit.Text.Trim() // Capture remarks from UI
                    };

                    db.TransferApprovalLog.Add(logEntry);
                    db.SaveChanges();

                    // 6. Send Email Notification
                    // hdncurentRoleApprover.Value was populated in LoadHistory
                    Emails.EmailsTransferBudgetForResubmit(_transferId, model, hdncurentRoleApprover.Value);

                    // 7. Success & Redirect
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Transfer Budget resubmitted successfully.");
                    Response.Redirect("~/Budget/Transfer/Default");
                }
            }
            else
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Invalid Transaction ID.");
            }
        }

        #endregion
    }
}