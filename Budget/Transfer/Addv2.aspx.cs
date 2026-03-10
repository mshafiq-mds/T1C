using FGV.Prodata.Web.UI;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.Auth;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.Transfer
{
    // 1. Data Model for Repeater Rows
    [Serializable]
    public class FromBudgetModel
    {
        public string GL { get; set; } = "";
        public string BudgetTypeID { get; set; } = "";
        public decimal OriginalBudget { get; set; } = 0;
        public decimal CurrentBalance { get; set; } = 0;
        public decimal TransferAmount { get; set; } = 0;
        public decimal BalanceAfter => CurrentBalance - TransferAmount;
    }

    public partial class Addv2 : ProdataPage
    {
        // 2. ViewState property to remember dynamically added rows across PostBacks
        private List<FromBudgetModel> FromBudgetList
        {
            get { return ViewState["FromBudgetList"] as List<FromBudgetModel> ?? new List<FromBudgetModel>(); }
            set { ViewState["FromBudgetList"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControl();
                BindBALabel();

                txtEVisa.Text = txtRefNo.Text = Functions.GetGeneratedRefNo("PB", true);
                txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");

                // 🚀 NEW: Inject readonly attribute from backend
                txtRefNo.Attributes.Add("readonly", "readonly");
                txtEVisa.Attributes.Add("readonly", "readonly");
                txtDate.Attributes.Add("readonly", "readonly");
                txtEstimatedCost.Attributes.Add("readonly", "readonly");

                txtToBudget.Attributes.Add("readonly", "readonly");
                txtToBalance.Attributes.Add("readonly", "readonly");
                txtToTransfer.Attributes.Add("readonly", "readonly");
                txtToAfter.Attributes.Add("readonly", "readonly");

                FromBudgetList = new List<FromBudgetModel> { new FromBudgetModel() };
                BindRepeater();
            }

            ScriptManager.GetCurrent(this.Page)?.RegisterAsyncPostBackControl(txtToBudgetType);
        }

        // --- Core UI Binding Methods ---

        private void BindBALabel()
        {
            string ba = Auth.User().CCMSBizAreaCode;
            bool isEmptyBA = string.IsNullOrWhiteSpace(ba);

            phBA.Visible = !isEmptyBA;
            phBADropdown.Visible = isEmptyBA;

            if (!isEmptyBA)
            {
                string baName = new Class.IPMSBizArea().GetNameByCode(ba) ?? "";
                LblBA.Text = ba;
                LblBAName.Text = baName;

                // Populate the Destination BA label above the table
                lblGlobalToBA.Text = $"{ba} - {baName}";
            }
        }

        private void BindControl()
        {
            // Bind static 'To' Budget Type Dropdown
            BindDropdown(ddlGlobalFromBA, new Class.IPMSBizArea().GetIPMSBizAreas(), "Code", "DisplayName");

            // Bind static TO Budget Type
            BindDropdown(txtToBudgetType, Functions.GetBudgetTypes(), "ID", "DisplayName");
        }

        private void BindDropdown(ListControl ddl, object dataSource, string dataValueField, string dataTextField)
        {
            ddl.DataSource = dataSource;
            ddl.DataValueField = dataValueField;
            ddl.DataTextField = dataTextField;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("", ""));
        }

        private void BindRepeater()
        {
            rptFromBudgets.DataSource = FromBudgetList;
            rptFromBudgets.DataBind();
            RecalculateToBudgetTotals();
        }

        // --- Repeater Events (The Dynamic Rows) ---

        protected void rptFromBudgets_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var row = (FromBudgetModel)e.Item.DataItem;

                var ddlType = (DropDownList)e.Item.FindControl("ddlFromBudgetType");
                BindDropdown(ddlType, Functions.GetBudgetTypes(), "ID", "DisplayName");
                ddlType.SelectedValue = row.BudgetTypeID;

                // 🚀 NEW: Find the textboxes in this specific row and lock them
                var txtFromBudget = (TextBox)e.Item.FindControl("txtFromBudget");
                var txtFromBalance = (TextBox)e.Item.FindControl("txtFromBalance");
                var txtFromAfter = (TextBox)e.Item.FindControl("txtFromAfter");

                txtFromBudget?.Attributes.Add("readonly", "readonly");
                txtFromBalance?.Attributes.Add("readonly", "readonly");
                txtFromAfter?.Attributes.Add("readonly", "readonly");

                ScriptManager.GetCurrent(this.Page)?.RegisterAsyncPostBackControl(ddlType);

                var btnRemoveRow = (LinkButton)e.Item.FindControl("btnRemoveRow");
                if (btnRemoveRow != null)
                {
                    ScriptManager.GetCurrent(this.Page)?.RegisterAsyncPostBackControl(btnRemoveRow);
                }
            }
        }

        protected void rptFromBudgets_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                SyncRepeaterToList(); // Save user inputs first
                int index = Convert.ToInt32(e.CommandArgument);
                var list = FromBudgetList;

                // Ensure the user doesn't delete the very last row
                if (list.Count > 1)
                {
                    list.RemoveAt(index);
                    FromBudgetList = list;
                    BindRepeater();
                }
                else
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "You must have at least one 'From Budget' row.");
                }
            }
        }

        protected void btnAddRow_Click(object sender, EventArgs e)
        {
            SyncRepeaterToList(); // Always sync before manipulating the list
            var list = FromBudgetList;
            list.Add(new FromBudgetModel());
            FromBudgetList = list;
            BindRepeater();
        }

        // This method scrapes the screen to save user-typed text before a postback wipes it
        private void SyncRepeaterToList()
        {
            var list = FromBudgetList;
            for (int i = 0; i < rptFromBudgets.Items.Count; i++)
            {
                var item = rptFromBudgets.Items[i];
                list[i].GL = ((TextBox)item.FindControl("txtFromGLCode")).Text;
                list[i].BudgetTypeID = ((DropDownList)item.FindControl("ddlFromBudgetType")).SelectedValue;

                decimal transferAmount;
                decimal.TryParse(((TextBox)item.FindControl("txtFromTransfer")).Text, out transferAmount);
                list[i].TransferAmount = transferAmount;
            }
            FromBudgetList = list;
        }

        // --- Dropdown Logic & Calculations ---

        protected void From_SelectedIndexChanged(object sender, EventArgs e)
        {
            SyncRepeaterToList();

            DropDownList ddl = (DropDownList)sender;
            RepeaterItem item = (RepeaterItem)ddl.NamingContainer;
            int index = item.ItemIndex;

            var row = FromBudgetList[index];
            string globalBaCode = ddlGlobalFromBA.SelectedValue;

            if (!string.IsNullOrEmpty(row.BudgetTypeID) && !string.IsNullOrEmpty(globalBaCode))
            {
                Guid typeId = Guid.Parse(row.BudgetTypeID);
                row.CurrentBalance = GetTotalBalance(typeId, globalBaCode);
                row.OriginalBudget = GetOriginalBudget(typeId, globalBaCode);
            }
            else
            {
                row.CurrentBalance = 0;
                row.OriginalBudget = 0;
            }

            BindRepeater();
        }

        protected void GlobalFromBA_SelectedIndexChanged(object sender, EventArgs e)
        {
            SyncRepeaterToList();
            string newBaCode = ddlGlobalFromBA.SelectedValue;

            // Recalculate balances for ALL rows based on the new BA
            foreach (var row in FromBudgetList)
            {
                if (!string.IsNullOrEmpty(row.BudgetTypeID) && !string.IsNullOrEmpty(newBaCode))
                {
                    Guid typeId = Guid.Parse(row.BudgetTypeID);
                    row.CurrentBalance = GetTotalBalance(typeId, newBaCode);
                    row.OriginalBudget = GetOriginalBudget(typeId, newBaCode);
                }
                else
                {
                    row.CurrentBalance = 0;
                    row.OriginalBudget = 0;
                }
            }

            BindRepeater();
        }

        protected void To_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateBudgetSum(txtToBudgetType.SelectedValue, LblBA.Text?.Trim(), txtToBudget, txtToBalance);
            RecalculateToBudgetTotals();
        }

        private void CalculateBudgetSum(string budgetTypeCode, string bizAreaCode, TextBox targetTextBox, TextBox BalanceTextBox)
        {
            if (string.IsNullOrWhiteSpace(budgetTypeCode) || string.IsNullOrWhiteSpace(bizAreaCode))
            {
                targetTextBox.Text = "0.00";
                BalanceTextBox.Text = "0.00";
                return;
            }

            var typeId = Guid.Parse(budgetTypeCode);

            // Set Current Balance
            var totalBalance = GetTotalBalance(typeId, bizAreaCode);
            BalanceTextBox.Text = totalBalance.ToString("N2");

            // Set Original Budget
            var originalBudget = GetOriginalBudget(typeId, bizAreaCode);
            targetTextBox.Text = originalBudget.ToString("N2");
        }

        private void RecalculateToBudgetTotals()
        {
            SyncRepeaterToList();

            // Sum all transfers from the Repeater
            decimal totalTransfer = FromBudgetList.Sum(x => x.TransferAmount);

            // Calculate To After
            decimal toBalance = decimal.TryParse(txtToBalance.Text, out var tb) ? tb : 0.0m;
            decimal toAfter = toBalance + totalTransfer;

            // Apply to UI
            txtToTransfer.Text = totalTransfer.ToString("N2");
            txtToAfter.Text = toAfter.ToString("N2");
            txtEstimatedCost.Text = totalTransfer.ToString("N2");
        }

        // --- Database Queries ---

        private decimal GetOriginalBudget(Guid typeId, string bizAreaCode)
        {
            int currentYear = DateTime.Now.Year;

            using (var db = new AppDbContext())
            {
                var sumAmount = (from b in db.Budgets
                                 join bt in db.BudgetTypes on b.TypeId equals bt.Id
                                 where bt.Id == typeId
                                       && b.BizAreaCode == bizAreaCode
                                       && b.DeletedDate == null
                                       && SqlFunctions.DatePart("year", b.Date) == currentYear
                                 select (decimal?)b.Amount).Sum() ?? 0;

                return sumAmount;
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
                          AND (t.DeletedDate IS NULL) 
                        GROUP BY b.Id, b.Amount
                    )
                    SELECT SUM(Balance) AS TotalBalance
                    FROM BudgetBalances;
                ", typeId, currentYear, bizAreaCode).FirstOrDefault() ?? 0m;

                return totalBalance;
            }
        }

        protected void btnSubmit_Click1(object sender, EventArgs e)
        {
            // 1. Ensure we have the latest data from the dynamic Repeater rows
            SyncRepeaterToList();

            // 2. Validate that they actually added transfer amounts
            if (FromBudgetList == null || FromBudgetList.Count == 0 || FromBudgetList.Sum(x => x.TransferAmount) <= 0)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Please add at least one valid transfer amount.");
                return;
            }

            Guid newId;

            using (var db = new AppDbContext())
            {
                // Generate unique ID
                do
                {
                    newId = Guid.NewGuid();
                } while (db.TransfersTransaction.Any(x => x.Id == newId));

                string refNo = Functions.GetGeneratedRefNo("PB", false);

                if (txtRefNo.Text != refNo)
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Ref No. " + txtRefNo.Text + " already exists. Updated to new Ref No: " + refNo + ".");

                // ==========================================
                // STEP 1: SAVE MASTER RECORD
                // ==========================================
                var masterModel = new TransfersTransaction
                {
                    Id = newId,
                    BA = Auth.User().CCMSBizAreaCode,
                    RefNo = refNo,
                    Project = txtProject.Text.Trim(),
                    Date = string.IsNullOrWhiteSpace(txtDate.Text) ? DateTime.Today : DateTime.Parse(txtDate.Text),
                    BudgetType = rdoOpex.Checked ? "OPEX" : "CAPEX",
                    EstimatedCost = FromBudgetList.Sum(x => x.TransferAmount), // Auto-calculated total
                    Justification = txtJustification.Text.Trim(),
                    EVisaNo = refNo,
                    WorkDetails = txtWorkDetails.Text.Trim(),

                    // The Global Source BA
                    FromBA = ddlGlobalFromBA.SelectedValue,

                    // Single Destination (TO)
                    ToBudgetType = Guid.TryParse(txtToBudgetType.SelectedValue, out var ToBudgetTypeGuid) ? ToBudgetTypeGuid : Guid.Empty,
                    ToBA = Auth.User().CCMSBizAreaCode, // Destination is always the user's BA
                    ToBudget = string.IsNullOrWhiteSpace(txtToBudget.Text) ? 0 : Convert.ToDecimal(txtToBudget.Text),
                    ToBalance = string.IsNullOrWhiteSpace(txtToBalance.Text) ? 0 : Convert.ToDecimal(txtToBalance.Text),
                    ToTransfer = string.IsNullOrWhiteSpace(txtToTransfer.Text) ? 0 : Convert.ToDecimal(txtToTransfer.Text),
                    ToAfter = string.IsNullOrWhiteSpace(txtToAfter.Text) ? 0 : Convert.ToDecimal(txtToAfter.Text),
                    ToGL = txtToGLCode.Text.Trim(),

                    status = "Submitted",
                    CreatedBy = Auth.Id(),
                    CreatedDate = DateTime.Now
                };

                db.TransfersTransaction.Add(masterModel);

                // ==========================================
                // STEP 2: SAVE MULTIPLE DETAILS (FROM ROWS)
                // ==========================================
                foreach (var fromItem in FromBudgetList)
                {
                    // Only save rows that have actual data
                    if (!string.IsNullOrEmpty(fromItem.GL) && fromItem.TransferAmount > 0)
                    {
                        var detailModel = new TransfersTransactionDetail
                        {
                            Id = Guid.NewGuid(),
                            TransferId = newId,
                            FromGL = fromItem.GL,
                            FromBudgetType = Guid.TryParse(fromItem.BudgetTypeID, out var fromTypeGuid) ? fromTypeGuid : (Guid?)null,
                            FromBudget = fromItem.OriginalBudget,
                            FromBalance = fromItem.CurrentBalance,
                            FromTransfer = fromItem.TransferAmount,
                            FromAfter = fromItem.BalanceAfter
                        };

                        db.TransfersTransactionDetails.Add(detailModel);
                    }
                }

                // ==========================================
                // STEP 3: LOG & DOCUMENT
                // ==========================================
                var log = new TransferApprovalLog
                {
                    BudgetTransferId = newId,
                    StepNumber = 100,
                    RoleName = Auth.User().CCMSRoleCode,
                    UserId = Auth.User().Id,
                    ActionType = "Submitted",
                    ActionDate = DateTime.Now,
                    Status = "UnderReview"
                };
                db.TransferApprovalLog.Add(log);

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
                    }
                }

                db.SaveChanges();

                Emails.EmailsReqTransferBudgetForNewRequest(newId, masterModel);

                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Transfer Budget added successfully.");
                Response.Redirect("~/Budget/Transfer");
            }
        }
    }
}