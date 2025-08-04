using CustomGuid.AspNet.Identity;
using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using Newtonsoft.Json;
using NPOI.HSSF.Record.Chart;
using Prodata.WebForm.Class;
using Prodata.WebForm.Helpers;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.T1C
{
    public partial class Edit : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControl();
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
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                bool isSuccess = false;
                string formId = hdnFormId.Value;
                Guid parsedFormId = Guid.Parse(formId);
                string ba = !string.IsNullOrEmpty(Auth.User().iPMSBizAreaCode) ? ddlBA.SelectedValue : lblBAText.Text.Trim().Split(new string[] { " - " }, StringSplitOptions.None)[0];
                string refNo = txtRefNo.Text.Trim();
                DateTime? date = !string.IsNullOrEmpty(txtDate.Text.Trim()) ? DateTime.Parse(txtDate.Text.Trim()) : (DateTime?)null;
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
                    {
                        type = "B";
                    }
                    else
                    {
                        type = "A";
                    }
                }
                else
                {
                    if (procurementType.Equals("direct_negotiation", StringComparison.OrdinalIgnoreCase))
                    {
                        type = "D";
                    }
                    else
                    {
                        type = "C";
                    }
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
                            //form.Ref = refNo;
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
                                    // Remove existing budgets for this form
                                    var existingBudgets = db.FormBudgets.Where(fb => fb.FormId == form.Id && fb.Type.ToLower() == "new").ToList();
                                    db.FormBudgets.RemoveRange(existingBudgets);
                                    db.SaveChanges(); // Save deletion first

                                    // Deserialize new allocation list
                                    var allocations = JsonConvert.DeserializeObject<List<AllocationItem>>(allocationList);

                                    // Add updated budgets
                                    var budgets = allocations.Select(x => new Models.FormBudget
                                    {
                                        FormId = form.Id,
                                        BudgetId = Guid.Parse(x.id),
                                        Amount = decimal.Parse(x.amount),
                                        Type = "New"
                                    }).ToList();
                                    db.FormBudgets.AddRange(budgets);
                                    db.SaveChanges(); // Save additions

                                    // Remove existing transactions for this form
                                    var existingTransactions = db.Transactions
                                        .Where(t => t.FromType.Equals("Budget", StringComparison.OrdinalIgnoreCase) 
                                                 && t.ToId == form.Id 
                                                 && t.ToType.Equals("Form", StringComparison.OrdinalIgnoreCase))
                                        .ToList();
                                    foreach (var transaction in existingTransactions)
                                    {
                                        db.SoftDelete(transaction); // Soft delete existing transactions
                                    }

                                    // Add new transactions
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

                                // Remove existing vendors for this form
                                var existingFormVendors = db.FormVendors.Where(fv => fv.FormId == form.Id);
                                db.FormVendors.RemoveRange(existingFormVendors);
                                db.SaveChanges();

                                // Add updated vendors
                                var formVendors = allVendors
                                    .Select(v => new Models.FormVendor
                                    {
                                        FormId = form.Id,
                                        VendorId = v.Id
                                    }).ToList();

                                db.FormVendors.AddRange(formVendors);
                                db.SaveChanges();
                            }

                            // Add or update attachments
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

                if (isSuccess)
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

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                bool isSuccess = false;
                string formId = hdnFormId.Value;
                Guid parsedFormId = Guid.Parse(formId);
                string ba = !string.IsNullOrEmpty(Auth.User().iPMSBizAreaCode) ? ddlBA.SelectedValue : lblBAText.Text.Trim().Split(new string[] { " - " }, StringSplitOptions.None)[0];
                string refNo = txtRefNo.Text.Trim();
                DateTime? date = !string.IsNullOrEmpty(txtDate.Text.Trim()) ? DateTime.Parse(txtDate.Text.Trim()) : (DateTime?)null;
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
                    {
                        type = "B";
                    }
                    else
                    {
                        type = "A";
                    }
                }
                else
                {
                    if (procurementType.Equals("direct_negotiation", StringComparison.OrdinalIgnoreCase))
                    {
                        type = "D";
                    }
                    else
                    {
                        type = "C";
                    }
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
                            //form.Ref = refNo;
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
                            form.Status = "Pending";
                            form.UpdatedBy = Auth.Id();
                            form.UpdatedDate = DateTime.Now;
                            db.Entry(form).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            if (!string.IsNullOrWhiteSpace(allocationList))
                            {
                                try
                                {
                                    // Remove existing budgets for this form
                                    var existingBudgets = db.FormBudgets.Where(fb => fb.FormId == form.Id && fb.Type.ToLower() == "new").ToList();
                                    db.FormBudgets.RemoveRange(existingBudgets);
                                    db.SaveChanges(); // Save deletion first

                                    // Deserialize new allocation list
                                    var allocations = JsonConvert.DeserializeObject<List<AllocationItem>>(allocationList);

                                    // Add updated budgets
                                    var budgets = allocations.Select(x => new Models.FormBudget
                                    {
                                        FormId = form.Id,
                                        BudgetId = Guid.Parse(x.id),
                                        Amount = decimal.Parse(x.amount),
                                        Type = "New"
                                    }).ToList();
                                    db.FormBudgets.AddRange(budgets);
                                    db.SaveChanges(); // Save additions

                                    // Remove existing transactions for this form
                                    var existingTransactions = db.Transactions
                                        .Where(t => t.FromType.Equals("Budget", StringComparison.OrdinalIgnoreCase) 
                                                 && t.ToId == form.Id 
                                                 && t.ToType.Equals("Form", StringComparison.OrdinalIgnoreCase))
                                        .ToList();
                                    foreach (var transaction in existingTransactions)
                                    {
                                        db.SoftDelete(transaction); // Soft delete existing transactions
                                    }

                                    // Add new transactions
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

                                // Remove existing vendors for this form
                                var existingFormVendors = db.FormVendors.Where(fv => fv.FormId == form.Id);
                                db.FormVendors.RemoveRange(existingFormVendors);
                                db.SaveChanges();

                                // Add updated vendors
                                var formVendors = allVendors
                                    .Select(v => new Models.FormVendor
                                    {
                                        FormId = form.Id,
                                        VendorId = v.Id
                                    }).ToList();

                                db.FormVendors.AddRange(formVendors);
                                db.SaveChanges();
                            }

                            // Add or update attachments
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

                            db.Approvals.Add(new Models.Approval
                            {
                                ObjectId = form.Id,
                                ObjectType = "Form",
                                ActionById = Auth.User().Id,
                                ActionByType = "User",
                                ActionByCode = Auth.User().iPMSRoleCode,
                                ActionByName = Auth.User().Name,
                                Action = "Submitted",
                                Section = "Applicant"
                            });
                            db.SaveChanges();

                            trans.Commit();
                            isSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, ex.Message);
                        }
                    }
                }

                if (isSuccess)
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Form submitted.");
                }
                else
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Failed to update form.");
                }
                Response.Redirect(Request.Url.GetCurrentUrl(true));
            }
        }

        protected void cvAmountLimit_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string roleCode = Auth.User().iPMSRoleCode;
            decimal amount;

            // Remove commas before parsing
            string rawText = txtAmount.Text.Trim().Replace(",", "");

            if (!decimal.TryParse(rawText, out amount))
            {
                args.IsValid = false;
                return;
            }

            if (roleCode.Equals("kb", StringComparison.OrdinalIgnoreCase))
            {
                args.IsValid = amount <= 10000;
            }
            else if (roleCode.Equals("mm", StringComparison.OrdinalIgnoreCase))
            {
                args.IsValid = amount >= 10000.01m;
            }
            else
            {
                args.IsValid = true;
            }
        }

        [WebMethod]
        public static List<Models.ViewModels.BudgetListViewModel> GetBudgets(string excludedFormId)
        {
            return new Class.Budget().GetBudgets(year: DateTime.Now.Year, bizAreaCode: Auth.User().iPMSBizAreaCode, excludedFormId: Guid.Parse(excludedFormId));
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

        private void BindData(string id = null)
        {
            if (!string.IsNullOrEmpty(id))
            {
                using (var db = new AppDbContext())
                {
                    var form = db.Forms.Find(Guid.Parse(id));
                    if (form != null)
                    {
                        ddlBA.SelectedValue = form.BizAreaCode;
                        txtRefNo.Text = form.Ref;
                        txtDate.Text = form.Date.HasValue ? form.Date.Value.ToString("yyyy-MM-dd") : string.Empty;
                        txtDetails.Text = form.Details;
                        txtJustificationOfNeed.Text = form.JustificationOfNeed;
                        txtAmount.Text = form.Amount.HasValue ? form.Amount.Value.ToString("#,##0.00") : string.Empty;
                        rblProcurementType.SelectedValue = form.ProcurementType;
                        txtJustificationDirectAward.Text = form.Justification;
                        txtCurrentYearActualYTD.Text = form.CurrentYearActualYTD.HasValue ? form.CurrentYearActualYTD.Value.ToString("#,##0.00") : string.Empty;
                        txtCurrentYearBudget.Text = form.CurrentYearBudget.HasValue ? form.CurrentYearBudget.Value.ToString("#,##0.00") : string.Empty;
                        txtPreviousYearActualYTD.Text = form.PreviousYearActualYTD.HasValue ? form.PreviousYearActualYTD.Value.ToString("#,##0.00") : string.Empty;
                        txtPreviousYearActual.Text = form.PreviousYearActual.HasValue ? form.PreviousYearActual.Value.ToString("#,##0.00") : string.Empty;
                        txtPreviousYearBudget.Text = form.PreviousYearBudget.HasValue ? form.PreviousYearBudget.Value.ToString("#,##0.00") : string.Empty;
                        txtA.Text = form.A.HasValue ? form.A.Value.ToString("#,##0.00") : string.Empty;
                        txtB.Text = form.B;
                        txtC.Text = form.C.HasValue ? form.C.Value.ToString("#,##0.00") : string.Empty;
                        txtD.Text = form.D.HasValue ? form.D.Value.ToString("#,##0.00") : string.Empty;
                    }
                }
            }

            if (!string.IsNullOrEmpty(Auth.User().iPMSBizAreaCode))
            {
                lblBAText.Visible = true;
                ddlBA.Visible = false;
                rfvBA.Visible = false;

                string baCode = Auth.User().iPMSBizAreaCode;
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

        #region Attachment Handling
        private void LoadAttachment(Guid formId, string type, HyperLink link, Panel viewPanel, Panel uploadPanel)
        {
            using (var db = new AppDbContext())
            {
                var attachment = db.Attachments
                    .FirstOrDefault(a => a.ObjectId == formId && a.ObjectType == "Form" && a.Type == type);

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
                var attachment = db.Attachments
                    .FirstOrDefault(a => a.ObjectId == formId && a.ObjectType == "Form" && a.Type == type);

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
                // Delete old if exists
                var existing = db.Attachments
                    .FirstOrDefault(a => a.ObjectId == formId && a.ObjectType == "Form" && a.Type == type);

                if (existing != null)
                    db.Attachments.Remove(existing);

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

        protected void btnDeletePicture_Click(object sender, EventArgs e)
        {
            var formId = Guid.Parse(hdnFormId.Value);
            DeleteAttachment(formId, "Picture");
            LoadAttachment(formId, "Picture", lnkPicture, pnlPictureView, pnlPictureUpload);
        }

        protected void btnDeleteMachineRepairHistory_Click(object sender, EventArgs e)
        {
            var formId = Guid.Parse(hdnFormId.Value);
            DeleteAttachment(formId, "MachineRepairHistory");
            LoadAttachment(formId, "MachineRepairHistory", lnkMachineRepairHistory, pnlMachineRepairHistoryView, pnlMachineRepairHistoryUpload);
        }

        protected void btnDeleteJobSpecification_Click(object sender, EventArgs e)
        {
            var formId = Guid.Parse(hdnFormId.Value);
            DeleteAttachment(formId, "JobSpecification");
            LoadAttachment(formId, "JobSpecification", lnkJobSpecification, pnlJobSpecificationView, pnlJobSpecificationUpload);
        }

        protected void btnDeleteEngineerEstimatePrice_Click(object sender, EventArgs e)
        {
            var formId = Guid.Parse(hdnFormId.Value);
            DeleteAttachment(formId, "EngineerEstimatePrice");
            LoadAttachment(formId, "EngineerEstimatePrice", lnkEngineerEstimatePrice, pnlEngineerEstimatePriceView, pnlEngineerEstimatePriceUpload);
        }

        protected void btnDeleteDecCostReportCurrentYear_Click(object sender, EventArgs e)
        {
            var formId = Guid.Parse(hdnFormId.Value);
            DeleteAttachment(formId, "DecCostReportCurrentYear");
            LoadAttachment(formId, "DecCostReportCurrentYear", lnkDecCostReportCurrentYear, pnlDecCostReportCurrentYearView, pnlDecCostReportCurrentYearUpload);
        }

        protected void btnDeleteDecCostReportLastYear_Click(object sender, EventArgs e)
        {
            var formId = Guid.Parse(hdnFormId.Value);
            DeleteAttachment(formId, "DecCostReportLastYear");
            LoadAttachment(formId, "DecCostReportLastYear", lnkDecCostReportLastYear, pnlDecCostReportLastYearView, pnlDecCostReportLastYearUpload);
        }

        protected void btnDeleteCostReportLastMonth_Click(object sender, EventArgs e)
        {
            var formId = Guid.Parse(hdnFormId.Value);
            DeleteAttachment(formId, "CostReportLastMonth");
            LoadAttachment(formId, "CostReportLastMonth", lnkCostReportLastMonth, pnlCostReportLastMonthView, pnlCostReportLastMonthUpload);
        }

        protected void btnDeleteDrawingSketching_Click(object sender, EventArgs e)
        {
            var formId = Guid.Parse(hdnFormId.Value);
            DeleteAttachment(formId, "DrawingSketching");
            LoadAttachment(formId, "DrawingSketching", lnkDrawingSketching, pnlDrawingSketchingView, pnlDrawingSketchingUpload);
        }

        protected void btnDeleteQuotation_Click(object sender, EventArgs e)
        {
            var formId = Guid.Parse(hdnFormId.Value);
            DeleteAttachment(formId, "Quotation");
            LoadAttachment(formId, "Quotation", lnkQuotation, pnlQuotationView, pnlQuotationUpload);
        }

        protected void btnDeleteDamageInvestigationReport_Click(object sender, EventArgs e)
        {
            var formId = Guid.Parse(hdnFormId.Value);
            DeleteAttachment(formId, "DamageInvestigationReport");
            LoadAttachment(formId, "DamageInvestigationReport", lnkDamageInvestigationReport, pnlDamageInvestigationReportView, pnlDamageInvestigationReportUpload);
        }

        protected void btnDeleteVendorRegistrationRecord_Click(object sender, EventArgs e)
        {
            var formId = Guid.Parse(hdnFormId.Value);
            DeleteAttachment(formId, "VendorRegistrationRecord");
            LoadAttachment(formId, "VendorRegistrationRecord", lnkVendorRegistrationRecord, pnlVendorRegistrationRecordView, pnlVendorRegistrationRecordUpload);
        }

        protected void btnDeleteBudgetTransferAddApproval_Click(object sender, EventArgs e)
        {
            var formId = Guid.Parse(hdnFormId.Value);
            DeleteAttachment(formId, "BudgetTransferAddApproval");
            LoadAttachment(formId, "BudgetTransferAddApproval", lnkBudgetTransferAddApproval, pnlBudgetTransferAddApprovalView, pnlBudgetTransferAddApprovalUpload);
        }

        protected void btnDeleteOtherSupportingDocument_Click(object sender, EventArgs e)
        {
            var formId = Guid.Parse(hdnFormId.Value);
            DeleteAttachment(formId, "OtherSupportingDocument");
            LoadAttachment(formId, "OtherSupportingDocument", lnkOtherSupportingDocument, pnlOtherSupportingDocumentView, pnlOtherSupportingDocumentUpload);
        }
        #endregion
    }
}