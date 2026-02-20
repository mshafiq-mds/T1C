using CustomGuid.AspNet.Identity;
using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using Newtonsoft.Json;
using Prodata.WebForm.Class;
using Prodata.WebForm.Helpers;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.MasterData;
using Prodata.WebForm.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.T1C
{
    public partial class BudgetEdit : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int[] allowedFormGroups = { 1, 3 };

            if (!IsPostBack)
            {
                BindControl(); 
                BindDropdown(ddlBT, Functions.GetBudgetTypes(), "ID", "DisplayName", allowedFormGroups);
                BindYearDropdown(); // Ensure year is bound

                ToggleDataCostVisibility();

                if (Request.QueryString["Id"] != null)
                {
                    string id = Request.QueryString["Id"].ToString();
                    Guid guid;
                    if (Guid.TryParse(id, out guid))
                    {
                        if (!FormHelper.IsFormEditable(guid))
                        {
                            Response.Redirect("~/T1C");
                        }

                        if (FormHelper.IsFormSentBack(guid))
                        {
                            alert.Visible = true;
                            lblAlert.Text = FormHelper.GetLatestFormRemark(guid);
                            btnSubmitLabel.Text = "Resubmit";
                            SweetAlert.SetAlert(SweetAlert.SweetAlertType.Warning, "This form has been sent back for correction. Please review the comments and make necessary changes before submitting again.");
                        }
                        else
                        {
                            alert.Visible = false;
                        }

                        hdnFormId.Value = id;
                        BindData(id);

                        LoadAttachment(guid, "Picture", lnkPicture, pnlPictureView, pnlPictureUpload);
                        LoadAttachment(guid, "MachineRepairHistory", lnkMachineRepairHistory, pnlMachineRepairHistoryView, pnlMachineRepairHistoryUpload);
                        LoadAttachment(guid, "JobSpecification", lnkJobSpecification, pnlJobSpecificationView, pnlJobSpecificationUpload);
                        LoadAttachment(guid, "EngineerEstimatePrice", lnkEngineerEstimatePrice, pnlEngineerEstimatePriceView, pnlEngineerEstimatePriceUpload);
                        LoadAttachment(guid, "DecCostReportCurrentYear", lnkDecCostReportCurrentYear, pnlDecCostReportCurrentYearView, pnlDecCostReportCurrentYearUpload);
                        LoadAttachment(guid, "DecCostReportLastYear", lnkDecCostReportLastYear, pnlDecCostReportLastYearView, pnlDecCostReportLastYearUpload);
                        LoadAttachment(guid, "CostReportLastMonth", lnkCostReportLastMonth, pnlCostReportLastMonthView, pnlCostReportLastMonthUpload);
                        LoadAttachment(guid, "DrawingSketching", lnkDrawingSketching, pnlDrawingSketchingView, pnlDrawingSketchingUpload);
                        LoadAttachment(guid, "Quotation", lnkQuotation, pnlQuotationView, pnlQuotationUpload);
                        LoadAttachment(guid, "DamageInvestigationReport", lnkDamageInvestigationReport, pnlDamageInvestigationReportView, pnlDamageInvestigationReportUpload);
                        LoadAttachment(guid, "VendorRegistrationRecord", lnkVendorRegistrationRecord, pnlVendorRegistrationRecordView, pnlVendorRegistrationRecordUpload);
                        LoadAttachment(guid, "BudgetTransferAddApproval", lnkBudgetTransferAddApproval, pnlBudgetTransferAddApprovalView, pnlBudgetTransferAddApprovalUpload);
                        LoadAttachment(guid, "OtherSupportingDocument", lnkOtherSupportingDocument, pnlOtherSupportingDocumentView, pnlOtherSupportingDocumentUpload);
                    }
                }
                else
                {
                    Response.Redirect("~/T1C");
                }
            }

            // Ensure JSON is populated on every load/postback for the modal
            PopulateBudgetsJson();
        }
        private void PopulateBudgetsJson()
        {
            string json = "[]";
            btnSelectAllocations.Enabled = false;

            // 1. Validate inputs and parse once
            if (Guid.TryParse(ddlBT.SelectedValue, out Guid selectedGuid) &&
                int.TryParse(ddlYear.SelectedValue, out int selectedYear))
            {
                try
                {
                    // 2. Fetch the specific budget type to check categories
                    var selectedType = Functions.GetBudgetTypes().FirstOrDefault(x => x.Id == selectedGuid);

                    if (selectedType != null)
                    {
                        Guid? excludedId = !string.IsNullOrEmpty(hdnFormId.Value) ? (Guid?)Guid.Parse(hdnFormId.Value) : null;

                        // 3. Logic Branching: Determine if bizAreaCode is required
                        // We assign the result to a single variable used later
                        var budgets = (selectedType.BudgetCategories != 3)
                            ? new Class.Budget().GetBudgets(typeId: selectedGuid, year: selectedYear, bizAreaCode: Auth.User().CCMSBizAreaCode, excludedFormId: excludedId)
                            : new Class.Budget().GetBudgets(typeId: selectedGuid, year: selectedYear, excludedFormId: excludedId);

                        // 4. Serialize if data exists
                        if (budgets != null && budgets.Any())
                        {
                            var budgetData = budgets.Select(b => new
                            {
                                b.Id,
                                b.Ref,
                                Display = b.DisplayName,
                                b.Amount,
                                // Strip formatting and handle nulls for the decimal conversion
                                Balance = decimal.TryParse(b.Amount?.Replace(",", ""), out decimal val) ? val : 0
                            });

                            json = JsonConvert.SerializeObject(budgetData);
                            btnSelectAllocations.Enabled = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Logging the error is recommended here
                    json = "[]";
                }
            }

            // Assign directly to the hidden field
            hdnBudgetsJson.Value = json;
        }
        private void PopulateBudgetsJsonOld()
        {
            string json = "[]";
            btnSelectAllocations.Enabled = false;

            // Only run if both Type and Year are selected
            if (!string.IsNullOrEmpty(ddlBT.SelectedValue) && !string.IsNullOrEmpty(ddlYear.SelectedValue))
            {
                if (Guid.TryParse(ddlBT.SelectedValue, out Guid selectedGuid) && int.TryParse(ddlYear.SelectedValue, out int selectedYear))
                {
                    try
                    {
                        // Assuming Class.Budget().GetBudgets has been updated to accept these parameters based on your snippet
                        var budgets = new Class.Budget().GetBudgets(
                             typeId: selectedGuid,
                             year: selectedYear,
                             bizAreaCode: Auth.User().CCMSBizAreaCode,
                             excludedFormId: !string.IsNullOrEmpty(hdnFormId.Value) ? (Guid?)Guid.Parse(hdnFormId.Value) : null
                        );

                        if (budgets != null && budgets.Any())
                        {
                            json = JsonConvert.SerializeObject(budgets.Select(b => new
                            {
                                Id = b.Id,
                                Ref = b.Ref,
                                Display = b.DisplayName,
                                Amount = b.Amount,
                                // Parse Amount string back to decimal for JS calculations
                                Balance = decimal.TryParse(b.Amount.Replace(",", ""), out decimal val) ? val : 0
                            }));

                            btnSelectAllocations.Enabled = true;
                        }
                    }
                    catch (Exception)
                    {
                        // Handle case where method signature might not match or DB error
                        json = "[]";
                    }
                }
            }

            // FIX: Assign 'json' string directly. Do NOT call SerializeObject again.
            hdnBudgetsJson.Value = json;
        }

        private void PopulateBudgetsJsonold()
        {
            // Fetch budgets excluding current form usage (optional, depends on logic) or all available
            var budgets1 = GetBudgets(hdnFormId.Value);

            var json = JsonConvert.SerializeObject(budgets1.Select(b => new
            {
                Id = b.Id,
                Ref = b.Ref,
                Display = b.DisplayName,
                Amount = b.Amount,
                // Parse Amount string back to decimal for JS calculations
                Balance = decimal.TryParse(b.Amount.Replace(",", ""), out decimal val) ? val : 0
            }));

            // Store in HiddenField instead of public property
            hdnBudgetsJson.Value = json;
        }

        protected void BudgetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Re-populate JSON when type changes
            PopulateBudgetsJson();
            ToggleDataCostVisibility();
        }

        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Re-populate JSON when year changes
            PopulateBudgetsJson();
            ToggleDataCostVisibility();
        }
        private void ToggleDataCostVisibility()
        {
            if (!string.IsNullOrEmpty(ddlBT.SelectedValue))
            {
                Guid selectedTypeId = Guid.Parse(ddlBT.SelectedValue);

                // Fetch the list of budget types (same method used in BindDropdown)
                var allTypes = Functions.GetBudgetTypes();

                // Find the specific type selected
                var selectedType = allTypes.FirstOrDefault(x => x.Id == selectedTypeId);

                if (selectedType != null)
                {
                    // Requirement: If BudgetCategories is 3, hide the Data Cost section
                    // Otherwise, show it.
                    phDataCost.Visible = (selectedType.BudgetCategories != 3);
                }
            }
            else
            {
                // Default visibility if nothing is selected
                phDataCost.Visible = true;
            }
        }
        // ==========================================
        // END FIX
        // ==========================================

        //private void BindDropdown(ListControl ddl, object dataSource, string valueField, string textField)
        //{
        //    ddl.DataSource = dataSource;
        //    ddl.DataValueField = valueField;
        //    ddl.DataTextField = textField;
        //    ddl.DataBind();
        //    ddl.Items.Insert(0, new ListItem("", ""));
        //}
        private void BindDropdown(ListControl ddl, object dataSource, string dataValueField, string dataTextField, int[] formCategory = null)
        {
            // Try casting to List<BudgetType>
            if (dataSource is IEnumerable<BudgetType> list)
            {
                list = list.Where(x => formCategory.Contains(x.FormCategories)).ToList();
                dataSource = list;
            }

            ddl.DataSource = dataSource;
            ddl.DataValueField = dataValueField;
            ddl.DataTextField = dataTextField;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("", ""));
        }

        private void BindYearDropdown()
        {
            int currentYear = DateTime.Now.Year;
            ddlYear.Items.Clear();
            for (int i = currentYear + 1; i >= currentYear - 3; i--)
                ddlYear.Items.Add(new ListItem(i.ToString(), i.ToString()));

            ddlYear.SelectedValue = currentYear.ToString();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValid)
            { 
                if (updatedb())
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Form updated successfully.");
                }
                else
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Failed to update form.");
                }
                Response.Redirect(Request.Url.GetCurrentUrl(true));
            }
        }

        public bool updatedb()
        {
            bool isSuccess = false;
            string formId = hdnFormId.Value;
            Guid parsedFormId = Guid.Parse(formId);
            string ba = !string.IsNullOrEmpty(Auth.User().CCMSBizAreaCode) ? ddlBA.SelectedValue : lblBAText.Text.Trim().Split(new string[] { " - " }, StringSplitOptions.None)[0];
            string refNo = txtRefNo.Text.Trim();
            DateTime? date = !string.IsNullOrEmpty(txtDate.Text.Trim()) ? DateTime.Parse(txtDate.Text.Trim()) : (DateTime?)null;
            string reqName = txtReqName.Text.Trim();
            string details = txtDetails.Text.Trim();
            string justificationOfNeed = txtJustificationOfNeed.Text.Trim();
            decimal amount = !string.IsNullOrEmpty(txtAmount.Text.Trim()) ? decimal.Parse(txtAmount.Text.Trim().Replace(",", "")) : 0;
            string procurementType = rblProcurementType.SelectedValue;
            string justification = txtJustificationDirectAward.Text.Trim();
            decimal currentYearActualYTD = !string.IsNullOrEmpty(txtCurrentYearActualYTD.Text.Trim()) ? decimal.Parse(txtCurrentYearActualYTD.Text.Trim().Replace(",", "")) : 0;
            decimal currentYearBudget = !string.IsNullOrEmpty(txtCurrentYearBudget.Text.Trim()) ? decimal.Parse(txtCurrentYearBudget.Text.Trim().Replace(",", "")) : 0;
            decimal previousYearActualYTD = !string.IsNullOrEmpty(txtPreviousYearActualYTD.Text.Trim()) ? decimal.Parse(txtPreviousYearActualYTD.Text.Trim().Replace(",", "")) : 0;
            decimal previousYearActual = !string.IsNullOrEmpty(txtPreviousYearActual.Text.Trim()) ? decimal.Parse(txtPreviousYearActual.Text.Trim().Replace(",", "")) : 0;
            decimal previousYearBudget = !string.IsNullOrEmpty(txtPreviousYearBudget.Text.Trim()) ? decimal.Parse(txtPreviousYearBudget.Text.Trim().Replace(",", "")) : 0;
            decimal a = !string.IsNullOrEmpty(txtA.Text.Trim()) ? decimal.Parse(txtA.Text.Trim().Replace(",", "")) : 0;
            string b = txtB.Text.Trim();
            decimal c = !string.IsNullOrEmpty(txtC.Text.Trim()) ? decimal.Parse(txtC.Text.Trim().Replace(",", "")) : 0;
            decimal d = !string.IsNullOrEmpty(txtD.Text.Trim()) ? decimal.Parse(txtD.Text.Trim().Replace(",", "")) : 0;

            string allocationList = hdnAllocationList.Value;
            string vendorList = hdnVendorList.Value;

            string type = string.Empty;
            if (amount <= 100000)
            {
                if (procurementType.Equals("direct_negotiation", StringComparison.OrdinalIgnoreCase))
                    type = "B";
                else
                    type = "A";
            }
            else
            {
                if (procurementType.Equals("direct_negotiation", StringComparison.OrdinalIgnoreCase))
                    type = "D";
                else
                    type = "C";
            }

            using (var db = new AppDbContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var typeEntity = db.FormTypes
                            .FirstOrDefault(t => t.Code.Equals(type, StringComparison.OrdinalIgnoreCase));

                        if (typeEntity == null)
                            throw new Exception("Form type not found.");

                        var form = db.Forms.Find(parsedFormId);
                        form.TypeId = typeEntity.Id;
                        form.BizAreaCode = ba;
                        form.BizAreaName = new Class.IPMSBizArea().GetNameByCode(ba);
                        form.Date = date;
                        form.FormRequesterName = reqName;
                        form.Details = details;
                        form.JustificationOfNeed = justificationOfNeed;
                        form.Amount = amount;
                        form.ProcurementType = procurementType;
                        form.Justification = justification;
                        form.CurrentYearActualYTD = currentYearActualYTD;
                        form.CurrentYearBudget = currentYearBudget;
                        form.PreviousYearActualYTD = previousYearActualYTD;
                        form.PreviousYearActual = previousYearActual;
                        form.PreviousYearBudget = previousYearBudget;
                        form.A = a;
                        form.B = b;
                        form.C = c;
                        form.D = d;
                        form.UpdatedBy = Auth.Id();
                        form.UpdatedDate = DateTime.Now;
                        db.Entry(form).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        if (!string.IsNullOrWhiteSpace(allocationList))
                        {
                            try
                            {
                                var existingBudgets = db.FormBudgets.Where(fb => fb.FormId == form.Id && fb.Type.ToLower() == "new").ToList();
                                db.FormBudgets.RemoveRange(existingBudgets);
                                db.SaveChanges();

                                var allocations = JsonConvert.DeserializeObject<List<AllocationItem>>(allocationList);

                                var budgets = allocations.Select(x => new Models.FormBudget
                                {
                                    FormId = form.Id,
                                    BudgetId = Guid.Parse(x.id),
                                    Amount = decimal.Parse(x.amount),
                                    Type = "New"
                                }).ToList();
                                db.FormBudgets.AddRange(budgets);
                                db.SaveChanges();

                                var existingTransactions = db.Transactions
                                    .Where(t => t.FromType.Equals("Budget", StringComparison.OrdinalIgnoreCase)
                                             && t.ToId == form.Id
                                             && t.ToType.Equals("Form", StringComparison.OrdinalIgnoreCase))
                                    .ToList();
                                foreach (var transaction in existingTransactions)
                                {
                                    db.SoftDelete(transaction);
                                }

                                var transactions = budgets
                                    .Where(x => x.Amount > 0)
                                    .Select(x => new Models.Transaction
                                    {
                                        FromId = x.BudgetId,
                                        FromType = "Budget",
                                        ToId = form.Id,
                                        ToType = "Form",
                                        Date = DateTime.Now,
                                        Ref = form.Ref,
                                        Name = "-",
                                        Amount = x.Amount,
                                        Status = form.Status.Equals("Draft", StringComparison.OrdinalIgnoreCase)
                                                 ? "Floating"
                                                 : form.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)
                                                    ? "Submitted"
                                                    : form.Status
                                    }).ToList();

                                db.Transactions.AddRange(transactions);
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(vendorList))
                        {
                            var vendorNames = vendorList.Split(',')
                                .Select(x => x.Trim())
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .ToList();

                            var existingVendors = db.Vendors
                                .Where(v => vendorNames.Contains(v.Name))
                                .ToList();

                            var existingNames = existingVendors
                                .Select(v => v.Name)
                                .ToList();

                            var newVendorNames = vendorNames
                                .Where(name => !existingNames.Contains(name, StringComparer.OrdinalIgnoreCase))
                                .ToList();

                            var newVendors = newVendorNames
                                .Select(name => new Models.Vendor { Name = name })
                                .ToList();

                            db.Vendors.AddRange(newVendors);
                            db.SaveChanges();

                            var allVendors = existingVendors.Concat(newVendors).ToList();

                            var existingFormVendors = db.FormVendors.Where(fv => fv.FormId == form.Id);
                            db.FormVendors.RemoveRange(existingFormVendors);
                            db.SaveChanges();

                            var formVendors = allVendors
                                .Select(v => new Models.FormVendor
                                {
                                    FormId = form.Id,
                                    VendorId = v.Id
                                }).ToList();

                            db.FormVendors.AddRange(formVendors);
                            db.SaveChanges();
                        }

                        // Save Attachments
                        SaveAttachment(form.Id, fuPicture, "Picture");
                        SaveAttachment(form.Id, fuMachineRepairHistory, "MachineRepairHistory");
                        SaveAttachment(form.Id, fuJobSpecification, "JobSpecification");
                        SaveAttachment(form.Id, fuEngineerEstimatePrice, "EngineerEstimatePrice");
                        SaveAttachment(form.Id, fuDecCostReportCurrentYear, "DecCostReportCurrentYear");
                        SaveAttachment(form.Id, fuDecCostReportLastYear, "DecCostReportLastYear");
                        SaveAttachment(form.Id, fuCostReportLastMonth, "CostReportLastMonth");
                        SaveAttachment(form.Id, fuDrawingSketching, "DrawingSketching");
                        SaveAttachment(form.Id, fuQuotation, "Quotation");
                        SaveAttachment(form.Id, fuDamageInvestigationReport, "DamageInvestigationReport");
                        SaveAttachment(form.Id, fuVendorRegistrationRecord, "VendorRegistrationRecord");
                        SaveAttachment(form.Id, fuBudgetTransferAddApproval, "BudgetTransferAddApproval");
                        SaveAttachment(form.Id, fuOtherSupportingDocument, "OtherSupportingDocument");

                        trans.Commit();
                        isSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
                    }
                }
            }
            return isSuccess;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                bool isSuccess = updatedb();
                string formId = hdnFormId.Value;
                Guid parsedFormId = Guid.Parse(formId);

                if (isSuccess)
                {
                    using (var db = new AppDbContext())
                    {
                        using (var trans = db.Database.BeginTransaction())
                        {
                            try
                            {
                                var form = db.Forms.Find(parsedFormId);

                                // Re-run the save logic to ensure latest data
                                // (For brevity, in real app, consolidate Save logic into a helper method)

                                // 1. Update form status
                                form.Status = "Pending";
                                form.UpdatedBy = Auth.Id();
                                form.UpdatedDate = DateTime.Now;
                                db.Entry(form).State = System.Data.Entity.EntityState.Modified;

                                // 2. Log Approval
                                db.Approvals.Add(new Models.Approval
                                {
                                    ObjectId = form.Id,
                                    ObjectType = "Form",
                                    ActionById = Auth.User().Id,
                                    ActionByType = "User",
                                    ActionByCode = Auth.User().CCMSRoleCode,
                                    ActionByName = Auth.User().Name,
                                    Action = "Submitted",
                                    Section = "Applicant"
                                });

                                db.SaveChanges();
                                trans.Commit();
                                isSuccess = true;

                                // Send Email
                                Emails.EmailsT1CForNewRequest(form.Id, form, Auth.User().CCMSRoleCode);
                            }
                            catch (Exception ex)
                            {
                                trans.Rollback();
                                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, ex.Message);
                            }
                        }
                    }
                }

                if (isSuccess)
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Form submitted.");
                    Response.Redirect(Request.Url.GetCurrentUrl(true));
                }
                else
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Failed to submit form.");
                }   
            }
        }

        // Web Methods
        [WebMethod]
        public static List<Models.ViewModels.BudgetListViewModel> GetBudgets(string excludedFormId)
        {
            Guid? excludeId = null;
            if (Guid.TryParse(excludedFormId, out Guid result)) excludeId = result;

            return new Class.Budget().GetBudgets(year: DateTime.Now.Year, bizAreaCode: Auth.User().CCMSBizAreaCode, excludedFormId: excludeId);
        }

        [WebMethod]
        public static List<Models.ViewModels.FormBudgetListViewModel> GetSelectedBudgetIds(Guid formId)
        {
            using (var db = new AppDbContext())
            {
                return db.FormBudgets
                    .Where(fb => fb.FormId == formId && fb.Type.ToLower() == "new")
                    .Select(fb => new
                    {
                        fb.BudgetId,
                        fb.Amount
                    })
                    .AsEnumerable()
                    .Select(fb => new Models.ViewModels.FormBudgetListViewModel
                    {
                        BudgetId = fb.BudgetId,
                        Amount = fb.Amount.HasValue ? fb.Amount.Value.ToString("#,##0.00") : ""
                    })
                    .ToList();
            }
        }

        [WebMethod]
        public static List<string> GetVendorsByForm(string formId)
        {
            Guid parsedFormId = Guid.Parse(formId);
            using (var db = new AppDbContext())
            {
                return (from fv in db.FormVendors
                        join v in db.Vendors on fv.VendorId equals v.Id
                        where fv.FormId == parsedFormId
                        select v.Name).ToList();
            }
        }

        private void BindControl()
        {
            ddlBA.DataSource = new Class.IPMSBizArea().GetIPMSBizAreas();
            ddlBA.DataValueField = "Code";
            ddlBA.DataTextField = "DisplayName";
            ddlBA.DataBind();
            ddlBA.Items.Insert(0, new ListItem("", ""));
        }

        private void BindData(string id)
        {
            using (var db = new AppDbContext())
            {
                var form = db.Forms.Find(Guid.Parse(id));
                if (form != null)
                {
                    ddlBA.SelectedValue = form.BizAreaCode;
                    txtRefNo.Text = form.Ref;
                    txtDate.Text = form.Date.HasValue ? form.Date.Value.ToString("yyyy-MM-dd") : "";
                    txtReqName.Text = form.FormRequesterName;
                    txtDetails.Text = form.Details;
                    txtJustificationOfNeed.Text = form.JustificationOfNeed;
                    txtAmount.Text = form.Amount.HasValue ? form.Amount.Value.ToString("#,##0.00") : "";

                    if (!string.IsNullOrEmpty(form.ProcurementType))
                        rblProcurementType.SelectedValue = form.ProcurementType;

                    txtJustificationDirectAward.Text = form.Justification;

                    txtCurrentYearActualYTD.Text = form.CurrentYearActualYTD?.ToString("#,##0.00");
                    txtCurrentYearBudget.Text = form.CurrentYearBudget?.ToString("#,##0.00");
                    txtPreviousYearActualYTD.Text = form.PreviousYearActualYTD?.ToString("#,##0.00");
                    txtPreviousYearActual.Text = form.PreviousYearActual?.ToString("#,##0.00");
                    txtPreviousYearBudget.Text = form.PreviousYearBudget?.ToString("#,##0.00");
                    txtA.Text = form.A?.ToString("#,##0.00");
                    txtB.Text = form.B;
                    txtC.Text = form.C?.ToString("#,##0.00");
                    txtD.Text = form.D?.ToString("#,##0.00");
                }
            }

            if (!string.IsNullOrEmpty(Auth.User().CCMSBizAreaCode))
            {
                lblBAText.Visible = true;
                ddlBA.Visible = false;
                rfvBA.Visible = false;
                string baCode = Auth.User().CCMSBizAreaCode;
                string baName = new Class.IPMSBizArea().GetNameByCode(baCode);
                lblBAText.Text = baCode + " - " + baName;
            }
            else
            {
                lblBAText.Visible = false;
                ddlBA.Visible = true;
                rfvBA.Visible = true;
            }
        }

        #region Attachments
        private void LoadAttachment(Guid formId, string type, HyperLink link, Panel viewPanel, Panel uploadPanel)
        {
            using (var db = new AppDbContext())
            {
                var attachment = db.Attachments.FirstOrDefault(a => a.ObjectId == formId && a.ObjectType == "Form" && a.Type == type);
                if (attachment != null)
                {
                    viewPanel.Visible = true;
                    uploadPanel.Visible = false;
                    link.NavigateUrl = $"~/DownloadAttachment.ashx?id={attachment.Id}";
                    link.Text = attachment.FileName;
                }
                else
                {
                    viewPanel.Visible = false;
                    uploadPanel.Visible = true;
                }
            }
        }
        private void DeleteAttachment(Guid formId, string type)
        {
            using (var db = new AppDbContext())
            {
                var attachment = db.Attachments.FirstOrDefault(a => a.ObjectId == formId && a.ObjectType == "Form" && a.Type == type);
                if (attachment != null)
                {
                    db.Attachments.Remove(attachment);
                    db.SaveChanges();
                }
            }
        }
        private void SaveAttachment(Guid formId, FileUpload fileUpload, string type)
        {
            if (!fileUpload.HasFile) return;
            using (var db = new AppDbContext())
            {
                var existing = db.Attachments.FirstOrDefault(a => a.ObjectId == formId && a.ObjectType == "Form" && a.Type == type);
                if (existing != null) db.Attachments.Remove(existing);

                var fileBytes = fileUpload.FileBytes;
                var fileName = Path.GetFileName(fileUpload.FileName);
                var attachment = new Attachment
                {
                    ObjectId = formId,
                    ObjectType = "Form",
                    Type = type,
                    Name = Path.GetFileNameWithoutExtension(fileName),
                    FileName = fileName,
                    ContentType = fileUpload.PostedFile.ContentType,
                    Content = fileBytes,
                    Ext = Path.GetExtension(fileName),
                    Size = fileBytes.Length,
                    UploadedDate = DateTime.Now,
                    UploadedBy = Prodata.WebForm.Auth.Id()
                };
                db.Attachments.Add(attachment);
                db.SaveChanges();
            }
        }

        // Attachment Buttons
        protected void btnDeletePicture_Click(object sender, EventArgs e) { DeleteAttachment(Guid.Parse(hdnFormId.Value), "Picture"); Response.Redirect(Request.RawUrl); }
        protected void btnDeleteMachineRepairHistory_Click(object sender, EventArgs e) { DeleteAttachment(Guid.Parse(hdnFormId.Value), "MachineRepairHistory"); Response.Redirect(Request.RawUrl); }
        protected void btnDeleteJobSpecification_Click(object sender, EventArgs e) { DeleteAttachment(Guid.Parse(hdnFormId.Value), "JobSpecification"); Response.Redirect(Request.RawUrl); }
        protected void btnDeleteEngineerEstimatePrice_Click(object sender, EventArgs e) { DeleteAttachment(Guid.Parse(hdnFormId.Value), "EngineerEstimatePrice"); Response.Redirect(Request.RawUrl); }
        protected void btnDeleteDecCostReportCurrentYear_Click(object sender, EventArgs e) { DeleteAttachment(Guid.Parse(hdnFormId.Value), "DecCostReportCurrentYear"); Response.Redirect(Request.RawUrl); }
        protected void btnDeleteDecCostReportLastYear_Click(object sender, EventArgs e) { DeleteAttachment(Guid.Parse(hdnFormId.Value), "DecCostReportLastYear"); Response.Redirect(Request.RawUrl); }
        protected void btnDeleteCostReportLastMonth_Click(object sender, EventArgs e) { DeleteAttachment(Guid.Parse(hdnFormId.Value), "CostReportLastMonth"); Response.Redirect(Request.RawUrl); }
        protected void btnDeleteDrawingSketching_Click(object sender, EventArgs e) { DeleteAttachment(Guid.Parse(hdnFormId.Value), "DrawingSketching"); Response.Redirect(Request.RawUrl); }
        protected void btnDeleteQuotation_Click(object sender, EventArgs e) { DeleteAttachment(Guid.Parse(hdnFormId.Value), "Quotation"); Response.Redirect(Request.RawUrl); }
        protected void btnDeleteDamageInvestigationReport_Click(object sender, EventArgs e) { DeleteAttachment(Guid.Parse(hdnFormId.Value), "DamageInvestigationReport"); Response.Redirect(Request.RawUrl); }
        protected void btnDeleteVendorRegistrationRecord_Click(object sender, EventArgs e) { DeleteAttachment(Guid.Parse(hdnFormId.Value), "VendorRegistrationRecord"); Response.Redirect(Request.RawUrl); }
        protected void btnDeleteBudgetTransferAddApproval_Click(object sender, EventArgs e) { DeleteAttachment(Guid.Parse(hdnFormId.Value), "BudgetTransferAddApproval"); Response.Redirect(Request.RawUrl); }
        protected void btnDeleteOtherSupportingDocument_Click(object sender, EventArgs e) { DeleteAttachment(Guid.Parse(hdnFormId.Value), "OtherSupportingDocument"); Response.Redirect(Request.RawUrl); }
        #endregion
    }

    // Helper class for JSON Deserialization
    public class AllocationItem
    {
        public string id { get; set; }
        public string amount { get; set; }
    }
}