using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.T1C.PoolBudget
{
    public partial class View : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["Id"] != null)
                {
                    string id = Request.QueryString["Id"].ToString();
                    Guid guid;
                    if (Guid.TryParse(id, out guid))
                    {
                        hdnFormId.Value = id;
                        BindData(guid);
                        LoadDocument(guid);
                        //BindAuditTrails(guid);

                        //LoadAttachment(guid, "Picture", lnkPicture, pnlPictureView, lblPictureDash);
                        //LoadAttachment(guid, "MachineRepairHistory", lnkMachineRepairHistory, pnlMachineRepairHistoryView, lblMachineHistoryDash);
                        //LoadAttachment(guid, "JobSpecification", lnkJobSpecification, pnlJobSpecificationView, lblJobSpecificationDash);
                        //LoadAttachment(guid, "EngineerEstimatePrice", lnkEngineerEstimatePrice, pnlEngineerEstimatePriceView, lblEngineerEstimatePriceDash);
                        //LoadAttachment(guid, "DecCostReportCurrentYear", lnkDecCostReportCurrentYear, pnlDecCostReportCurrentYearView, lblDecCostReportCurrentYearDash);
                        //LoadAttachment(guid, "DecCostReportLastYear", lnkDecCostReportLastYear, pnlDecCostReportLastYearView, lblDecCostReportLastYearDash);
                        //LoadAttachment(guid, "CostReportLastMonth", lnkCostReportLastMonth, pnlCostReportLastMonthView, lblCostReportLastMonthDash);
                        //LoadAttachment(guid, "DrawingSketching", lnkDrawingSketching, pnlDrawingSketchingView, lblDrawingSketchingDash);
                        //LoadAttachment(guid, "Quotation", lnkQuotation, pnlQuotationView, lblQuotationDash);
                        //LoadAttachment(guid, "DamageInvestigationReport", lnkDamageInvestigationReport, pnlDamageInvestigationReportView, lblDamageInvestigationReportDash);
                        //LoadAttachment(guid, "VendorRegistrationRecord", lnkVendorRegistrationRecord, pnlVendorRegistrationRecordView, lblVendorRegistrationRecordDash);
                        //LoadAttachment(guid, "BudgetTransferAddApproval", lnkBudgetTransferAddApproval, pnlBudgetTransferAddApprovalView, lblBudgetTransferAddApprovalDash);
                        //LoadAttachment(guid, "OtherSupportingDocument", lnkOtherSupportingDocument, pnlOtherSupportingDocumentView, lblOtherSupportingDocumentDash);
                    }
                }
                else
                {
                    Response.Redirect("~/T1C/PoolBudget");
                }
            }
        }
        private void LoadDocument(Guid transferId)
        {
            using (var db = new AppDbContext())
            {
                var documents = db.FormsProcurementDocuments
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
                            NavigateUrl = "~/DocumentHandler.ashx?id=" + doc.Id + "&module=FormsProcurementDocuments",
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
        private void BindData(Guid formId)
        {
            using (var db = new Models.AppDbContext())
            {
                var form = db.FormsProcurement
                    .FirstOrDefault(x => x.Id == formId);

                if (form == null)
                    return;

                // Left column
                lblBA.Text = form.BizAreaName ?? string.Empty;
                lblDetails.Text = form.Details ?? string.Empty;
                lblJustificationOfNeed.Text = form.JustificationOfNeed ?? string.Empty;
                lblRemarks.Text = form.Remarks ?? string.Empty;

                // Right column
                lblRefNo.Text = form.Ref ?? string.Empty;
                lblDate.Text = form.CreatedDate.ToString("dd/MM/yyyy");
                lblAmount.Text = form.Amount?.ToString("N2") ?? "0.00";
                //lblActualAmount.Text = form.ActualAmount?.ToString("N2") ?? "0.00";
                lblProcurementType.Text = db.BudgetTypes.Where(x => x.Id == form.TypeId).Select(x => x.Name).FirstOrDefault();

                // Status badge
                lblStatus.Text = form.Status;
                lblStatus.CssClass = "badge badge-pill " + GetStatusCss(form.Status);
                 
            }
        }

        private string GetStatusCss(string status)
        {
            switch (status?.ToLower())
            {
                case "approved": return "badge-success";
                case "rejected": return "badge-danger";
                case "pending": return "badge-warning";
                default: return "badge-secondary";
            }
        }

    }
}