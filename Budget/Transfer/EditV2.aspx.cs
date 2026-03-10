using FGV.Prodata.Web.UI;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.Transfer
{
    public partial class EditV2 : ProdataPage
    {
        private Guid _transferId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Parse ID and Load Data
                if (Guid.TryParse(Request.QueryString["Id"], out _transferId))
                {
                    LoadTransferMasterAndDetails(_transferId);
                    LoadDocument(_transferId);
                    LoadHistory(_transferId);
                }
                else
                {
                    Response.Redirect("~/Budget/Transfer/Default");
                }
            }
        }

        #region Loading Data Methods

        private void LoadTransferMasterAndDetails(Guid id)
        {
            using (var db = new AppDbContext())
            {
                // 1. Load Master Record
                var transfer = db.TransfersTransaction.FirstOrDefault(x => x.Id == id);
                if (transfer == null)
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Transfer record not found.");
                    Response.Redirect("~/Budget/Transfer/Default");
                    return;
                }

                // 2. Bind Header Info
                txtRefNo.Text = transfer.RefNo;
                txtProject.Text = transfer.Project;
                txtDate.Text = transfer.Date.ToString("yyyy-MM-dd");
                txtEstimatedCost.Text = transfer.EstimatedCost.ToString("F2");
                txtEVisa.Text = transfer.EVisaNo;
                txtWorkDetails.Text = transfer.WorkDetails;
                txtJustification.Text = transfer.Justification;

                rdoOpex.Checked = transfer.BudgetType == "OPEX";
                rdoCapex.Checked = transfer.BudgetType == "CAPEX";

                // 3. Bind Global BAs
                var ipmsBA = new Class.IPMSBizArea();
                string fromBaName = ipmsBA.GetNameByCode(transfer.FromBA);
                string toBaName = ipmsBA.GetNameByCode(transfer.ToBA);

                lblGlobalFromBA.Text = $"{transfer.FromBA} - {fromBaName}";
                lblGlobalToBA.Text = $"{transfer.ToBA} - {toBaName}";

                // 4. Bind the Multiple 'From' Details to the Repeater
                var detailsList = db.TransfersTransactionDetails
                                    .Where(x => x.TransferId == id)
                                    .ToList();

                rptFromBudgets.DataSource = detailsList;
                rptFromBudgets.DataBind();

                // 5. Bind 'To' Budget Info
                var ToBudgetTypeName = db.BudgetTypes
                    .Where(x => x.Id == transfer.ToBudgetType)
                    .Select(x => x.Name)
                    .FirstOrDefault();

                txtToBudgetType.Text = ToBudgetTypeName ?? "Unknown";
                txtToGL.Text = transfer.ToGL;
                txtToBudget.Text = (transfer.ToBudget ?? 0).ToString("F2");
                txtToBalance.Text = (transfer.ToBalance ?? 0).ToString("F2");
                txtToTransfer.Text = (transfer.ToTransfer ?? 0).ToString("F2");
                txtToAfter.Text = (transfer.ToAfter ?? 0).ToString("F2");
            }
        }

        // Helper method for the Repeater to resolve Budget Type ID to Name
        protected string GetBudgetTypeName(object budgetTypeIdObj)
        {
            if (budgetTypeIdObj == null) return "Unknown";

            if (Guid.TryParse(budgetTypeIdObj.ToString(), out Guid typeId))
            {
                using (var db = new AppDbContext())
                {
                    return db.BudgetTypes.Where(x => x.Id == typeId).Select(x => x.Name).FirstOrDefault() ?? "Unknown";
                }
            }
            return "Unknown";
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
                            CssClass = "btn btn-sm btn-outline-success mr-2",
                            Target = "_blank",
                            Text = "<i class='fas fa-external-link-alt'></i> View"
                        };

                        var fileLabel = new Label
                        {
                            Text = $"<i class='fas fa-file-alt text-secondary mr-2'></i> {doc.FileName}",
                            CssClass = "text-dark font-weight-bold",
                            EnableViewState = false
                        };

                        panel.Controls.Add(link);
                        panel.Controls.Add(fileLabel);
                        phDocumentList.Controls.Add(panel);
                    }
                }
            }
        }

        private void LoadHistory(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var transfers = db.TransferApprovalLog
                    .Where(x => x.DeletedDate == null && x.BudgetTransferId == id)
                    .OrderByDescending(x => x.ActionDate)
                    .Select(x => new
                    {
                        x.Id,
                        x.ActionDate,
                        x.ActionType,
                        x.RoleName,
                        x.Status,
                        x.Remarks
                    }).ToList();

                // Store the most recent RoleName to know who sent it back (for email purposes)
                hdncurentRoleApprover.Value = transfers.Any() ? transfers.First().RoleName : "";

                if (transfers.Any())
                {
                    pnHistoryApproval.Visible = true;
                    gvHistory.DataSource = transfers;
                    gvHistory.DataBind();
                }
            }
        }

        #endregion

        #region Submission Logic

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
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

                    // 1. Update Transaction Status back to workflow
                    model.status = "Submitted";
                    model.UpdatedBy = Auth.User().Id;
                    model.UpdatedDate = DateTime.Now;

                    db.SaveChanges();

                    // 2. Handle Additional File Upload 
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

                    // 3. Log Resubmit Action
                    string roleCode = Auth.User().CCMSRoleCode;
                    Guid userId = Auth.User().Id;

                    // Fetch the last active step before rejection
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
                        Remarks = txtResubmit.Text.Trim() // Mandatory remarks captured
                    };

                    db.TransferApprovalLog.Add(logEntry);
                    db.SaveChanges();

                    // 4. Send Notification
                    Emails.EmailsTransferBudgetForResubmit(_transferId, model, hdncurentRoleApprover.Value);

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