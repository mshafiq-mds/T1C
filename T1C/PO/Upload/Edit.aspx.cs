using CustomGuid.AspNet.Identity;

using FGV.Prodata.App;

using FGV.Prodata.Web.UI;

using Prodata.WebForm.Helpers;

using Prodata.WebForm.Models;

using System;

using System.Collections.Generic;

using System.Data.Entity;

using System.IO;

using System.Linq;

using System.Web.UI;

using System.Web.UI.WebControls;

using Newtonsoft.Json;



namespace Prodata.WebForm.T1C.PO.Upload

{

    public partial class UploadPO : ProdataPage

    {

        public string BudgetsJson { get; set; }



        public class AllocationItem

        {

            public Guid BudgetId { get; set; }

            public decimal Amount { get; set; }

        }



        protected void Page_Load(object sender, EventArgs e)

        {

            if (!IsPostBack)

            {

                string id = Request.QueryString["Id"];

                if (Guid.TryParse(id, out Guid guid))

                {

                    hdnFormId.Value = guid.ToString();

                    BindData(guid.ToString());

                    BindAuditTrails(guid);

                    LoadAllDocuments(guid);

                }

                else

                {

                    Response.Redirect("~/T1C/PO/Upload");

                }

            }

        }



        private void BindData(string id)

        {

            using (var db = new AppDbContext())

            {

                var form = db.Forms.Find(Guid.Parse(id));

                if (form == null) return;

                DateTime? fdate = form.Date.HasValue ? form.Date : DateTime.Now;

                PopulateAllocationDropdown(form.BizAreaCode, fdate , form.Id);



                // Header Info

                lblTitle.Text = form.Ref;

                lblBA.Text = $"{form.BizAreaCode} - {form.BizAreaName}";

                lblReqName.Text = form.FormRequesterName;

                lblRefNo.Text = form.Ref;

                lblDate.Text = form.Date.HasValue ? form.Date.Value.ToString("dd/MM/yyyy") : "-";

                lblDetails.Text = form.Details;

                lblJustificationOfNeed.Text = form.JustificationOfNeed;

                lblAmount.Text = form.Amount.HasValue ? "RM " + form.Amount.Value.ToString("#,##0.00") : "-";

                lblAmount2.Text = lblAmount.Text;

                txtActualAmount.Text = form.ActualAmount.HasValue ? form.ActualAmount.Value.ToString("#,##0.00") : "";



                // Financial Table

                lblCurrentYearActualYTD.Text = form.CurrentYearActualYTD.HasValue ? form.CurrentYearActualYTD.Value.ToString("#,##0.00") : "-";

                lblCurrentYearBudget.Text = form.CurrentYearBudget.HasValue ? form.CurrentYearBudget.Value.ToString("#,##0.00") : "-";

                lblPreviousYearActualYTD.Text = form.PreviousYearActualYTD.HasValue ? form.PreviousYearActualYTD.Value.ToString("#,##0.00") : "-";

                lblPreviousYearActual.Text = form.PreviousYearActual.HasValue ? form.PreviousYearActual.Value.ToString("#,##0.00") : "-";

                lblPreviousYearBudget.Text = form.PreviousYearBudget.HasValue ? form.PreviousYearBudget.Value.ToString("#,##0.00") : "-";

                lblA.Text = form.A.HasValue ? form.A.Value.ToString("#,##0.00") : "-";

                lblB.Text = !string.IsNullOrEmpty(form.B) ? form.B : "-";

                lblC.Text = form.C.HasValue ? form.C.Value.ToString("#,##0.00") : "-";

                lblD.Text = form.D.HasValue ? form.D.Value.ToString("#,##0.00") : "-";



                // Procurement

                divJustificationDirectNegotiation.Visible = form.ProcurementType.Equals("direct_negotiation", StringComparison.OrdinalIgnoreCase);

                lblJustificationDirectAward.Text = form.Justification;

                string pt = "";

                if (form.ProcurementType.Equals("quotation_inclusive", StringComparison.OrdinalIgnoreCase)) pt = "Inclusive Quotation";

                else if (form.ProcurementType.Equals("quotation_selective", StringComparison.OrdinalIgnoreCase)) pt = "Selective Quotation";

                else if (form.ProcurementType.Equals("direct_negotiation", StringComparison.OrdinalIgnoreCase)) pt = "Direct Negotiation";

                lblProcurementType.Text = pt;



                // Vendor

                var vendorNames = db.FormVendors.Where(fv => fv.FormId == form.Id).Select(fv => fv.Vendor.Name).ToList();

                lblVendor.Text = vendorNames.Any() ? "<ol style='padding-left:15px;margin-bottom:0'>" + string.Join("", vendorNames.Select(v => $"<li>{Server.HtmlEncode(v)}</li>")) + "</ol>" : "<em>No Vendors</em>";



                // Allocations (Savings)

                var savingsBudgets = db.FormBudgets

                    .Where(fb => fb.FormId == form.Id && fb.Type.ToLower() == "new")

                    .Select(fb => new {

                        fb.BudgetId,

                        fb.Budget.Ref,

                        fb.Budget.Details,

                        fb.Amount,

                        AllocatedAmount = db.Transactions.Where(t => t.FromId == fb.FormId && t.ToId == fb.BudgetId && t.DeletedDate == null).Sum(t => (decimal?)t.Amount) ?? 0

                    }).ToList();



                rptBudgetAllocations.DataSource = savingsBudgets;

                rptBudgetAllocations.DataBind();



                // >>> CRITICAL FIX: Fill the lblAllocation list correctly <<<

                if (savingsBudgets.Any())

                {

                    string html = "<ol style='padding-left: 15px; margin-bottom: 0;'>";

                    foreach (var budget in savingsBudgets)

                    {

                        string amount = budget.Amount.HasValue ? "RM" + budget.Amount.Value.ToString("#,##0.00") : "-";

                        html += $"<li>{Server.HtmlEncode($"{budget.Ref} - {budget.Details} [{amount}]")}</li>";

                    }

                    html += "</ol>";

                    lblAllocation.Text = html;

                }

                else

                {

                    lblAllocation.Text = "<em>No Budgets Assigned</em>";

                }



                // Allocations (Overrun)

                rptOverrunAllocations.DataSource = db.Transactions

                    .Where(t => t.ToId == form.Id && t.Status == "PO Overrun Allocation" && t.DeletedDate == null)

                    .Select(t => new { BudgetId = t.FromId, Amount = t.Amount })

                    .ToList();

                rptOverrunAllocations.DataBind();



                // PO File

                var attachmentPO = db.Attachments.FirstOrDefault(a => a.ObjectId == form.Id && a.ObjectType == "Form" && a.Type == "PO");

                if (attachmentPO != null) { pnlPOView.Visible = true; lnkPO.NavigateUrl = $"~/DownloadAttachment.ashx?id={attachmentPO.Id}"; lnkPO.Text = attachmentPO.FileName; }

                else pnlPOView.Visible = false;



                SetStatusBadge(form.Status);



                // Set Initial Visibility

                decimal actual = form.ActualAmount ?? 0;

                decimal estimate = form.Amount ?? 0;

                if (actual > 0)

                {

                    allocationSection.Attributes["class"] = (actual < estimate) ? "form-group" : "form-group d-none";

                    allocationDropdownSection.Attributes["class"] = (actual > estimate) ? "form-group" : "form-group d-none";

                }

            }

        }



        private void LoadAllDocuments(Guid guid)

        {

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

                    dashLabel.Visible = false;

                }

                else

                {

                    link.Visible = false;

                    viewPanel.Visible = false;

                    dashLabel.Visible = true;

                }

            }

        }



        protected void rptOverrunAllocations_ItemDataBound(object sender, RepeaterItemEventArgs e)

        {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)

            {

                DropDownList ddl = (DropDownList)e.Item.FindControl("ddlAllocationItem");

                HiddenField hdnId = (HiddenField)e.Item.FindControl("hdnSelectedBudgetId");



                if (ddl != null && !string.IsNullOrEmpty(BudgetsJson))

                {

                    var budgets = JsonConvert.DeserializeObject<List<dynamic>>(BudgetsJson);

                    ddl.DataSource = budgets;

                    ddl.DataTextField = "DisplayText";

                    ddl.DataValueField = "BudgetId";

                    ddl.DataBind();

                    ddl.Items.Insert(0, new ListItem("Select Budget Source", ""));

                    if (hdnId != null && !string.IsNullOrEmpty(hdnId.Value)) ddl.SelectedValue = hdnId.Value;

                }

            }

        }



        private void PopulateAllocationDropdown(string bizAreaCode, DateTime? formdate, Guid? Formid)

        {

            using (var db = new AppDbContext())

            {

                // 1. Get Base Budgets
                // =========================================================================
                // STEP 0: Find the Target TypeId based on the Formid (from your SQL logic)
                // =========================================================================
                Guid? targetTypeId = null;

                if (Formid.HasValue)
                {
                    // Equivalent to:
                    // 1. select * from Transactions where ToId = Formid
                    // 2. select * from Budgets where id = Transactions.FromId
                    // 3. Get the TypeId
                    targetTypeId = db.Transactions.ExcludeSoftDeleted()
                        .Where(t => t.ToId == Formid.Value)
                        .Join(db.Budgets,                  // Join with Budgets table
                              t => t.FromId,               // Transaction.FromId
                              b => b.Id,                   // Budget.Id
                              (t, b) => b.TypeId)          // Select the TypeId
                        .FirstOrDefault();
                }

                // =========================================================================
                // STEP 1: Get Base Budgets (Updated with TypeId condition)
                // =========================================================================
                var query = db.Budgets.ExcludeSoftDeleted()
                    .Where(b => b.BizAreaCode == bizAreaCode
                             && b.Date.HasValue
                             && b.Date.Value.Year == formdate.Value.Year);

                // Apply the extra condition ONLY if we found a TypeId from the Form
                if (targetTypeId.HasValue)
                {
                    query = query.Where(b => b.TypeId == targetTypeId.Value);
                }

                var budgetQuery = query.ToList(); // Execute Query
                //var budgetQuery = db.Budgets.ExcludeSoftDeleted()

                //    .Where(b => b.BizAreaCode == bizAreaCode && b.Date.HasValue && b.Date.Value.Year == formdate.Value.Year )

                //    .ToList();



                var budgetIds = budgetQuery.Select(q => q.Id).ToList();



                // 2. Get Standard Utilized (Standard transactions - Returns)

                var utilizedMap = db.Transactions.ExcludeSoftDeleted()

                    .Where(t => t.Status.ToLower() != "rejected" &&

                          ((t.FromId.HasValue && budgetIds.Contains(t.FromId.Value) && t.FromType == "Budget") ||

                           (t.ToId.HasValue && budgetIds.Contains(t.ToId.Value) && t.ToType == "Budget")))

                    .ToList()

                    .GroupBy(t => t.FromType.ToLower() == "budget" ? t.FromId : t.ToId)

                    .ToDictionary(g => g.Key.Value, g => g.Sum(t => t.FromType.ToLower() == "budget" ? (t.Amount ?? 0m) : -(t.Amount ?? 0m)));



                // 3. Get Incoming Transfers (Top-ups)

                var transfersInMap = db.Transactions.ExcludeSoftDeleted()

                    .Where(t => t.ToId.HasValue && budgetIds.Contains(t.ToId.Value) && t.FromType == "Budget" && t.ToType == "Budget" && t.Status.ToLower() != "rejected")

                    .GroupBy(t => t.ToId.Value)

                    .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount ?? 0m));



                // 4. Map into a list that includes the calculated balance

                var dropdownSource = budgetQuery.Select(b => {

                    decimal baseAmt = b.Amount ?? 0m;

                    decimal topUp = transfersInMap.ContainsKey(b.Id) ? transfersInMap[b.Id] : 0m;

                    decimal used = utilizedMap.ContainsKey(b.Id) ? utilizedMap[b.Id] : 0m;

                    decimal currentBalance = (baseAmt + topUp) - used;



                    return new

                    {

                        BudgetId = b.Id,

                        DisplayText = $"{b.Ref} - {b.Details}",

                        Balance = currentBalance // This is used by JavaScript

                    };

                }).ToList();



                // Serialize for JS

                BudgetsJson = JsonConvert.SerializeObject(dropdownSource);

            }

        }



        protected void btnSave_Click(object sender, EventArgs e)

        {

            if (!decimal.TryParse(txtActualAmount.Text.Replace(",", ""), out decimal actualAmount)) return;



            using (var db = new AppDbContext())

            {

                using (var trans = db.Database.BeginTransaction())

                {

                    try

                    {

                        var form = db.Forms.Find(Guid.Parse(hdnFormId.Value));

                        decimal balance = (form.Amount ?? 0) - actualAmount; // Positive = Savings



                        // ---------------------------------------------------------

                        // SERVER-SIDE VALIDATION FOR SAVINGS (STRICT ENFORCEMENT)

                        // ---------------------------------------------------------

                        if (balance > 0)

                        {

                            decimal totalCheck = 0;

                            foreach (RepeaterItem ri in rptBudgetAllocations.Items)

                            {

                                decimal.TryParse(((TextBox)ri.FindControl("txtAllocateAmount")).Text, out decimal amt);

                                totalCheck += amt;

                            }



                            // If total allocated back doesn't equal savings balance -> STOP

                            if (Math.Abs(totalCheck - balance) > 0.01m)

                            {

                                lblAllocationError.Text = $"Server Validation Failed: Total savings allocated (RM {totalCheck:N2}) must equal (RM {balance:N2}).";

                                lblAllocationError.CssClass = "text-danger";

                                lblAllocationError.Visible = true;

                                return; // Blocks the Save

                            }

                        }



                        // ---------------------------------------------------------

                        // EXECUTE DB OPERATIONS

                        // ---------------------------------------------------------



                        // 1. Clean up ALL previous PO-related transactions

                        var oldTransactions = db.Transactions.Where(t => (t.FromId == form.Id || t.ToId == form.Id) && t.DeletedDate == null).ToList();

                        oldTransactions.ForEach(x => db.SoftDelete(x));



                        // 2. Overrun

                        if (balance < 0)

                        {

                            var items = JsonConvert.DeserializeObject<List<AllocationItem>>(hdnAllocationData.Value);

                            foreach (var item in items)

                            {

                                db.Transactions.Add(new Transaction

                                {

                                    Id = Guid.NewGuid(),

                                    FromId = item.BudgetId,

                                    FromType = "Budget",

                                    ToId = form.Id,

                                    ToType = "Form",

                                    Date = DateTime.Now,

                                    Amount = item.Amount,

                                    Status = "PO Overrun Allocation"

                                });

                            }

                        }

                        // 3. Savings

                        else if (balance > 0)

                        {

                            foreach (RepeaterItem ri in rptBudgetAllocations.Items)

                            {

                                var id = Guid.Parse(((HiddenField)ri.FindControl("hdnBudgetId")).Value);

                                decimal.TryParse(((TextBox)ri.FindControl("txtAllocateAmount")).Text, out decimal amt);

                                if (amt > 0) db.Transactions.Add(new Transaction { Id = Guid.NewGuid(), FromId = form.Id, FromType = "Form", ToId = id, ToType = "Budget", Date = DateTime.Now, Amount = amt, Status = "Approved" });

                            }

                        }



                        form.ActualAmount = actualAmount;

                        form.Status = "Completed";

                        db.Entry(form).State = EntityState.Modified;



                        // 4. File

                        if (fuPO.HasFile)

                        {

                            var oldFile = db.Attachments.FirstOrDefault(a => a.ObjectId == form.Id && a.Type == "PO");

                            if (oldFile != null) db.Attachments.Remove(oldFile);



                            using (var reader = new BinaryReader(fuPO.PostedFile.InputStream))

                            {

                                db.Attachments.Add(new Models.Attachment

                                {

                                    Id = Guid.NewGuid(),

                                    ObjectId = form.Id,

                                    ObjectType = "Form",

                                    Type = "PO",

                                    FileName = Path.GetFileName(fuPO.PostedFile.FileName),

                                    Content = reader.ReadBytes(fuPO.PostedFile.ContentLength),

                                    ContentType = fuPO.PostedFile.ContentType,

                                    Size = fuPO.PostedFile.ContentLength

                                });

                            }

                        }



                        db.SaveChanges();

                        trans.Commit();

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

                    catch (Exception ex) { trans.Rollback(); }

                }

            }

        }



        private void SetStatusBadge(string status)

        {

            lblStatus.Text = status;

            lblStatus.CssClass = "badge badge-info badge-pill";

            if (status?.ToLower() == "approved") lblStatus.CssClass = "badge badge-success badge-pill";

            else if (status?.ToLower() == "pending") lblStatus.CssClass = "badge badge-warning badge-pill";

            else if (status?.ToLower() == "rejected") lblStatus.CssClass = "badge badge-danger badge-pill";

        }



        protected void gvAuditTrails_PageIndexChanging(object sender, GridViewPageEventArgs e)

        {

            ViewState["pageIndex"] = e.NewPageIndex.ToString();

            BindAuditTrails(Guid.Parse(hdnFormId.Value));

        }



        private void BindAuditTrails(Guid formId)

        {

            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            var auditTrails = new Class.Approval().GetApprovals(formId, "Form");

            if (auditTrails != null && auditTrails.Count > 0)

            {

                var latestTrail = auditTrails.LastOrDefault();

                if (latestTrail != null && latestTrail.Action.Equals("Reviewed PO", StringComparison.OrdinalIgnoreCase)) btnSave.Visible = false;

                else btnSave.Visible = true;

            }

            gvAuditTrails.DataSource = auditTrails;

            gvAuditTrails.PageIndex = int.Parse(ViewState["pageIndex"].ToString());

            gvAuditTrails.DataBind();

        }

    }

}