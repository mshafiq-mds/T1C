using FGV.Prodata.Web.UI;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.Ocsp;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.Additional
{
    public partial class Resubmit : ProdataPage
    {
        private Guid _transferId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string idUserStr = Request.QueryString["userId"];

                // Redirect only if userId exists and does not match the current user
                if (!string.IsNullOrEmpty(idUserStr) && idUserStr != Auth.User().Id.ToString())
                {
                    Response.Redirect("~/Budget/Additional");
                }

                if (Guid.TryParse(Request.QueryString["Id"], out _transferId))
                {
                    LoadTransfer(_transferId);
                    LoadDocument(_transferId);
                    Loadhistory(_transferId);
                }
                else
                {
                    Response.Redirect("~/Budget/Additional");
                }
            }
        }

        private void LoadTransfer(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var Additions = db.AdditionalBudgetRequests.FirstOrDefault(x => x.Id == id);
                if (Additions == null)
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Additional not found.");
                    Response.Redirect("~/Budget/Additional");
                    return;
                }

                LblBA.Text = Additions.BA ?? "-";
                LblBAName.Text = new Class.IPMSBizArea().GetNameByCode(Additions.BA ?? "") ?? "-";

                lblBudgetType.Text = Additions.BudgetType ?? "-";
                lblProject.Text = Additions.Project ?? "-";
                lblRefNo.Text = Additions.RefNo ?? "-";
                lblDate.Text = Additions.ApplicationDate.ToString("yyyy-MM-dd");

                lblBudgetEstimate.Text = Additions.EstimatedCost.ToString("N2");

                lblEVisa.Text = string.IsNullOrWhiteSpace(Additions.EVisaNo) ? "-" : Additions.EVisaNo;

                lblRequestDetails.Text = string.IsNullOrWhiteSpace(Additions.RequestDetails) ? "-" : Additions.RequestDetails;
                lblReason.Text = string.IsNullOrWhiteSpace(Additions.Reason) ? "-" : Additions.Reason;

                lblCostCentre.Text = string.IsNullOrWhiteSpace(Additions.CostCentre) ? "-" : Additions.CostCentre;
                Guid? TOGuid = Additions.ToBudgetType;
                var ToBudgetType = db.BudgetTypes
                    .Where(x => x.Id == TOGuid)
                    .Select(x => x.Name)
                    .FirstOrDefault();
                lblTBT.Text = ToBudgetType;

                lblApprovedBudget.Text = Additions.ApprovedBudget.HasValue ? Additions.ApprovedBudget.Value.ToString("N2") : "-";
                lblNewTotalBudget.Text = Additions.NewTotalBudget.HasValue ? Additions.NewTotalBudget.Value.ToString("N2") : "-";
                lblAdditionalBudget.Text = Additions.AdditionalBudget.HasValue ? Additions.AdditionalBudget.Value.ToString("N2") : "-";
                lblCheckType.Text = Additions.CheckType;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Guid.TryParse(Request.QueryString["Id"], out _transferId))
            {
                using (var db = new AppDbContext())
                {
                    var model = db.AdditionalBudgetRequests.FirstOrDefault(x => x.Id == _transferId);
                    if (model == null)
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Additions budget not found.");
                        return;
                    }

                    model.Status = 2;

                    model.UpdatedBy = Auth.User().Id; // Or your method to get current user
                    model.UpdatedDate = DateTime.Now;

                    db.SaveChanges();

                    if (fuDocument.HasFile)
                    {
                        using (var binaryReader = new System.IO.BinaryReader(fuDocument.PostedFile.InputStream))
                        {
                            byte[] fileData = binaryReader.ReadBytes(fuDocument.PostedFile.ContentLength);

                            // Check if a document already exists
                            // Always insert a new document
                            var newDoc = new AdditionalBudgetDocuments
                            {
                                Id = Guid.NewGuid(),
                                TransferId = _transferId,
                                FileName = fuDocument.FileName,
                                ContentType = fuDocument.PostedFile.ContentType,
                                FileData = fileData,
                                UploadedBy = Auth.Id(),
                                UploadedDate = DateTime.Now
                            };
                            db.AdditionalBudgetDocuments.Add(newDoc);

                            db.SaveChanges();
                        }
                    }

                    string roleCode = Auth.User().CCMSRoleCode;
                    Guid userId = Auth.User().Id;

                    int currentLevelApproval = db.AdditionalBudgetLog
                            .Where(w => w.BudgetTransferId == _transferId && w.StepNumber != -1)
                            .OrderByDescending(w => w.CreatedDate)
                            .Select(w => w.StepNumber)
                            .FirstOrDefault();

                    var logEntry = new AdditionalBudgetLog
                    {
                        BudgetTransferId = _transferId,
                        StepNumber = currentLevelApproval,
                        RoleName = roleCode,
                        UserId = userId,
                        ActionType = "Resubmit",
                        ActionDate = DateTime.Now,
                        Status = "Submitted",
                        Remarks = txtResubmit.Text?.Trim()
                    };

                    db.AdditionalBudgetLog.Add(logEntry);
                    db.SaveChanges();

                    Emails.EmailsAdditionalBudgetForResubmit(_transferId, model, hdncurentRoleApprover.Value);
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Additional Budget updated.");
                    Response.Redirect("~/Budget/Additional");
                }
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

                        // View link
                        var link = new HyperLink
                        {
                            NavigateUrl = "~/DocumentHandler.ashx?id=" + doc.Id + "&module=AdditionalBudgetDocuments",
                            CssClass = "btn btn-outline-success mr-2",
                            Target = "_blank",
                            Text = "<i class='fas fa-external-link-alt'></i> View"
                        };

                        // File name
                        var fileLabel = new Label
                        {
                            Text = $"<i class='fas fa-file-alt text-success mr-2'></i> {doc.FileName}",
                            CssClass = "text-dark font-weight-bold",
                            EnableViewState = false
                        };

                        // Add View then FileName (both on left)
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

                var Additions = query
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

                // Set first/top RoleName to curentRoleApprover
                hdncurentRoleApprover.Value = Additions.Any() ? Additions.First().RoleName : null;

                if (Additions.Any())
                {
                    pnHistoryApproval.Visible = true;
                }
                gvHistory.DataSource = Additions;
                gvHistory.DataBind();
            }
        }
    }
}