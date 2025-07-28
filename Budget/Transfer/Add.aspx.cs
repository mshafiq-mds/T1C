using FGV.Prodata.Web.UI;
using NPOI.SS.Formula.Functions;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.Auth;
using Prodata.WebForm.Models.MasterData;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

namespace Prodata.WebForm.Budget.Transfer
{
    public partial class Add : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControl();
                BindBALabel();

                txtEVisa.Text = txtRefNo.Text = Functions.GetGeneratedRefNo("PB", true); 

                txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            }
        }
        protected void btnSubmit_Click1(object sender, EventArgs e)
        {
            Guid newId;

            using (var db = new AppDbContext())
            {
                do
                {
                    newId = Guid.NewGuid();
                } while (db.TransfersTransaction.Any(x => x.Id == newId));

                string refNo = Functions.GetGeneratedRefNo("PB", false);

                if(txtRefNo.Text != refNo)
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Ref No. " + txtRefNo.Text + " already exists and has been updated to a new Ref No: " + refNo + ".");

                var model = new TransfersTransaction
                {
                    BA = Auth.User().iPMSBizAreaCode,
                    Id = newId,
                    RefNo = refNo,
                    Project = txtProject.Text.Trim(),
                    Date = string.IsNullOrWhiteSpace(txtDate.Text) ? DateTime.Today : DateTime.Parse(txtDate.Text),
                    BudgetType = rdoOpex.Checked ? "OPEX" : "CAPEX",
                    EstimatedCost = string.IsNullOrWhiteSpace(txtEstimatedCost.Text) ? 0 : Convert.ToDecimal(txtEstimatedCost.Text),
                    Justification = txtJustification.Text.Trim(),
                    EVisaNo = refNo,
                    WorkDetails = txtWorkDetails.Text.Trim(),

                    FromGL = Guid.TryParse(txtFromGL.Text.Trim(), out var fromGLGuid) ? fromGLGuid : Guid.Empty,
                    FromBA = ddFromBA.SelectedValue,
                    FromBudget = string.IsNullOrWhiteSpace(txtFromBudget.Text) ? 0 : Convert.ToDecimal(txtFromBudget.Text),
                    FromBalance = string.IsNullOrWhiteSpace(txtFromBalance.Text) ? 0 : Convert.ToDecimal(txtFromBalance.Text),
                    FromTransfer = string.IsNullOrWhiteSpace(txtFromTransfer.Text) ? 0 : Convert.ToDecimal(txtFromTransfer.Text),
                    FromAfter = string.IsNullOrWhiteSpace(txtFromAfter.Text) ? 0 : Convert.ToDecimal(txtFromAfter.Text),  

                    ToGL = Guid.TryParse(txtToGL.Text.Trim(), out var toGLGuid) ? toGLGuid : Guid.Empty,
                    ToBA = lblToBA.Text.Trim(),
                    ToBudget = string.IsNullOrWhiteSpace(txtToBudget.Text) ? 0 : Convert.ToDecimal(txtToBudget.Text),
                    ToBalance = string.IsNullOrWhiteSpace(txtToBalance.Text) ? 0 : Convert.ToDecimal(txtToBalance.Text),
                    ToTransfer = string.IsNullOrWhiteSpace(txtToTransfer.Text) ? 0 : Convert.ToDecimal(txtToTransfer.Text),  
                    ToAfter = string.IsNullOrWhiteSpace(txtToAfter.Text) ? 0 : Convert.ToDecimal(txtToAfter.Text), 
                    status = 1,
                    //Nota
                    //status == 0 ? "Resubmit" :
                    //status == 1 ? "Submitted" :
                    //status == 2 ? "Under Review" :
                    //status == 3 ? "Completed" :

                    CreatedBy = Auth.Id(),  
                    CreatedDate = DateTime.Now
                };

                db.TransfersTransaction.Add(model);
                db.SaveChanges();

                if (fuDocument.HasFile)
                {
                    using (var binaryReader = new System.IO.BinaryReader(fuDocument.PostedFile.InputStream))
                    {
                        byte[] fileData = binaryReader.ReadBytes(fuDocument.PostedFile.ContentLength);

                        var document = new TransferDocument
                        {
                            Id = Guid.NewGuid(),
                            TransferId = newId,
                            FileName = fuDocument.FileName,
                            ContentType = fuDocument.PostedFile.ContentType,
                            FileData = fileData,
                            UploadedBy = Auth.Id(),
                            UploadedDate = DateTime.Now
                        };

                        db.TransferDocuments.Add(document);
                        db.SaveChanges();
                    }
                }
                Emails.EmailsReqTransferBudgetForNewRequest(newId, model, Auth.User().iPMSRoleCode);

                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Transfer Budget added.");
                Response.Redirect("~/Budget/Transfer");
            }
        }

        private void BindBALabel()
        {
            string ba = Auth.User().iPMSBizAreaCode;

            bool isEmptyBA = string.IsNullOrWhiteSpace(ba);

            phBA.Visible = !isEmptyBA;
            phBADropdown.Visible = isEmptyBA;

            if (!isEmptyBA) 
            {
                LblBA.Text = 
                    lblToBA.Text = ba;
                LblBAName.Text = new Class.IPMSBizArea().GetNameByCode(ba) ?? "";
            }
        }

        private void BindControl()
        {
            BindDropdown(ddFromBA, new Class.IPMSBizArea().GetIPMSBizAreas(), "Code", "DisplayName");
            BindDropdown(txtFromGL, Functions.GetBudgetTypes(), "ID", "DisplayName");
            BindDropdown(txtToGL, Functions.GetBudgetTypes(), "ID", "DisplayName");
        }

        /// <summary>
        /// Binds a dropdown list to a datasource and inserts an empty item at the top.
        /// </summary>
        private void BindDropdown(ListControl ddl, object dataSource, string dataValueField, string dataTextField)
        {
            ddl.DataSource = dataSource;
            ddl.DataValueField = dataValueField;
            ddl.DataTextField = dataTextField;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("", ""));
        }


        protected void From_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateBudgetSum(txtFromGL.SelectedValue, ddFromBA.SelectedValue, txtFromBudget, txtFromBalance);
            RecalculateTotals();
        }

        protected void To_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateBudgetSum(txtToGL.SelectedValue, lblToBA.Text?.Trim(), txtToBudget, txtToBalance);
            RecalculateTotals();
        }

        /// <summary>
        /// Calculates and updates the sum amount into the target TextBox.
        /// </summary>
        private void CalculateBudgetSum(string budgetTypeCode, string bizAreaCode, TextBox targetTextBox, TextBox BalanceTextBox)
        {
            if (string.IsNullOrWhiteSpace(budgetTypeCode) || string.IsNullOrWhiteSpace(bizAreaCode))
            {
                targetTextBox.Text = "0.00";
                return;
            }
            var typeId = Guid.Parse(budgetTypeCode); // or get dynamically
            var total = GetTotalBalance(typeId, bizAreaCode);
            BalanceTextBox.Text = total.ToString("N2");

            using (var db = new AppDbContext())
            {
                int currentYear = DateTime.Now.Year;

                var sumAmount = (from b in db.Budgets
                                 join bt in db.BudgetTypes on b.TypeId equals bt.Id
                                 where bt.Id == typeId
                                       && b.BizAreaCode == bizAreaCode
                                       && b.DeletedDate == null
                                       && SqlFunctions.DatePart("year", b.Date) == currentYear
                                 select (decimal?)b.Amount).Sum() ?? 0;

                targetTextBox.Text = sumAmount.ToString("N2");
            }
        }
        private decimal GetTotalBalance(Guid typeId, string bizAreaCode)
        {
            int currentYear = DateTime.Now.Year;

            using (var db = new AppDbContext())
            {
                var totalBalance = db.Database.SqlQuery<decimal?>(@"
                    WITH BudgetBalances AS (
                        SELECT 
                            b.Id,
                            b.Amount 
                                + ISNULL(SUM(
                                    CASE 
                                        WHEN b.Id = t.ToId THEN t.Amount
                                        WHEN b.Id = t.FromId THEN -t.Amount
                                        ELSE 0
                                    END
                                ), 0) AS Balance
                        FROM Budgets b
                        LEFT JOIN Transactions t ON (b.Id = t.FromId OR b.Id = t.ToId)
                        WHERE b.TypeId = @p0
                          AND b.BizAreaCode = @p2
                          AND YEAR(b.Date) = @p1
                          AND b.DeletedDate IS NULL
                          AND (t.DeletedDate IS NULL OR t.DeletedDate IS NULL) -- handles left join null
                        GROUP BY b.Id, b.Amount
                    )
                    SELECT SUM(Balance) AS TotalBalance
                    FROM BudgetBalances;
                ", typeId, currentYear, bizAreaCode).FirstOrDefault() ?? 0m;

                return totalBalance;
            }
        }


        private void RecalculateTotals()
        {
            decimal fromBalance = decimal.TryParse(txtFromBalance.Text, out var fb) ? fb : 0.0m;
            decimal fromTransfer = decimal.TryParse(txtFromTransfer.Text, out var ft) ? ft : 0.0m;
            decimal toBalance = decimal.TryParse(txtToBalance.Text, out var tb) ? tb : 0.0m;

            decimal fromAfter = fromBalance - fromTransfer;
            decimal toTransfer = fromTransfer;  // Same amount
            decimal toAfter = toBalance + fromTransfer;

            txtFromAfter.Text = fromAfter.ToString("N2");
            txtToTransfer.Text = toTransfer.ToString("N2");
            txtToAfter.Text = toAfter.ToString("N2");
        }

    }
}