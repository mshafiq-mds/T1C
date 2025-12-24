using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.T1C.PO.Review
{
    public partial class Review : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["Id"] != null && Guid.TryParse(Request.QueryString["Id"], out Guid guid))
                {
                    hdnFormId.Value = guid.ToString();
                    BindData(guid);
                    BindAuditTrails(guid);

                    // Load all supporting documents
                    LoadAttachment(guid, "Picture", lnkPicture, pnlPictureView, lblPictureDash);
                    LoadAttachment(guid, "MachineRepairHistory", lnkMachineRepairHistory, pnlMachineRepairHistoryView, lblMachineHistoryDash);
                    LoadAttachment(guid, "JobSpecification", lnkJobSpecification, pnlJobSpecificationView, lblJobSpecificationDash);
                    LoadAttachment(guid, "EngineerEstimatePrice", lnkEngineerEstimatePrice, pnlEngineerEstimatePriceView, lblEngineerEstimatePriceDash);
                    LoadAttachment(guid, "DecCostReportCurrentYear", lnkDecCostReportCurrentYear, pnlDecCostReportCurrentYearView, lblDecCostReportCurrentYearDash);
                    LoadAttachment(guid, "DecCostReportLastYear", lnkDecCostReportLastYear, pnlDecCostReportLastYearView, lblDecCostReportLastYearDash);
                    LoadAttachment(guid, "CostReportLastMonth", lnkCostReportLastMonth, pnlCostReportLastMonthView, lblCostReportLastMonthDash);
                    LoadAttachment(guid, "DrawingSketching", lnkDrawingSketching, pnlDrawingSketchingView, lblDrawingSketchingDash);
                    LoadAttachment(guid, "Quotation", lnkQuotation, pnlQuotationView, lblQuotationDash);
                    LoadAttachment(guid, "DamageInvestigationReport", lnkDamageInvestigationReport, pnlDamageInvestigationReportView, lblDamageInvestigationReportDash);
                    LoadAttachment(guid, "VendorRegistrationRecord", lnkVendorRegistrationRecord, pnlVendorRegistrationRecordView, lblVendorRegistrationRecordDash);
                    LoadAttachment(guid, "BudgetTransferAddApproval", lnkBudgetTransferAddApproval, pnlBudgetTransferAddApprovalView, lblBudgetTransferAddApprovalDash);
                    LoadAttachment(guid, "OtherSupportingDocument", lnkOtherSupportingDocument, pnlOtherSupportingDocumentView, lblOtherSupportingDocumentDash);
                }
                else
                {
                    Response.Redirect("~/T1C/PO/Review/Default");
                }
            }
        }

        private void BindData(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var form = db.Forms.Find(id);
                if (form != null)
                {
                    // =================================================================
                    // 1. ORIGINAL BASIC INFO
                    // =================================================================
                    lblTitle.Text = form.Ref;
                    lblBA.Text = form.BizAreaCode + " - " + form.BizAreaName;
                    lblReqName.Text = form.FormRequesterName;
                    lblRefNo.Text = form.Ref;
                    lblDate.Text = form.Date.HasValue ? form.Date.Value.ToString("dd/MM/yyyy") : "-";
                    lblDetails.Text = form.Details;
                    lblJustificationOfNeed.Text = form.JustificationOfNeed;
                    lblAmount.Text = form.Amount.HasValue ? "RM" + form.Amount.Value.ToString("#,##0.00") : "-";

                    // Display Actual Amount (Simple View)
                    lblActualAmountView.Text = form.ActualAmount.HasValue ? "RM" + form.ActualAmount.Value.ToString("#,##0.00") : "-";

                    // =================================================================
                    // 2. PROCUREMENT LOGIC
                    // =================================================================
                    string procurementType = string.Empty;
                    if (form.ProcurementType != null)
                    {
                        if (form.ProcurementType.Equals("quotation_inclusive", StringComparison.OrdinalIgnoreCase)) procurementType = "Inclusive Quotation";
                        else if (form.ProcurementType.Equals("quotation_selective", StringComparison.OrdinalIgnoreCase)) procurementType = "Selective Quotation";
                        else if (form.ProcurementType.Equals("direct_negotiation", StringComparison.OrdinalIgnoreCase))
                        {
                            procurementType = "Direct Negotiation";
                            divJustificationDirectNegotiation.Visible = true;
                            lblJustificationDirectAward.Text = form.Justification;
                        }
                    }
                    lblProcurementType.Text = procurementType;

                    // =================================================================
                    // 3. DATA COST
                    // =================================================================
                    lblCurrentYearActualYTD.Text = form.CurrentYearActualYTD.HasValue ? form.CurrentYearActualYTD.Value.ToString("#,##0.00") : "-";
                    lblCurrentYearBudget.Text = form.CurrentYearBudget.HasValue ? form.CurrentYearBudget.Value.ToString("#,##0.00") : "-";
                    lblPreviousYearActualYTD.Text = form.PreviousYearActualYTD.HasValue ? form.PreviousYearActualYTD.Value.ToString("#,##0.00") : "-";
                    lblPreviousYearActual.Text = form.PreviousYearActual.HasValue ? form.PreviousYearActual.Value.ToString("#,##0.00") : "-";
                    lblPreviousYearBudget.Text = form.PreviousYearBudget.HasValue ? form.PreviousYearBudget.Value.ToString("#,##0.00") : "-";
                    lblA.Text = form.A.HasValue ? form.A.Value.ToString("#,##0.00") : "-";
                    lblB.Text = !string.IsNullOrEmpty(form.B) ? form.B : "-";
                    lblC.Text = form.C.HasValue ? form.C.Value.ToString("#,##0.00") : "-";
                    lblD.Text = form.D.HasValue ? form.D.Value.ToString("#,##0.00") : "-";

                    // =================================================================
                    // 4. STATUS BADGE & BUTTON VISIBILITY
                    // =================================================================
                    if (form.Status != null)
                    {
                        lblStatus.Text = form.Status;
                        switch (form.Status.ToLower())
                        {
                            case "approved": lblStatus.CssClass = "badge badge-success badge-pill"; break;
                            case "pending": lblStatus.CssClass = "badge badge-warning badge-pill"; break;
                            case "rejected": lblStatus.CssClass = "badge badge-danger badge-pill"; break;
                            case "completed": lblStatus.CssClass = "badge badge-dark badge-pill"; break;
                            default: lblStatus.CssClass = "badge badge-secondary badge-pill"; break;
                        }

                        // Only show the "Reviewed" button if status is "Completed"
                        if (form.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                        {
                            btnReviewTrigger.Visible = true;
                        }
                        else
                        {
                            btnReviewTrigger.Visible = false;
                        }
                    }
                    else
                    {
                        btnReviewTrigger.Visible = false;
                    }

                    // =================================================================
                    // 5. ALLOCATION LIST (Original HTML String version)
                    // =================================================================
                    var budgets = db.FormBudgets
                        .Where(fb => fb.FormId == form.Id && fb.Type.ToLower() == "new")
                        .Select(fb => new {
                            fb.Budget.Ref,
                            fb.Budget.Details,
                            fb.Amount
                        }).ToList();

                    if (budgets.Any())
                    {
                        string html = "<ol style='padding-left: 15px; margin-bottom: 0;'>";
                        foreach (var budget in budgets)
                        {
                            string amt = budget.Amount.HasValue ? "RM" + budget.Amount.Value.ToString("#,##0.00") : "-";
                            html += $"<li>{Server.HtmlEncode(budget.Ref)} - {Server.HtmlEncode(budget.Details)} [{amt}]</li>";
                        }
                        html += "</ol>";
                        lblAllocation.Text = html;
                    }
                    else
                    {
                        lblAllocation.Text = "<em>No Budgets Assigned</em>";
                    }

                    // =================================================================
                    // 6. VENDOR LIST
                    // =================================================================
                    var vendorNames = db.FormVendors.Where(fv => fv.FormId == form.Id).Select(fv => fv.Vendor.Name).ToList();
                    if (vendorNames.Any())
                    {
                        string html = "<ol style='padding-left: 15px; margin-bottom: 0;'>";
                        foreach (var name in vendorNames) html += $"<li>{Server.HtmlEncode(name)}</li>";
                        html += "</ol>";
                        lblVendor.Text = html;
                    }
                    else
                    {
                        lblVendor.Text = "<em>No Vendors Assigned</em>";
                    }

                    // =================================================================
                    // 7. PO ATTACHMENT VIEW
                    // =================================================================
                    var attachmentPO = db.Attachments.FirstOrDefault(a => a.ObjectId == form.Id && a.ObjectType == "Form" && a.Type == "PO");
                    if (attachmentPO != null)
                    {
                        pnlPOView.Visible = true;
                        lblPONotUploaded.Visible = false;
                        lnkPO.NavigateUrl = $"~/DownloadAttachment.ashx?id={attachmentPO.Id}";
                        lnkPO.Text = $"<i class='fas fa-file-download mr-1'></i> {attachmentPO.FileName}";
                    }
                    else
                    {
                        pnlPOView.Visible = false;
                        lblPONotUploaded.Visible = true;
                    }

                    // =================================================================
                    // 8. [NEW] FINANCIALS & ALLOCATIONS BREAKDOWN (Repeater)
                    // =================================================================

                    // A. Bind the Labels
                    lblActualAmount.Text = form.ActualAmount.HasValue ? "RM " + form.ActualAmount.Value.ToString("N2") : "Not Set";
                    lblEstimateAmount.Text = form.Amount.HasValue ? "RM " + form.Amount.Value.ToString("N2") : "-";

                    // B. Bind the Repeater
                    // This query fetches budgets and calculates the sum of 'Approved' transactions 
                    // from this Form to that Budget.
                    // 1. Savings Query: Explicitly use the ViewModel
                    var budgetAllocations = db.FormBudgets
                        .Where(fb => fb.FormId == form.Id && fb.Type.ToLower() == "new")
                        .Select(fb => new BudgetAllocationViewModel
                        {
                            BudgetId = fb.BudgetId,
                            Ref = fb.Budget.Ref,
                            Details = fb.Budget.Details,
                            AllocatedAmount = db.Transactions
                                .Where(t =>
                                    t.FromId == fb.FormId &&
                                    t.FromType == "Form" &&
                                    t.ToId == fb.BudgetId &&
                                    t.ToType == "Budget" &&
                                    t.Status == "Approved" &&
                                    t.DeletedBy == null
                                )
                                .Sum(t => (decimal?)t.Amount) ?? 0
                        })
                        .Where(x => x.AllocatedAmount > 0)
                        .ToList();

                    // 2. Overrun Query: Now budgetAllocations is a List<BudgetAllocationViewModel>
                    if (budgetAllocations.Count == 0)
                    {
                        budgetAllocations = (from t in db.Transactions
                                             join b in db.Budgets on t.FromId equals b.Id
                                             where t.ToId == form.Id
                                                && t.Status == "PO Overrun Allocation"
                                                && t.DeletedDate == null
                                             select new BudgetAllocationViewModel
                                             {
                                                 BudgetId = t.FromId,
                                                 Ref = b.Ref,       // Now pulling from the joined Budgets table
                                                 Details = b.Details, // Now pulling from the joined Budgets table
                                                 AllocatedAmount = t.Amount ?? 0
                                             }).ToList();
                    }

                    // Bind to UI
                    rptAllocationView.DataSource = budgetAllocations;
                    rptAllocationView.DataBind();
                }
            }
        }

        public class BudgetAllocationViewModel
        {
            public Guid? BudgetId { get; set; }
            public string Ref { get; set; }
            public string Details { get; set; }
            public decimal AllocatedAmount { get; set; }
        }
        private void LoadAttachment(Guid formId, string type, HyperLink link, Panel viewPanel, Label dashLabel)
        {
            using (var db = new AppDbContext())
            {
                var attachment = db.Attachments.FirstOrDefault(a => a.ObjectId == formId && a.ObjectType == "Form" && a.Type == type);
                if (attachment != null)
                {
                    link.NavigateUrl = $"~/DownloadAttachment.ashx?id={attachment.Id}";
                    link.Text = attachment.FileName;
                    viewPanel.Visible = true;
                    dashLabel.Visible = false;
                }
                else
                {
                    viewPanel.Visible = false;
                    dashLabel.Visible = true;
                }
            }
        }

        protected void gvAuditTrails_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAuditTrails.PageIndex = e.NewPageIndex;
            BindAuditTrails(Guid.Parse(hdnFormId.Value));
        }

        private void BindAuditTrails(Guid formId)
        {
            var auditTrails = new Class.Approval().GetApprovals(formId, "Form");
            gvAuditTrails.DataSource = auditTrails;
            gvAuditTrails.DataBind();
        }

        // --- THE REVIEW ACTION HANDLER ---
        protected void btnReviewed_Click(object sender, EventArgs e)
        {
            string formId = hdnFormId.Value;
            string remark = hdnRemark.Value;

            if (!Guid.TryParse(formId, out Guid parsedFormId))
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Invalid Form ID.");
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    var form = db.Forms.Find(parsedFormId);
                    if (form == null) return;

                    var lastApproval = db.Approvals
                        .Where(a => a.ObjectId == form.Id && a.ObjectType == "Form")
                        .OrderByDescending(a => a.CreatedDate)
                        .FirstOrDefault();

                    int lastOrder = lastApproval?.Order ?? 0;

                    db.Approvals.Add(new Models.Approval
                    {
                        ObjectId = form.Id,
                        ObjectType = "Form",
                        ActionById = Auth.User().Id,
                        ActionByType = "User",
                        ActionByCode = Auth.User().CCMSRoleCode,
                        ActionByName = Auth.User().Name,
                        Action = "Reviewed PO",
                        Remark = remark,
                        Order = lastOrder + 1,
                        CreatedDate = DateTime.Now
                    });

                    // Note: Since this is "Reviewed", we usually don't change the main status from "Completed" 
                    // unless your workflow requires it (e.g. form.Status = "Reviewed").
                    // Assuming this is just an audit log entry.

                    db.SaveChanges();
                }

                // 1. Store the message in Session
                Session["SweetAlertMessage"] = "PO Reviewed Successfully.";
                Session["SweetAlertType"] = "success"; // Optional: store type if dynamic

                // 2. Perform the redirect (using the safe method you just fixed)
                Response.Redirect("~/T1C/PO/Review/Default", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error: " + ex.Message);
            }
        }
    }
}