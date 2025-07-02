using CustomGuid.AspNet.Identity;
using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using Newtonsoft.Json;
using Prodata.WebForm.Class;
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
    public partial class Add : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControl(Auth.User().iPMSBizAreaCode);
                BindData();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                bool isSuccess = false;
                string formId = string.Empty;
                string ba = string.IsNullOrEmpty(Auth.User().iPMSBizAreaCode) ? ddlBA.SelectedValue : lblBAText.Text.Trim().Split(new string[] { " - " }, StringSplitOptions.None)[0];
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

                            var form = new Models.Form
                            {
                                TypeId = typeEntity.Id,
                                BizAreaCode = ba,
                                BizAreaName = new Class.IPMSBizArea().GetNameByCode(ba),
                                Date = date,
                                Ref = Functions.GetGeneratedRefNo("T1C", false),
                                Details = details,
                                JustificationOfNeed = justificationOfNeed,
                                Amount = amount,
                                ProcurementType = procurementType,
                                Justification = justification,
                                CurrentYearActualYTD = currentYearActualYTD,
                                CurrentYearBudget = currentYearBudget,
                                PreviousYearActualYTD = previousYearActualYTD,
                                PreviousYearActual = previousYearActual,
                                PreviousYearBudget = previousYearBudget,
                                A = a,
                                B = b,
                                C = c,
                                D = d,
                                Status = "Draft"
                            };

                            db.Forms.Add(form);
                            db.SaveChanges();

                            formId = form.Id.ToString();

                            if (!string.IsNullOrWhiteSpace(allocationList))
                            {
                                try
                                {
                                    var allocations = JsonConvert.DeserializeObject<List<AllocationItem>>(allocationList);

                                    var budgets = allocations.Select(x => new Models.FormBudget
                                    {
                                        FormId = form.Id,
                                        BudgetId = Guid.Parse(x.id),
                                        Amount = decimal.Parse(x.amount)
                                    }).ToList();

                                    db.FormBudgets.AddRange(budgets);
                                }
                                catch (Exception ex)
                                {
                                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
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

                                var formVendors = allVendors
                                    .Select(v => new Models.FormVendor
                                    {
                                        FormId = form.Id,
                                        VendorId = v.Id
                                    }).ToList();

                                db.FormVendors.AddRange(formVendors);
                            }

                            // insert attachments
                            SaveAttachment(db, fuPicture, "Picture", form.Id);
                            SaveAttachment(db, fuMachineRepairHistory, "MachineRepairHistory", form.Id);
                            SaveAttachment(db, fuJobSpecification, "JobSpecification", form.Id);
                            SaveAttachment(db, fuEngineerEstimatePrice, "EngineerEstimatePrice", form.Id);
                            SaveAttachment(db, fuDecCostReportCurrentYear, "DecCostReportCurrentYear", form.Id);
                            SaveAttachment(db, fuDecCostReportLastYear, "DecCostReportLastYear", form.Id);
                            SaveAttachment(db, fuCostReportLastMonth, "CostReportLastMonth", form.Id);
                            SaveAttachment(db, fuDrawingSketching, "DrawingSketching", form.Id);
                            SaveAttachment(db, fuQuotation, "Quotation", form.Id);
                            SaveAttachment(db, fuDamageInvestigationReport, "DamageInvestigationReport", form.Id);
                            SaveAttachment(db, fuVendorRegistrationRecord, "VendorRegistrationRecord", form.Id);
                            SaveAttachment(db, fuBudgetTransferAddApproval, "BudgetTransferAddApproval", form.Id);
                            SaveAttachment(db, fuOtherSupportingDocument, "OtherSupportingDocument", form.Id);

                            db.SaveChanges();
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
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Information saved.");
                    Response.Redirect("~/T1C/Edit?Id=" + formId);
                }
                else
                {
                    Response.Redirect(Request.Url.GetCurrentUrl());
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                bool isSuccess = false;
                string formId = string.Empty;
                string ba = string.IsNullOrEmpty(Auth.User().iPMSBizAreaCode) ? ddlBA.SelectedValue : lblBAText.Text.Trim().Split(new string[] { " - " }, StringSplitOptions.None)[0];
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

                            var form = new Models.Form
                            {
                                TypeId = typeEntity.Id,
                                BizAreaCode = ba,
                                BizAreaName = new Class.IPMSBizArea().GetNameByCode(ba),
                                Date = date,
                                Ref = Functions.GetGeneratedRefNo("T1C", false), //refNo,
                                Details = details,
                                JustificationOfNeed = justificationOfNeed,
                                Amount = amount,
                                ProcurementType = procurementType,
                                Justification = justification,
                                CurrentYearActualYTD = currentYearActualYTD,
                                CurrentYearBudget = currentYearBudget,
                                PreviousYearActualYTD = previousYearActualYTD,
                                PreviousYearActual = previousYearActual,
                                PreviousYearBudget = previousYearBudget,
                                A = a,
                                B = b,
                                C = c,
                                D = d,
                                Status = "Pending"
                            };

                            db.Forms.Add(form);
                            db.SaveChanges();

                            formId = form.Id.ToString();

                            if (!string.IsNullOrWhiteSpace(allocationList))
                            {
                                try
                                {
                                    var allocations = JsonConvert.DeserializeObject<List<AllocationItem>>(allocationList);

                                    var budgets = allocations.Select(x => new Models.FormBudget
                                    {
                                        FormId = form.Id,
                                        BudgetId = Guid.Parse(x.id),
                                        Amount = decimal.Parse(x.amount)
                                    }).ToList();

                                    db.FormBudgets.AddRange(budgets);
                                    db.SaveChanges();

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
                                            Amount = x.Amount
                                        }).ToList();

                                    db.Transactions.AddRange(transactions);
                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
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

                                var formVendors = allVendors
                                    .Select(v => new Models.FormVendor
                                    {
                                        FormId = form.Id,
                                        VendorId = v.Id
                                    }).ToList();

                                db.FormVendors.AddRange(formVendors);
                            }

                            // insert attachments
                            SaveAttachment(db, fuPicture, "Picture", form.Id);
                            SaveAttachment(db, fuMachineRepairHistory, "MachineRepairHistory", form.Id);
                            SaveAttachment(db, fuJobSpecification, "JobSpecification", form.Id);
                            SaveAttachment(db, fuEngineerEstimatePrice, "EngineerEstimatePrice", form.Id);
                            SaveAttachment(db, fuDecCostReportCurrentYear, "DecCostReportCurrentYear", form.Id);
                            SaveAttachment(db, fuDecCostReportLastYear, "DecCostReportLastYear", form.Id);
                            SaveAttachment(db, fuCostReportLastMonth, "CostReportLastMonth", form.Id);
                            SaveAttachment(db, fuDrawingSketching, "DrawingSketching", form.Id);
                            SaveAttachment(db, fuQuotation, "Quotation", form.Id);
                            SaveAttachment(db, fuDamageInvestigationReport, "DamageInvestigationReport", form.Id);
                            SaveAttachment(db, fuVendorRegistrationRecord, "VendorRegistrationRecord", form.Id);
                            SaveAttachment(db, fuBudgetTransferAddApproval, "BudgetTransferAddApproval", form.Id);
                            SaveAttachment(db, fuOtherSupportingDocument, "OtherSupportingDocument", form.Id);

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
                            SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
                        }
                    }
                }

                if (isSuccess)
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Information saved.");
                    Response.Redirect("~/T1C/Edit?Id=" + formId);
                }
                else
                {
                    Response.Redirect(Request.Url.GetCurrentUrl());
                }
            }
        }

        private void SaveAttachment(AppDbContext db, FileUpload fileUpload, string type, Guid formId)
        {
            if (fileUpload.HasFile)
            {
                var fileName = Path.GetFileName(fileUpload.FileName);
                var ext = Path.GetExtension(fileName);
                var contentType = fileUpload.PostedFile.ContentType;
                var size = fileUpload.PostedFile.ContentLength;

                using (var binaryReader = new BinaryReader(fileUpload.PostedFile.InputStream))
                {
                    var fileBytes = binaryReader.ReadBytes(size);

                    var attachment = new Attachment
                    {
                        ObjectId = formId,
                        ObjectType = "Form",
                        Type = type,
                        Name = fileName,
                        FileName = fileName,
                        ContentType = contentType,
                        Content = fileBytes,
                        Ext = ext,
                        Size = size
                    };

                    db.Attachments.Add(attachment);
                }
            }
        }

        protected void cvAmountLimit_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string roleCode = Auth.User().iPMSRoleCode;
            decimal amount;

            // Remove commas before parsing
            string rawText = txtAmount.Text.Replace(",", "");

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
        public static List<Models.ViewModels.BudgetListViewModel> GetBudgets()
        {
            return new Class.Budget().GetBudgets(year: DateTime.Now.Year, bizAreaCode: Auth.User().iPMSBizAreaCode);
        }

        private void BindControl(string ipmsBizAreaCode)
        {
            ddlBA.DataSource = new Class.IPMSBizArea().GetIPMSBizAreas();
            ddlBA.DataValueField = "Code";
            ddlBA.DataTextField = "DisplayName";
            ddlBA.DataBind();
            ddlBA.Items.Insert(0, new ListItem("", ""));

            var budgets = new Class.Budget().GetBudgets(year: DateTime.Now.Year, bizAreaCode: ipmsBizAreaCode);
            ddlAllocation.DataSource = budgets;
            ddlAllocation.DataValueField = "Id";
            ddlAllocation.DataTextField = "DisplayName";
            ddlAllocation.Attributes["data-amount"] = budgets.Any() ? budgets.First().Amount : "0";
            ddlAllocation.DataBind();
            ddlAllocation.Items.Insert(0, new ListItem("", ""));

            foreach (ListItem item in ddlAllocation.Items)
            {
                var budget = budgets.FirstOrDefault(b => b.Id.ToString() == item.Value);
                if (budget != null)
                {
                    item.Attributes["data-max"] = budget.Amount.Replace(",", ""); // Ensure it's in number format
                }
            }
        }

        private void BindData()
        {
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

            txtRefNo.Text = Functions.GetGeneratedRefNo("T1C", true);
            txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }
    }
}