using CustomGuid.AspNet.Identity;
using FGV.Prodata.Web.UI;
using Newtonsoft.Json;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace Prodata.WebForm.Budget.Transfer.TransferApplication
{
    public partial class Application : ProdataPage
    {
        private Guid _transferId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string idStr = Request.QueryString["id"];
                if (Guid.TryParse(idStr, out _transferId))
                {
                    if (LoadTransferHeader(_transferId))
                    {
                        LoadBudgetDropdownData(_transferId);
                    }
                }
                else
                {
                    Response.Redirect("~/Budget/Transfer/TransferApplication/Default");
                }
            }
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            // 1. Parse ID locally (Robustness)
            if (!Guid.TryParse(Request.QueryString["id"], out Guid transferId))
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Invalid Transfer ID.");
                return;
            }

            // 2. Validate Remarks
            string remarks = hdnRemarks.Value?.Trim();
            if (string.IsNullOrEmpty(remarks))
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Warning, "Remarks are required for rejection.");
                return;
            }

            using (var db = new AppDbContext())
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var transferRecord = db.TransfersTransaction.FirstOrDefault(x => x.Id == transferId);
                    if (transferRecord == null) throw new Exception("Transfer record not found.");

                    var currentUser = Auth.User();

                    // 3. Log the Rejection
                    var log = new TransferApprovalLog
                    {
                        BudgetTransferId = transferId,
                        StepNumber = 100, // Adjust logic if dynamic steps are needed
                        RoleName = currentUser.CCMSRoleCode,
                        UserId = currentUser.Id,
                        ActionType = "Reject",
                        ActionDate = DateTime.Now,
                        Status = "Reject",
                        Remarks = remarks
                    };
                    db.TransferApprovalLog.Add(log);

                    transferRecord.DeletedDate = DateTime.Now;
                    transferRecord.DeletedBy = currentUser.Id;
                    transferRecord.status = "Rejected";

                    // 5. DELETE TRANSACTIONS DATA
                    if (transferRecord.NewBudgetId.HasValue)
                    {
                        var linkedTransactions = db.Transactions
                            .Where(t => t.ToId == transferRecord.NewBudgetId.Value)
                            .ToList();

                        if (linkedTransactions.Any())
                        {
                            //db.Transactions.RemoveRange(linkedTransactions);
                            // Option B: Soft Delete (If your Transactions table has these columns)
                            foreach(var trans in linkedTransactions) {
                                trans.DeletedDate = DateTime.Now;
                                trans.DeletedBy = currentUser.Id;
                            } 
                        }
                    }

                    db.SaveChanges();

                    Emails.EmailsReqTransferBudgetForApprover(id: transferId, TT: transferRecord, reject: true);

                    // CRITICAL FIX: Commit the transaction to persist changes
                    transaction.Commit();

                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Application rejected successfully.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error: " + ex.Message);
                }
            }

            Response.Redirect("~/Budget/Transfer/TransferApplication/Default");
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Guid.TryParse(Request.QueryString["id"], out _transferId))
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Invalid Transfer ID.");
                return;
            }

            // Retrieve remarks from the HiddenField populated by SweetAlert
            string remarks = hdnRemarks.Value;
            if (string.IsNullOrWhiteSpace(remarks))
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Warning, "Remarks are required.");
                return;
            }

            using (var db = new AppDbContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var transferRecord = db.TransfersTransaction.FirstOrDefault(x => x.Id == _transferId);
                        if (transferRecord == null) throw new Exception("Transfer record not found.");

                        // 1. Create Destination Budget (The "To" bucket)
                        var newBudget = new Prodata.WebForm.Models.Budget
                        {
                            Id = Guid.NewGuid(),
                            TypeId = transferRecord.ToBudgetType,
                            BizAreaCode = transferRecord.ToBA,
                            BizAreaName = new Class.IPMSBizArea().GetNameByCode(transferRecord.ToBA) ?? "-",
                            Date = DateTime.Now,
                            Month = DateTime.Now.Month,
                            Num = (db.Budgets.Where(b => b.BizAreaCode == transferRecord.ToBA).Max(x => (int?)x.Num) ?? 0) + 1,
                            Ref = transferRecord.RefNo,
                            Details = $"TRANSFER FROM {transferRecord.FromBA}: {transferRecord.Project}",
                            Amount = 0, // Initial amount is 0, balance comes from transactions
                            Vendor = "INTERNAL TRANSFER",
                            CreatedBy = Auth.User().Id,
                            CreatedDate = DateTime.Now,
                            DeletedBy = Auth.User().Id,
                            DeletedDate = DateTime.Now
                        };
                        db.Budgets.Add(newBudget);
                        db.SaveChanges();

                        transferRecord.NewBudgetId = newBudget.Id;
                        // 2. Loop through dynamic rows from Request.Form to create Transactions
                        int index = 0;
                        decimal totalAllocated = 0;

                        while (true)
                        {
                            // Keys matching the HTML input names generated in ASPX JavaScript
                            string idKey = $"allocations[{index}][BudgetId]";
                            string amountKey = $"allocations[{index}][Amount]";
                            string refKeyName = $"allocations[{index}][BudgetRef]";

                            // Break loop if no more rows found
                            if (string.IsNullOrEmpty(Request.Form[idKey])) break;

                            Guid budgetSourceId = Guid.Parse(Request.Form[idKey]);
                            string budgetSourceRefName = Request.Form[refKeyName];
                            decimal amount = 0;
                            decimal.TryParse(Request.Form[amountKey], out amount);

                            //if (amount > 0)
                            //{
                            var txn = new Transaction
                            {
                                Id = Guid.NewGuid(),
                                FromId = budgetSourceId,    // Taking money FROM Source Budget
                                FromType = "Budget",
                                ToId = newBudget.Id,        // Giving money TO New Destination Budget
                                ToType = "Budget",
                                Amount = amount,
                                Date = DateTime.Now,
                                Name = remarks.Trim(),      // Use remarks from HiddenField
                                Ref = transferRecord.RefNo,      // Reference the source
                                CreatedBy = Auth.User().Id,
                                CreatedDate = DateTime.Now
                            };
                            db.Transactions.Add(txn);
                            totalAllocated += amount;
                            //}
                            index++;
                        }

                        // 3. Final Validation: Total Allocated must match Requested Transfer Amount
                        //decimal requestedAmt = transferRecord.FromTransfer ?? 0;
                        //if (Math.Abs(totalAllocated - requestedAmt) > 0.01m)
                        //{
                        //    throw new Exception($"Allocation Mismatch. Requested: {requestedAmt}, Allocated: {totalAllocated}");
                        //}

                        // 4. Update Transfer Transaction Status to "Finalized" (String)
                        transferRecord.status = "UnderReview";
                        transferRecord.UpdatedBy = Auth.User().Id;
                        transferRecord.UpdatedDate = DateTime.Now;

                        // 5. Log Action
                        var log = new TransferApprovalLog
                        {
                            BudgetTransferId = _transferId,
                            StepNumber = 0,
                            RoleName = Auth.User().CCMSRoleCode,
                            UserId = Auth.User().Id,
                            ActionType = "Submitted",
                            ActionDate = DateTime.Now,
                            Status = "UnderReview",
                            Remarks = remarks.Trim()
                        };
                        db.TransferApprovalLog.Add(log);

                        db.SaveChanges();
                        transaction.Commit();

                        Emails.EmailsReqTransferBudgetForFirstApprover(_transferId, transferRecord);

                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Transfer Application successfully.");
                    }
                    catch (Exception ex)
                    { 
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error: " + ex.Message);
                    }
                }
            }
            Response.Redirect("~/Budget/Transfer/TransferApplication/Default");

        }

        #region Helper Methods (LoadHeader, LoadDropdown)

        private bool LoadTransferHeader(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var transfer = db.TransfersTransaction.FirstOrDefault(x => x.Id == id);
                if (transfer == null)
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Transfer record not found.");
                    Response.Redirect("~/Budget/Transfer/TransferApplication");
                    return false;
                }

                lblRef.Text = transfer.RefNo;
                lblDate.Text = transfer.Date.ToString("dd/MM/yyyy");
                lblApplicantName.Text = db.Users.Where(u => u.Id == transfer.CreatedBy).Select(u => u.Name).FirstOrDefault() ?? "Unknown";
                lblBA.Text = transfer.FromBA;
                LblBAName.Text = new Class.IPMSBizArea().GetNameByCode(transfer.FromBA) ?? "-";

                decimal reqAmount = transfer.FromTransfer ?? 0;
                lblAmount.Text = $"RM {reqAmount:N2}";
                lblReason.Text = transfer.Justification;

                hdnTargetAmount.Value = reqAmount.ToString("0.00");

                string fromType = db.BudgetTypes.Where(u => u.Id == transfer.FromBudgetType).Select(u => u.Name).FirstOrDefault();
                string toType = db.BudgetTypes.Where(u => u.Id == transfer.ToBudgetType).Select(u => u.Name).FirstOrDefault();
                string toBAName = new Class.IPMSBizArea().GetNameByCode(transfer.ToBA) ?? transfer.ToBA;
                string FromBAName = new Class.IPMSBizArea().GetNameByCode(transfer.FromBA) ?? transfer.FromBA;

                lblTransferInfo.Text = $"<strong>{transfer.FromBA}-{FromBAName}</strong> ({fromType}) <i class='fas fa-arrow-right mx-1'></i> <strong>{transfer.ToBA}-{toBAName}</strong> ({toType})";

                // Status Display Logic
                string badgeClass = "badge bg-secondary";
                string statusText = transfer.status ?? "Unknown";

                if (transfer.DeletedDate != null) { statusText = "Deleted"; badgeClass = "badge bg-danger"; }
                else if (statusText == "sentback") { statusText = "Resubmit"; badgeClass = "badge bg-warning text-dark"; }
                else if (statusText == "Submitted") { badgeClass = "badge bg-info text-dark"; }
                else if (statusText == "UnderReview") { statusText = "Under Review"; badgeClass = "badge bg-primary"; }
                else if (statusText == "Completed") { badgeClass = "badge bg-success"; }
                else if (statusText == "Finalized") { badgeClass = "badge bg-dark"; }

                litStatusHtml.Text = $"<span class='{badgeClass} badge-status rounded-pill'>{statusText}</span>";

                return true;
            }
        }

        private void LoadBudgetDropdownData(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var transfer = db.TransfersTransaction.FirstOrDefault(x => x.Id == id);
                Guid typeid = transfer.FromBudgetType;
                var baCode = Auth.User().CCMSBizAreaCode; // Or transfer.FromBA if user can finalize for others
                int year = transfer.Date.Year;

                string sql = @"
                    SELECT 
                        b.Id, b.Ref, b.Details,
                        (b.Amount + 
                         ISNULL((SELECT SUM(ISNULL(ti.Amount, 0)) FROM Transactions ti WHERE ti.ToId = b.Id AND ti.ToType = 'Budget' AND ti.DeletedDate IS NULL), 0) - 
                         ISNULL((SELECT SUM(ISNULL(to_t.Amount, 0)) FROM Transactions to_t WHERE to_t.FromId = b.Id AND to_t.FromType = 'Budget' AND to_t.DeletedDate IS NULL), 0)
                        ) AS Balance
                    FROM Budgets b
                    WHERE b.BizAreaCode = @p0 
                      AND b.DeletedDate IS NULL 
                      AND YEAR(b.Date) = @p1
                      AND TypeId = @p2
                ";

                var rawData = db.Database.SqlQuery<RawBudgetDTO>(sql, transfer.FromBA, year, typeid).ToList();

                var dropdownList = rawData
                    .Where(x => x.Balance > 0)
                    .Select(x => new
                    {
                        Id = x.Id,
                        Ref = x.Ref,
                        Balance = x.Balance,
                        Display = $"{x.Ref} - {x.Details} (Bal: RM {x.Balance:N2})"
                    })
                    .OrderBy(x => x.Display)
                    .ToList();

                hdnBudgetJson.Value = JsonConvert.SerializeObject(dropdownList);
            }
        }

        private class RawBudgetDTO
        {
            public Guid Id { get; set; }
            public string Ref { get; set; }
            public string Details { get; set; }
            public decimal Balance { get; set; }
        }

        #endregion
    }
}