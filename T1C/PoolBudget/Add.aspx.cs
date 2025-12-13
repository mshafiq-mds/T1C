using Antlr.Runtime.Misc;
using FGV.Prodata.Web.UI;
using NPOI.POIFS.NIO;
using NPOI.SS.Formula.Functions;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.T1C.PoolBudget
{
    public partial class Add : ProdataPage
    {
        string reftypecode = "T2";
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                Label1.Text = Functions.GetGeneratedRefNo(reftypecode, true);
                BindPurchaseType();
                BindBALabel();
                BindBudgetLabel();
            }
        }
        protected void BudgetType_CheckedChanged(object sender, EventArgs e)
        {
            PnlBekalan.Visible = rdoPeralatan.Checked;
            PnlBelian.Visible = rdoBelian.Checked;
        }
        /// <summary>
        /// Handle Bekalan button click (BudgetType = 5)
        /// </summary>
        protected void btnPnlBekalan_Click(object sender, EventArgs e)
        {
            CreateFormAndTransaction(
                budgetTypeCode: "5",
                baCode: LblBA.Text.Trim(),
                baName: LblBAName.Text.Trim(),
                justification: txtJustification.Text.Trim(),
                amountText: txtAmount.Text.Trim(),
                details: ddlPT.SelectedValue,
                fromId: hdnGuidBekalan.Value,
                fuDd: fuDocumentBekalan
            );
        }

        /// <summary>
        /// Handle Belian button click (BudgetType = 2)
        /// </summary>
        protected void btnPnlBelian_Click(object sender, EventArgs e)
        {
            CreateFormAndTransaction(
                budgetTypeCode: "2",
                baCode: lblBABelian.Text.Trim(),
                baName: lblBANameBelian.Text.Trim(),
                justification: txtJustificationBelian.Text.Trim(),
                amountText: txtAmountBelian.Text.Trim(),
                details: ddlBelian.SelectedValue,
                fromId: hdnGuidBelian.Value,
                fuDd: fuDocumentBelian
            );
        }

        /// Common method to create FormsProcurement + Transaction
        private void CreateFormAndTransaction(
            string budgetTypeCode,
            string baCode,
            string baName,
            string justification,
            string amountText,
            string details,
            string fromId,
            FileUpload fuDd
            )
        {
            using (var db = new AppDbContext())
            {
                try
                {
                    // Parse amount safely
                    decimal amount = !string.IsNullOrEmpty(amountText)
                        ? decimal.Parse(amountText.Replace(",", ""))
                        : 0;

                    // Get BudgetType Id from code
                    var typeEntity = db.BudgetTypes
                        .Where(t => t.Code == budgetTypeCode)
                        .Select(t => t.Id)
                        .FirstOrDefault();

                    // Create procurement form record
                    var form = new Models.FormsProcurement
                    {
                        TypeId = typeEntity,
                        BizAreaCode = baCode,
                        BizAreaName = new Class.IPMSBizArea().GetNameByCode(baCode),
                        Date = DateTime.Now,
                        Ref = Functions.GetGeneratedRefNo(reftypecode, false), // unique ref no
                        JustificationOfNeed = justification,
                        Amount = amount,
                        Details = details,
                        Status = "Pending"
                    };

                    db.FormsProcurement.Add(form);
                    db.SaveChanges();

                    // Create transaction linked to procurement form
                    var transaction = new Models.Transaction
                    {
                        FromId = Guid.Parse(fromId),
                        FromType = "Budget",
                        ToId = form.Id,
                        ToType = "FormsProcurement",
                        Date = DateTime.Now,
                        Ref = form.Ref,
                        Name = "-",
                        Amount = amount,
                        Status = "Submitted"
                    };

                    db.Transactions.Add(transaction);
                    db.SaveChanges();

                    if (fuDd.HasFile)
                    {
                        using (var binaryReader = new System.IO.BinaryReader(fuDd.PostedFile.InputStream))
                        {
                            byte[] fileData = binaryReader.ReadBytes(fuDd.PostedFile.ContentLength);

                            var document = new FormsProcurementDocuments
                            {
                                TransferId = form.Id,
                                FileName = fuDd.FileName,
                                ContentType = fuDd.PostedFile.ContentType,
                                FileData = fileData,
                                UploadedBy = Auth.Id(),
                                UploadedDate = DateTime.Now
                            };

                            db.FormsProcurementDocuments.Add(document);
                            db.SaveChanges();
                        }
                    }

                    // Function Emails
                    Emails.EmailsT2ForNewRequest(form.Id, form , Auth.User().CCMSRoleCode);

                    // Show success alert
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Successful Create Transaction.");

                    // Redirect with slight delay → allow SweetAlert to display first
                    ScriptManager.RegisterStartupScript(
                        this,
                        GetType(),
                        "redirect",
                        "setTimeout(function(){ window.location = '" + ResolveUrl("~/T1C/PoolBudget") + "'; }, 100);",
                        true);
                }
                catch (Exception ex)
                {
                    // Show error if exception occurs
                    //SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, ex.Message);
                    string script = $"Swal.fire({{ icon: 'error', title: 'Error', text: '{ex.Message.Replace("'", "\\'")}' }});";
                    ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlertError", script, true);
                }
            }
        }

        private void BindPurchaseType()
        {
            using (var db = new AppDbContext())
            {
                var dataSource = db.PurchaseTypes
                    .Where(x => x.DeletedDate == null)
                    .OrderBy(x => x.Code)
                    .ToList();

                ddlPT.DataSource = dataSource;
                ddlPT.DataValueField = "Name";
                ddlPT.DataTextField = "Name";
                ddlPT.DataBind();
                ddlPT.Items.Insert(0, new ListItem("-- Select Purchase Type --", ""));

                // Insert default first option
                ddlBelian.Items.Insert(0, new ListItem("-- Select Purchase Type --", ""));

                // Insert hardcoded items
                ddlBelian.Items.Add(new ListItem("Purchase Spare Parts", "SpareParts"));
                ddlBelian.Items.Add(new ListItem("STO", "STO"));
                ddlBelian.Items.Add(new ListItem("NDC", "NDC"));
            }
        }
        private void BindBudgetLabel()
        {
            string typebudgetCodeBekalan = "5";
            string typebudgetCodeBelian = "2";
            string BA = Auth.User().CCMSBizAreaCode;

            using (var db = new AppDbContext())
            {
                // ===== BEKALAN =====
                var TypeGuidIdBekalan = db.BudgetTypes
                    .Where(b => b.Code == typebudgetCodeBekalan && b.DeletedBy == null)
                    .Select(b => b.Id)
                    .FirstOrDefault();

                var TotalBudgetBekalan = db.Budgets
                    .Where(x => x.TypeId == TypeGuidIdBekalan && x.BizAreaCode == BA && x.Date.Value.Year == DateTime.Today.Year && x.DeletedBy == null)
                    .Select(x => new { x.Amount, x.Id })
                    .FirstOrDefault();

                decimal budgetBekalanAmount = TotalBudgetBekalan?.Amount ?? 0;
                Guid budgetBekalanId = TotalBudgetBekalan?.Id ?? Guid.Empty;

                decimal totalOutflowBekalan = db.Transactions
                    .Where(x => x.FromId == budgetBekalanId && x.DeletedBy == null)
                    .Sum(x => (decimal?)x.Amount)
                    .GetValueOrDefault();

                decimal totalInflowBekalan = db.Transactions
                    .Where(x => x.ToId == budgetBekalanId && x.DeletedBy == null)
                    .Sum(x => (decimal?)x.Amount)
                    .GetValueOrDefault();

                decimal balanceBudgetBekalan = budgetBekalanAmount - totalOutflowBekalan + totalInflowBekalan;

                LblBalanceBekalan.Text = balanceBudgetBekalan.ToString("N2");
                LblBugetBekalan.Text = budgetBekalanAmount.ToString("N2");
                if (TotalBudgetBekalan != null && TotalBudgetBekalan.Id != Guid.Empty)
                {
                    hdnGuidBekalan.Value = TotalBudgetBekalan.Id.ToString();
                } 

                // ===== BELIAN =====
                var TypeGuidIdBelian = db.BudgetTypes
                    .Where(b => b.Code == typebudgetCodeBelian && b.DeletedBy == null)
                    .Select(b => b.Id)
                    .FirstOrDefault();

                var TotalBudgetBelian = db.Budgets
                    .Where(x => x.TypeId == TypeGuidIdBelian && x.BizAreaCode == BA && x.Date.Value.Year == DateTime.Today.Year && x.DeletedBy == null)
                    .Select(x => new { x.Amount, x.Id })
                    .FirstOrDefault();

                decimal budgetBelianAmount = TotalBudgetBelian?.Amount ?? 0;
                Guid budgetBelianId = TotalBudgetBelian?.Id ?? Guid.Empty;

                decimal totalOutflowBelian = db.Transactions
                    .Where(x => x.FromId == budgetBelianId && x.DeletedBy == null)
                    .Sum(x => (decimal?)x.Amount)
                    .GetValueOrDefault();

                decimal totalInflowBelian = db.Transactions
                    .Where(x => x.ToId == budgetBelianId && x.DeletedBy == null)
                    .Sum(x => (decimal?)x.Amount)
                    .GetValueOrDefault();

                decimal balanceBudgetBelian = budgetBelianAmount - totalOutflowBelian + totalInflowBelian;

                LblBalanceBelian.Text = balanceBudgetBelian.ToString("N2");
                LblBudgeteBelian.Text = budgetBelianAmount.ToString("N2");
                if (TotalBudgetBelian != null && TotalBudgetBelian.Id != Guid.Empty)
                {
                    hdnGuidBelian.Value = TotalBudgetBelian.Id.ToString();
                } 
            }
        }


        private void BindBALabel()
        {
            string ba = Auth.User().CCMSBizAreaCode;

            bool isEmptyBA = string.IsNullOrWhiteSpace(ba);

            phBA.Visible = phBABelian.Visible = !isEmptyBA;
            phBAPh.Visible = phBABelianph.Visible = isEmptyBA;

            if (!isEmptyBA)
            {
                LblBA.Text = lblBABelian.Text = ba;
                LblBAName.Text = lblBANameBelian.Text = new Class.IPMSBizArea().GetNameByCode(ba) ?? "";
            } 
        }
    }
}