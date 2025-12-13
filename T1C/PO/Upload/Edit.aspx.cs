using CustomGuid.AspNet.Identity;
using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using Prodata.WebForm.Helpers;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.T1C.PO.Upload
{
    public partial class UploadPO : ProdataPage
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
                        BindData(id);
                        BindAuditTrails(guid);

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
                }
                else
                {
                    Response.Redirect("~/T1C/PO/Upload");
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                bool isSuccess = false;
                string formId = hdnFormId.Value;
                Guid parsedFormId = Guid.Parse(formId);

                // Actual amount validation
                if (string.IsNullOrWhiteSpace(txtActualAmount.Text))
                {
                    lblActualAmountError.Text = "Actual amount is required.";
                    lblActualAmountError.CssClass = "text-danger";
                    lblActualAmountError.CssClass = lblActualAmountError.CssClass.Replace("d-none", "").Trim();
                    return;
                }

                if (!decimal.TryParse(txtActualAmount.Text.Replace(",", ""), out decimal actualAmount))
                {
                    lblActualAmountError.Text = "Invalid actual amount.";
                    lblActualAmountError.CssClass = "text-danger";
                    lblActualAmountError.CssClass = lblActualAmountError.CssClass.Replace("d-none", "").Trim();
                    return;
                }

                using (var db = new AppDbContext())
                {
                    using (var trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var form = db.Forms.Find(parsedFormId);
                            if (form != null)
                            {
                                decimal estimateAmount = form.Amount ?? 0;
                                decimal availableBalance = estimateAmount - actualAmount;
                                bool isAllocationSectionVisible = (actualAmount < estimateAmount);

                                decimal totalAllocated = 0;
                                var allocations = new List<Transaction>();

                                foreach (RepeaterItem item in rptBudgetAllocations.Items)
                                {
                                    var hdnBudgetId = (HiddenField)item.FindControl("hdnBudgetId");
                                    var txtAllocateAmount = (TextBox)item.FindControl("txtAllocateAmount");

                                    if (Guid.TryParse(hdnBudgetId.Value, out Guid budgetId) &&
                                        decimal.TryParse(txtAllocateAmount.Text, out decimal allocateAmount) &&
                                        allocateAmount > 0)
                                    {
                                        // Find all old matching transactions (not just one)
                                        var oldAllocations = db.Transactions
                                            .Where(t => t.FromId == form.Id &&
                                                        t.FromType == "Form" &&
                                                        t.ToId == budgetId &&
                                                        t.ToType == "Budget")
                                            .ToList();

                                        // Soft delete them all
                                        foreach (var old in oldAllocations)
                                            db.SoftDelete(old);

                                        totalAllocated += allocateAmount;

                                        if (isAllocationSectionVisible)
                                        {
                                            allocations.Add(new Transaction
                                            {
                                                FromId = form.Id,
                                                FromType = "Form",
                                                ToId = budgetId,
                                                ToType = "Budget",
                                                Date = DateTime.Now,
                                                Amount = allocateAmount,
                                                Status = "Approved"
                                            });
                                        }
                                    }
                                }

                                if ((Math.Abs(totalAllocated - availableBalance) > 0.009m) && isAllocationSectionVisible)
                                {
                                    lblAllocationError.Text = "Total allocated does not match available balance.";
                                    lblAllocationError.CssClass = "text-danger";
                                    lblAllocationError.Visible = true;
                                    return;
                                }

                                // Save actual amount to form
                                form.ActualAmount = actualAmount;
                                form.Status = "Completed";
                                db.Entry(form).State = EntityState.Modified;

                                // Save file upload (if any)
                                if (fuPO.HasFile)
                                {
                                    // Delete existing PO attachment for this form
                                    var existingAttachment = db.Attachments
                                        .FirstOrDefault(a => a.ObjectId == form.Id && a.ObjectType == "Form" && a.Type == "PO");

                                    if (existingAttachment != null)
                                    {
                                        db.Attachments.Remove(existingAttachment);
                                    }

                                    var file = fuPO.PostedFile;
                                    using (var binaryReader = new BinaryReader(file.InputStream))
                                    {
                                        var attachment = new Models.Attachment
                                        {
                                            ObjectId = form.Id,
                                            ObjectType = "Form",
                                            Type = "PO",
                                            Name = Path.GetFileNameWithoutExtension(file.FileName),
                                            FileName = Path.GetFileName(file.FileName),
                                            ContentType = file.ContentType,
                                            Content = binaryReader.ReadBytes(file.ContentLength),
                                            Ext = Path.GetExtension(file.FileName),
                                            Size = file.ContentLength,
                                        };
                                        db.Attachments.Add(attachment);
                                    }
                                }

                                // Save allocations
                                foreach (var transaction in allocations)
                                {
                                    db.Transactions.Add(transaction);
                                }

                                db.SaveChanges();
                                trans.Commit();
                                isSuccess = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
                        }
                    }
                }

                if (isSuccess)
                {
                    string script = @"
                        Swal.fire({
                            icon: 'success',
                            title: 'Form updated successfully.',
                            showConfirmButton: true
                        }).then(function() {
                            window.location.href = '" + Request.Url.GetLeftPart(UriPartial.Path) + @"';
                        });";

                    ScriptManager.RegisterStartupScript(this, GetType(), "alertRedirect", script, true);
                }
                else
                {
                    string script = @"
                        Swal.fire({d
                            icon: 'error',
                            title: 'Failed to update form. Please try again.',
                            showConfirmButton: true
                        }).then(function() {
                            window.location.href = '" + Request.Url.GetLeftPart(UriPartial.Path) + @"';
                        });";

                    ScriptManager.RegisterStartupScript(this, GetType(), "alertRedirect", script, true);
                }

                //if (isSuccess)
                //{
                //    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Form updated successfully.");
                //}
                //else
                //{
                //    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Failed to update form. Please try again.");
                //}

                // Reload page
                //Response.Redirect(Request.Url.GetCurrentUrl(true));
                //Response.Redirect(Request.Url.GetLeftPart(UriPartial.Path));

            }
        }

        [WebMethod]
        public static List<Models.ViewModels.BudgetListViewModel> GetBudgets()
        {
            return new Class.Budget().GetBudgets(year: DateTime.Now.Year, bizAreaCode: Auth.User().iPMSBizAreaCode);
        }

        [WebMethod]
        public static List<Models.ViewModels.FormBudgetListViewModel> GetSelectedBudgetIds(Guid formId)
        {
            using (var db = new AppDbContext())
            {
                return db.FormBudgets
                    .Where(fb => fb.FormId == formId && fb.Type.ToLower() == "additional")
                    .Select(fb => new
                    {
                        fb.BudgetId,
                        fb.Amount
                    })
                    .AsEnumerable() // switch to LINQ to Objects
                    .Select(fb => new Models.ViewModels.FormBudgetListViewModel
                    {
                        BudgetId = fb.BudgetId,
                        Amount = fb.Amount.HasValue
                            ? fb.Amount.Value.ToString("#,##0.00")
                            : string.Empty
                    })
                    .ToList();
            }
        }

        private void BindData(string id = null)
        {
            if (!string.IsNullOrEmpty(id))
            {
                using (var db = new AppDbContext())
                {
                    var form = db.Forms.Find(Guid.Parse(id));
                    if (form != null)
                    {
                        lblTitle.Text = form.Ref;

                        lblBA.Text = form.BizAreaCode + " - " + form.BizAreaName;
                        lblReqName.Text = form.FormRequesterName;
                        lblRefNo.Text = form.Ref;
                        lblDate.Text = form.Date.HasValue ? form.Date.Value.ToString("dd/MM/yyyy") : "-";
                        lblDetails.Text = form.Details;
                        lblJustificationOfNeed.Text = form.JustificationOfNeed;
                        lblAmount.Text = form.Amount.HasValue ? "RM" + form.Amount.Value.ToString("#,##0.00") : "-";
                        lblAmount2.Text = form.Amount.HasValue ? "RM" + form.Amount.Value.ToString("#,##0.00") : "-";

                        #region Allocation
                        var budgets = db.FormBudgets
                            .Where(fb => fb.FormId == form.Id && fb.Type.ToLower() == "new")
                            .Select(fb => new
                            {
                                fb.BudgetId,
                                fb.Budget.Ref,
                                fb.Budget.Details,
                                fb.Amount,
                                AllocatedAmount = db.Transactions
                                    .Where(t =>
                                        t.FromId == fb.FormId &&
                                        t.FromType == "Form" &&
                                        t.ToId == fb.BudgetId &&
                                        t.ToType == "Budget" &&
                                        t.Status == "Approved"
                                    )
                                    .Sum(t => (decimal?)t.Amount) ?? 0
                            })
                            .ToList();

                        if (budgets.Any())
                        {
                            string html = "<ol style='padding-left: 15px; margin-bottom: 0;'>";  // 👈 added style here

                            foreach (var budget in budgets)
                            {
                                string amount = budget.Amount.HasValue ? "RM" + budget.Amount.Value.ToString("#,##0.00") : "-";
                                string line = $"{budget.Ref} - {budget.Details} [{amount}]";
                                html += $"<li>{Server.HtmlEncode(line)}</li>";
                            }

                            html += "</ol>";

                            lblAllocation.Text = html;
                        }
                        else
                        {
                            lblAllocation.Text = "<em>No Budgets Assigned</em>";
                        }
                        #endregion

                        #region Vendor
                        var vendorNames = db.FormVendors
                            .Where(fv => fv.FormId == form.Id)
                            .Select(fv => fv.Vendor.Name)
                            .ToList();

                        if (vendorNames.Any())
                        {
                            string html = "<ol style='padding-left: 15px; margin-bottom: 0;'>";
                            foreach (var name in vendorNames)
                            {
                                html += $"<li>{Server.HtmlEncode(name)}</li>";
                            }
                            html += "</ol>";
                            lblVendor.Text = html;
                        }
                        else
                        {
                            lblVendor.Text = "<em>No Vendors Assigned</em>";
                        }
                        #endregion

                        divJustificationDirectNegotiation.Visible = false;
                        string procurementType = string.Empty;
                        if (form.ProcurementType.Equals("quotation_inclusive", StringComparison.OrdinalIgnoreCase))
                        {
                            procurementType = "Inclusive Quotation";
                        }
                        else if (form.ProcurementType.Equals("quotation_selective", StringComparison.OrdinalIgnoreCase))
                        {
                            procurementType = "Selective Quotation";
                        }
                        else if (form.ProcurementType.Equals("direct_negotiation", StringComparison.OrdinalIgnoreCase))
                        {
                            procurementType = "Direct Negotiation";
                            divJustificationDirectNegotiation.Visible = true;
                        }

                        lblProcurementType.Text = procurementType;
                        lblJustificationDirectAward.Text = form.Justification;

                        lblCurrentYearActualYTD.Text = form.CurrentYearActualYTD.HasValue ? form.CurrentYearActualYTD.Value.ToString("#,##0.00") : "-";
                        lblCurrentYearBudget.Text = form.CurrentYearBudget.HasValue ? form.CurrentYearBudget.Value.ToString("#,##0.00") : "-";
                        lblPreviousYearActualYTD.Text = form.PreviousYearActualYTD.HasValue ? form.PreviousYearActualYTD.Value.ToString("#,##0.00") : "-";
                        lblPreviousYearActual.Text = form.PreviousYearActual.HasValue ? form.PreviousYearActual.Value.ToString("#,##0.00") : "-";
                        lblPreviousYearBudget.Text = form.PreviousYearBudget.HasValue ? form.PreviousYearBudget.Value.ToString("#,##0.00") : "-";
                        lblA.Text = form.A.HasValue ? form.A.Value.ToString("#,##0.00") : "-";
                        lblB.Text = !string.IsNullOrEmpty(form.B) ? form.B : "-";
                        lblC.Text = form.C.HasValue ? form.C.Value.ToString("#,##0.00") : "-";
                        lblD.Text = form.D.HasValue ? form.D.Value.ToString("#,##0.00") : "-";

                        if (form.Status != null)
                        {
                            switch (form.Status.ToLower())
                            {
                                case "approved":
                                    lblStatus.Text = "Approved";
                                    lblStatus.CssClass = "badge badge-success badge-pill";
                                    break;
                                case "pending":
                                    lblStatus.Text = "Pending";
                                    lblStatus.CssClass = "badge badge-warning badge-pill";
                                    break;
                                case "rejected":
                                    lblStatus.Text = "Rejected";
                                    lblStatus.CssClass = "badge badge-danger badge-pill";
                                    break;
                                case "draft":
                                    lblStatus.Text = "Draft";
                                    lblStatus.CssClass = "badge badge-info badge-pill";
                                    break;
                                case "sentback":
                                    lblStatus.Text = "Sent Back";
                                    lblStatus.CssClass = "badge badge-secondary badge-pill";
                                    break;
                                default:
                                    lblStatus.Text = form.Status;
                                    lblStatus.CssClass = "badge badge-secondary badge-pill";
                                    break;
                            }
                        }

                        #region Allocate budget
                        rptBudgetAllocations.DataSource = budgets;
                        rptBudgetAllocations.DataBind();
                        #endregion

                        txtActualAmount.Text = form.ActualAmount.HasValue ? form.ActualAmount.Value.ToString("#,##0.00") : string.Empty;

                        #region PO
                        var attachmentPO = db.Attachments
                            .FirstOrDefault(a => a.ObjectId == form.Id && a.ObjectType == "Form" && a.Type == "PO");

                        if (attachmentPO != null)
                        {
                            pnlPOView.Visible = true;
                            lnkPO.NavigateUrl = $"~/DownloadAttachment.ashx?id={attachmentPO.Id}";
                            lnkPO.Text = attachmentPO.FileName;
                        }
                        else
                        {
                            pnlPOView.Visible = false;
                        }
                        #endregion
                    }
                }
            }
        }

        #region Documents
        private void LoadAttachment(Guid formId, string type, HyperLink link, Panel viewPanel, Label dashLabel)
        {
            using (var db = new AppDbContext())
            {
                var attachment = db.Attachments.FirstOrDefault(a => a.ObjectId == formId && a.ObjectType.Equals("Form", StringComparison.OrdinalIgnoreCase) && a.Type.Equals(type, StringComparison.OrdinalIgnoreCase));

                if (attachment != null)
                {
                    link.NavigateUrl = $"~/DownloadAttachment.ashx?id={attachment.Id}";
                    link.Text = attachment.FileName;
                    link.Visible = true;
                    viewPanel.Visible = true;
                    dashLabel.Visible = false; // Hide the dash label if the link is visible
                }
                else
                {
                    link.Visible = false;
                    viewPanel.Visible = false;
                    dashLabel.Visible = true; // Show the dash label if no attachment exists
                }
            }
        }
        #endregion

        #region Audit Trails
        protected void gvAuditTrails_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["pageIndex"] = e.NewPageIndex.ToString();
            BindAuditTrails(Guid.Parse(hdnFormId.Value));
        }

        private void BindAuditTrails(Guid formId)
        {
            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            // 1. Get the list of approvals
            var auditTrails = new Class.Approval().GetApprovals(formId, "Form");

            // 2. CHECK LATEST ACTION
            if (auditTrails != null && auditTrails.Count > 0)
            {
                // Get the most recent approval record
                var latestTrail = auditTrails.LastOrDefault();

                // Check if the latest action is "Reviewed PO"
                if (latestTrail != null &&
                    latestTrail.Action.Equals("Reviewed PO", StringComparison.OrdinalIgnoreCase))
                {
                    btnSave.Visible = false;
                }
                else
                {
                    // Ensure button is visible otherwise (in case it was hidden previously)
                    btnSave.Visible = true;
                }
            }

            // 3. Bind to GridView
            gvAuditTrails.DataSource = auditTrails;
            gvAuditTrails.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
            gvAuditTrails.DataBind();
        }
        #endregion
    }
}

//using CustomGuid.AspNet.Identity;
//using FGV.Prodata.App;
//using FGV.Prodata.Web.UI;
//using Prodata.WebForm.Helpers;
//using Prodata.WebForm.Models;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Net.Mail;
//using System.Web;
//using System.Web.Services;
//using System.Web.UI;
//using System.Web.UI.WebControls;

//namespace Prodata.WebForm.T1C.PO.Upload
//{
//    public partial class UploadPO : ProdataPage
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                if (Request.QueryString["Id"] != null)
//                {
//                    string id = Request.QueryString["Id"].ToString();
//                    Guid guid;
//                    if (Guid.TryParse(id, out guid))
//                    {
//                        hdnFormId.Value = id;
//                        BindData(id);
//                        BindAuditTrails(guid);

//                        // LOGIC: Check for "Reviewed PO"
//                        var auditHistory = new Class.Approval().GetApprovals(guid, "Form");
//                        if (auditHistory != null)
//                        {
//                            var latestAction = auditHistory.OrderByDescending(x => x.Datetime).FirstOrDefault();
//                            if (latestAction != null && latestAction.Action.Equals("Reviewed PO", StringComparison.OrdinalIgnoreCase))
//                            {
//                                // Disable File Upload Input specifically
//                                fuPO.Enabled = false;

//                                // Disable the container panels
//                                pnlFileUpload.Enabled = false;
//                                pnlFormFields.Enabled = false;

//                                // Hide Save Button
//                                btnSave.Visible = false;

//                                // Hide JS-based allocation buttons
//                                ScriptManager.RegisterStartupScript(this, GetType(), "disableAllocButtons", "$('#btnAddAllocation, .btnRemoveAllocation').hide();", true);
//                            }
//                        }

//                        LoadAttachment(guid, "Picture", lnkPicture, pnlPictureView, lblPictureDash);
//                        LoadAttachment(guid, "MachineRepairHistory", lnkMachineRepairHistory, pnlMachineRepairHistoryView, lblMachineHistoryDash);
//                        LoadAttachment(guid, "JobSpecification", lnkJobSpecification, pnlJobSpecificationView, lblJobSpecificationDash);
//                        LoadAttachment(guid, "EngineerEstimatePrice", lnkEngineerEstimatePrice, pnlEngineerEstimatePriceView, lblEngineerEstimatePriceDash);
//                        LoadAttachment(guid, "DecCostReportCurrentYear", lnkDecCostReportCurrentYear, pnlDecCostReportCurrentYearView, lblDecCostReportCurrentYearDash);
//                        LoadAttachment(guid, "DecCostReportLastYear", lnkDecCostReportLastYear, pnlDecCostReportLastYearView, lblDecCostReportLastYearDash);
//                        LoadAttachment(guid, "CostReportLastMonth", lnkCostReportLastMonth, pnlCostReportLastMonthView, lblCostReportLastMonthDash);
//                        LoadAttachment(guid, "DrawingSketching", lnkDrawingSketching, pnlDrawingSketchingView, lblDrawingSketchingDash);
//                        LoadAttachment(guid, "Quotation", lnkQuotation, pnlQuotationView, lblQuotationDash);
//                        LoadAttachment(guid, "DamageInvestigationReport", lnkDamageInvestigationReport, pnlDamageInvestigationReportView, lblDamageInvestigationReportDash);
//                        LoadAttachment(guid, "VendorRegistrationRecord", lnkVendorRegistrationRecord, pnlVendorRegistrationRecordView, lblVendorRegistrationRecordDash);
//                        LoadAttachment(guid, "BudgetTransferAddApproval", lnkBudgetTransferAddApproval, pnlBudgetTransferAddApprovalView, lblBudgetTransferAddApprovalDash);
//                        LoadAttachment(guid, "OtherSupportingDocument", lnkOtherSupportingDocument, pnlOtherSupportingDocumentView, lblOtherSupportingDocumentDash);
//                    }
//                }
//                else
//                {
//                    Response.Redirect("~/T1C/PO/Upload");
//                }
//            }
//        }

//        protected void btnSave_Click(object sender, EventArgs e)
//        {
//            if (IsValid)
//            {
//                bool isSuccess = false;
//                string formId = hdnFormId.Value;
//                Guid parsedFormId = Guid.Parse(formId);

//                // Actual amount validation
//                if (string.IsNullOrWhiteSpace(txtActualAmount.Text))
//                {
//                    lblActualAmountError.Text = "Actual amount is required.";
//                    lblActualAmountError.CssClass = "text-danger";
//                    lblActualAmountError.CssClass = lblActualAmountError.CssClass.Replace("d-none", "").Trim();
//                    return;
//                }

//                if (!decimal.TryParse(txtActualAmount.Text.Replace(",", ""), out decimal actualAmount))
//                {
//                    lblActualAmountError.Text = "Invalid actual amount.";
//                    lblActualAmountError.CssClass = "text-danger";
//                    lblActualAmountError.CssClass = lblActualAmountError.CssClass.Replace("d-none", "").Trim();
//                    return;
//                }

//                using (var db = new AppDbContext())
//                {
//                    using (var trans = db.Database.BeginTransaction())
//                    {
//                        try
//                        {
//                            var form = db.Forms.Find(parsedFormId);
//                            if (form != null)
//                            {
//                                decimal estimateAmount = form.Amount ?? 0;
//                                decimal availableBalance = estimateAmount - actualAmount;

//                                decimal totalAllocated = 0;
//                                var allocations = new List<Transaction>();

//                                foreach (RepeaterItem item in rptBudgetAllocations.Items)
//                                {
//                                    var hdnBudgetId = (HiddenField)item.FindControl("hdnBudgetId");
//                                    var txtAllocateAmount = (TextBox)item.FindControl("txtAllocateAmount");

//                                    if (Guid.TryParse(hdnBudgetId.Value, out Guid budgetId) &&
//                                        decimal.TryParse(txtAllocateAmount.Text, out decimal allocateAmount) &&
//                                        allocateAmount > 0)
//                                    {
//                                        // Find all old matching transactions (not just one)
//                                        var oldAllocations = db.Transactions
//                                            .Where(t => t.FromId == form.Id &&
//                                                        t.FromType == "Form" &&
//                                                        t.ToId == budgetId &&
//                                                        t.ToType == "Budget")
//                                            .ToList();

//                                        // Soft delete them all
//                                        foreach (var old in oldAllocations)
//                                            db.SoftDelete(old);

//                                        totalAllocated += allocateAmount;

//                                        allocations.Add(new Transaction
//                                        {
//                                            FromId = form.Id,
//                                            FromType = "Form",
//                                            ToId = budgetId,
//                                            ToType = "Budget",
//                                            Date = DateTime.Now,
//                                            Amount = allocateAmount,
//                                            Status = "Approved"
//                                        });
//                                    }
//                                }

//                                if (Math.Abs(totalAllocated - availableBalance) > 0.009m)
//                                {
//                                    lblAllocationError.Text = "Total allocated does not match available balance.";
//                                    lblAllocationError.CssClass = "text-danger";
//                                    lblAllocationError.Visible = true;
//                                    return;
//                                }

//                                // Save actual amount to form
//                                form.ActualAmount = actualAmount;
//                                form.Status = "Completed";
//                                db.Entry(form).State = EntityState.Modified;

//                                // Save file upload (if any)
//                                if (fuPO.HasFile)
//                                {
//                                    // Delete existing PO attachment for this form
//                                    var existingAttachment = db.Attachments
//                                        .FirstOrDefault(a => a.ObjectId == form.Id && a.ObjectType == "Form" && a.Type == "PO");

//                                    if (existingAttachment != null)
//                                    {
//                                        db.Attachments.Remove(existingAttachment);
//                                    }

//                                    var file = fuPO.PostedFile;
//                                    using (var binaryReader = new BinaryReader(file.InputStream))
//                                    {
//                                        var attachment = new Models.Attachment
//                                        {
//                                            ObjectId = form.Id,
//                                            ObjectType = "Form",
//                                            Type = "PO",
//                                            Name = Path.GetFileNameWithoutExtension(file.FileName),
//                                            FileName = Path.GetFileName(file.FileName),
//                                            ContentType = file.ContentType,
//                                            Content = binaryReader.ReadBytes(file.ContentLength),
//                                            Ext = Path.GetExtension(file.FileName),
//                                            Size = file.ContentLength,
//                                        };
//                                        db.Attachments.Add(attachment);
//                                    }
//                                }

//                                // Save allocations
//                                foreach (var transaction in allocations)
//                                {
//                                    db.Transactions.Add(transaction);
//                                }

//                                db.SaveChanges();
//                                trans.Commit();
//                                isSuccess = true;
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            trans.Rollback();
//                            SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
//                        }
//                    }
//                }

//                if (isSuccess)
//                {
//                    string script = @"
//                        Swal.fire({
//                            icon: 'success',
//                            title: 'Form updated successfully.',
//                            showConfirmButton: true
//                        }).then(function() {
//                            window.location.href = '" + Request.Url.GetLeftPart(UriPartial.Path) + @"';
//                        });";

//                    ScriptManager.RegisterStartupScript(this, GetType(), "alertRedirect", script, true);
//                }
//                else
//                {
//                    string script = @"
//                        Swal.fire({
//                            icon: 'error',
//                            title: 'Failed to update form. Please try again.',
//                            showConfirmButton: true
//                        }).then(function() {
//                            window.location.href = '" + Request.Url.GetLeftPart(UriPartial.Path) + @"';
//                        });";

//                    ScriptManager.RegisterStartupScript(this, GetType(), "alertRedirect", script, true);
//                }
//            }
//        }

//        [WebMethod]
//        public static List<Models.ViewModels.BudgetListViewModel> GetBudgets()
//        {
//            return new Class.Budget().GetBudgets(year: DateTime.Now.Year, bizAreaCode: Auth.User().iPMSBizAreaCode);
//        }

//        [WebMethod]
//        public static List<Models.ViewModels.FormBudgetListViewModel> GetSelectedBudgetIds(Guid formId)
//        {
//            using (var db = new AppDbContext())
//            {
//                return db.FormBudgets
//                    .Where(fb => fb.FormId == formId && fb.Type.ToLower() == "additional")
//                    .Select(fb => new
//                    {
//                        fb.BudgetId,
//                        fb.Amount
//                    })
//                    .AsEnumerable() // switch to LINQ to Objects
//                    .Select(fb => new Models.ViewModels.FormBudgetListViewModel
//                    {
//                        BudgetId = fb.BudgetId,
//                        Amount = fb.Amount.HasValue
//                            ? fb.Amount.Value.ToString("#,##0.00")
//                            : string.Empty
//                    })
//                    .ToList();
//            }
//        }

//        private void BindData(string id = null)
//        {
//            if (!string.IsNullOrEmpty(id))
//            {
//                using (var db = new AppDbContext())
//                {
//                    var form = db.Forms.Find(Guid.Parse(id));
//                    if (form != null)
//                    {
//                        lblTitle.Text = form.Ref;

//                        lblBA.Text = form.BizAreaCode + " - " + form.BizAreaName;
//                        lblReqName.Text = form.FormRequesterName;
//                        lblRefNo.Text = form.Ref;
//                        lblDate.Text = form.Date.HasValue ? form.Date.Value.ToString("dd/MM/yyyy") : "-";
//                        lblDetails.Text = form.Details;
//                        lblJustificationOfNeed.Text = form.JustificationOfNeed;
//                        lblAmount.Text = form.Amount.HasValue ? "RM" + form.Amount.Value.ToString("#,##0.00") : "-";
//                        lblAmount2.Text = form.Amount.HasValue ? "RM" + form.Amount.Value.ToString("#,##0.00") : "-";

//                        #region Allocation
//                        var budgets = db.FormBudgets
//                            .Where(fb => fb.FormId == form.Id && fb.Type.ToLower() == "new")
//                            .Select(fb => new
//                            {
//                                fb.BudgetId,
//                                fb.Budget.Ref,
//                                fb.Budget.Details,
//                                fb.Amount,
//                                AllocatedAmount = db.Transactions
//                                    .Where(t =>
//                                        t.FromId == fb.FormId &&
//                                        t.FromType == "Form" &&
//                                        t.ToId == fb.BudgetId &&
//                                        t.ToType == "Budget" &&
//                                        t.Status == "Approved" &&
//                                        t.DeletedBy == null
//                                    ) 
//                                    .Sum(t => (decimal?)t.Amount) ?? 0
//                            })
//                            .ToList();

//                        if (budgets.Any())
//                        {
//                            string html = "<ol style='padding-left: 15px; margin-bottom: 0;'>";

//                            foreach (var budget in budgets)
//                            {
//                                string amount = budget.Amount.HasValue ? "RM" + budget.Amount.Value.ToString("#,##0.00") : "-";
//                                string line = $"{budget.Ref} - {budget.Details} [{amount}]";
//                                html += $"<li>{Server.HtmlEncode(line)}</li>";
//                            }

//                            html += "</ol>";

//                            lblAllocation.Text = html;
//                        }
//                        else
//                        {
//                            lblAllocation.Text = "<em>No Budgets Assigned</em>";
//                        }
//                        #endregion

//                        #region Vendor
//                        var vendorNames = db.FormVendors
//                            .Where(fv => fv.FormId == form.Id)
//                            .Select(fv => fv.Vendor.Name)
//                            .ToList();

//                        if (vendorNames.Any())
//                        {
//                            string html = "<ol style='padding-left: 15px; margin-bottom: 0;'>";
//                            foreach (var name in vendorNames)
//                            {
//                                html += $"<li>{Server.HtmlEncode(name)}</li>";
//                            }
//                            html += "</ol>";
//                            lblVendor.Text = html;
//                        }
//                        else
//                        {
//                            lblVendor.Text = "<em>No Vendors Assigned</em>";
//                        }
//                        #endregion

//                        divJustificationDirectNegotiation.Visible = false;
//                        string procurementType = string.Empty;
//                        if (form.ProcurementType.Equals("quotation_inclusive", StringComparison.OrdinalIgnoreCase))
//                        {
//                            procurementType = "Inclusive Quotation";
//                        }
//                        else if (form.ProcurementType.Equals("quotation_selective", StringComparison.OrdinalIgnoreCase))
//                        {
//                            procurementType = "Selective Quotation";
//                        }
//                        else if (form.ProcurementType.Equals("direct_negotiation", StringComparison.OrdinalIgnoreCase))
//                        {
//                            procurementType = "Direct Negotiation";
//                            divJustificationDirectNegotiation.Visible = true;
//                        }

//                        lblProcurementType.Text = procurementType;
//                        lblJustificationDirectAward.Text = form.Justification;

//                        lblCurrentYearActualYTD.Text = form.CurrentYearActualYTD.HasValue ? form.CurrentYearActualYTD.Value.ToString("#,##0.00") : "-";
//                        lblCurrentYearBudget.Text = form.CurrentYearBudget.HasValue ? form.CurrentYearBudget.Value.ToString("#,##0.00") : "-";
//                        lblPreviousYearActualYTD.Text = form.PreviousYearActualYTD.HasValue ? form.PreviousYearActualYTD.Value.ToString("#,##0.00") : "-";
//                        lblPreviousYearActual.Text = form.PreviousYearActual.HasValue ? form.PreviousYearActual.Value.ToString("#,##0.00") : "-";
//                        lblPreviousYearBudget.Text = form.PreviousYearBudget.HasValue ? form.PreviousYearBudget.Value.ToString("#,##0.00") : "-";
//                        lblA.Text = form.A.HasValue ? form.A.Value.ToString("#,##0.00") : "-";
//                        lblB.Text = !string.IsNullOrEmpty(form.B) ? form.B : "-";
//                        lblC.Text = form.C.HasValue ? form.C.Value.ToString("#,##0.00") : "-";
//                        lblD.Text = form.D.HasValue ? form.D.Value.ToString("#,##0.00") : "-";

//                        if (form.Status != null)
//                        {
//                            switch (form.Status.ToLower())
//                            {
//                                case "approved":
//                                    lblStatus.Text = "Approved";
//                                    lblStatus.CssClass = "badge badge-success badge-pill";
//                                    break;
//                                case "pending":
//                                    lblStatus.Text = "Pending";
//                                    lblStatus.CssClass = "badge badge-warning badge-pill";
//                                    break;
//                                case "rejected":
//                                    lblStatus.Text = "Rejected";
//                                    lblStatus.CssClass = "badge badge-danger badge-pill";
//                                    break;
//                                case "draft":
//                                    lblStatus.Text = "Draft";
//                                    lblStatus.CssClass = "badge badge-info badge-pill";
//                                    break;
//                                case "sentback":
//                                    lblStatus.Text = "Sent Back";
//                                    lblStatus.CssClass = "badge badge-secondary badge-pill";
//                                    break;
//                                default:
//                                    lblStatus.Text = form.Status;
//                                    lblStatus.CssClass = "badge badge-secondary badge-pill";
//                                    break;
//                            }
//                        }

//                        #region Allocate budget
//                        rptBudgetAllocations.DataSource = budgets;
//                        rptBudgetAllocations.DataBind();
//                        #endregion

//                        txtActualAmount.Text = form.ActualAmount.HasValue ? form.ActualAmount.Value.ToString("#,##0.00") : string.Empty;

//                        #region PO
//                        var attachmentPO = db.Attachments
//                            .FirstOrDefault(a => a.ObjectId == form.Id && a.ObjectType == "Form" && a.Type == "PO");

//                        if (attachmentPO != null)
//                        {
//                            pnlPOView.Visible = true;
//                            lnkPO.NavigateUrl = $"~/DownloadAttachment.ashx?id={attachmentPO.Id}";
//                            lnkPO.Text = attachmentPO.FileName;
//                        }
//                        else
//                        {
//                            pnlPOView.Visible = false;
//                        }
//                        #endregion
//                    }
//                }
//            }
//        }

//        #region Documents
//        private void LoadAttachment(Guid formId, string type, HyperLink link, Panel viewPanel, Label dashLabel)
//        {
//            using (var db = new AppDbContext())
//            {
//                var attachment = db.Attachments.FirstOrDefault(a => a.ObjectId == formId && a.ObjectType.Equals("Form", StringComparison.OrdinalIgnoreCase) && a.Type.Equals(type, StringComparison.OrdinalIgnoreCase));

//                if (attachment != null)
//                {
//                    link.NavigateUrl = $"~/DownloadAttachment.ashx?id={attachment.Id}";
//                    link.Text = attachment.FileName;
//                    link.Visible = true;
//                    viewPanel.Visible = true;
//                    dashLabel.Visible = false; // Hide the dash label if the link is visible
//                }
//                else
//                {
//                    link.Visible = false;
//                    viewPanel.Visible = false;
//                    dashLabel.Visible = true; // Show the dash label if no attachment exists
//                }
//            }
//        }
//        #endregion

//        #region Audit Trails
//        protected void gvAuditTrails_PageIndexChanging(object sender, GridViewPageEventArgs e)
//        {
//            ViewState["pageIndex"] = e.NewPageIndex.ToString();
//            BindAuditTrails(Guid.Parse(hdnFormId.Value));
//        }

//        private void BindAuditTrails(Guid formId)
//        {
//            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

//            var auditTrails = new Class.Approval().GetApprovals(formId, "Form");
//            gvAuditTrails.DataSource = auditTrails;
//            gvAuditTrails.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
//            gvAuditTrails.DataBind();
//        }
//        #endregion
//    }
//}


