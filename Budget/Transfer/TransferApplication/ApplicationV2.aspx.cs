using CustomGuid.AspNet.Identity;
using FGV.Prodata.Web.UI;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Web.UI;

namespace Prodata.WebForm.Budget.Transfer.TransferApplication
{
    public partial class ApplicationV2 : ProdataPage
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

                // Header Info
                lblRef.Text = transfer.RefNo;
                lblDate.Text = transfer.Date.ToString("dd-MMM-yyyy");
                lblApplicantName.Text = db.Users.Where(u => u.Id == transfer.CreatedBy).Select(u => u.Name).FirstOrDefault() ?? "Unknown";

                var ipmsBA = new Class.IPMSBizArea();
                lblFromBA.Text = $"{transfer.FromBA} - {ipmsBA.GetNameByCode(transfer.FromBA)}";
                lblToBA.Text = $"{transfer.ToBA} - {ipmsBA.GetNameByCode(transfer.ToBA)}";

                lblReason.Text = transfer.Project;

                // Determine Requested Amount (From V2 Details Table)
                decimal reqAmount = transfer.EstimatedCost;
                lblAmount.Text = $"RM {reqAmount:N2}";
                hdnTargetAmount.Value = reqAmount.ToString("0.00");

                return true;
            }
        }

        private void LoadBudgetDropdownData(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var transfer = db.TransfersTransaction.FirstOrDefault(x => x.Id == id);
                if (transfer == null) return;

                string sourceBaCode = transfer.FromBA;
                int year = transfer.Date.Year;

                // 1. Gather all requested "From" items specifically requested by the user
                var requestedDetails = db.TransfersTransactionDetails
                    .Where(x => x.TransferId == id && x.FromTransfer > 0)
                    .Select(x => new
                    {
                        TypeId = x.FromBudgetType,
                        Amount = x.FromTransfer,
                        // Grab a display name for the budget type to show in the UI
                        TypeName = db.Budgets.Where(b => b.TypeId == x.FromBudgetType).Select(b => b.Details).FirstOrDefault() ?? "Requested Budget Type"
                    }).ToList();

                // Send the requested details to the frontend to Auto-Fill the UI
                hdnRequestedAllocationsJson.Value = JsonConvert.SerializeObject(requestedDetails);

                // 2. Fetch all available Budgets for this BA to populate the dropdowns
                string sql = @"
                    SELECT 
                        b.Id, b.Ref, b.Details, b.TypeId,
                        (b.Amount + 
                         ISNULL((SELECT SUM(ISNULL(ti.Amount, 0)) FROM Transactions ti WHERE ti.ToId = b.Id AND ti.ToType = 'Budget' AND ti.DeletedDate IS NULL), 0) - 
                         ISNULL((SELECT SUM(ISNULL(to_t.Amount, 0)) FROM Transactions to_t WHERE to_t.FromId = b.Id AND to_t.FromType = 'Budget' AND to_t.DeletedDate IS NULL), 0)
                        ) AS Balance
                    FROM Budgets b
                    WHERE b.BizAreaCode = @p0 
                      AND b.DeletedDate IS NULL 
                      AND YEAR(b.Date) = @p1
                ";

                var rawData = db.Database.SqlQuery<RawBudgetDTO>(sql, sourceBaCode, year).ToList();

                // Build the dropdown object, including the TypeId so the JS can match it up
                var dropdownList = rawData
                    .Where(x => x.Balance > 0)
                    .Select(x => new
                    {
                        Id = x.Id,
                        Ref = x.Ref,
                        TypeId = x.TypeId, // Required for matching against requested items
                        Balance = x.Balance,
                        Display = $"{x.Ref} (Bal: RM {x.Balance:N2})"
                    })
                    .OrderBy(x => x.Display)
                    .ToList();

                hdnBudgetJson.Value = JsonConvert.SerializeObject(dropdownList);
            }
        }

        private class RawBudgetDTO
        {
            public Guid Id { get; set; }
            public Guid? TypeId { get; set; }
            public string Ref { get; set; }
            public string Details { get; set; }
            public decimal Balance { get; set; }
        }

        #endregion

        #region Actions (Reject / Submit)

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!Guid.TryParse(Request.QueryString["id"], out Guid transferId)) return;

            string remarks = hdnRemarks.Value?.Trim();
            if (string.IsNullOrEmpty(remarks)) return;

            using (var db = new AppDbContext())
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var transferRecord = db.TransfersTransaction.FirstOrDefault(x => x.Id == transferId);
                    if (transferRecord == null) throw new Exception("Transfer record not found.");

                    var currentUser = Auth.User();

                    // Log the Rejection
                    var log = new TransferApprovalLog
                    {
                        BudgetTransferId = transferId,
                        StepNumber = -1, // Indicates rejection end state
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

                    // Reverse Transactions if any were made previously (Safety net)
                    if (transferRecord.NewBudgetId.HasValue)
                    {
                        var linkedTransactions = db.Transactions
                            .Where(t => t.ToId == transferRecord.NewBudgetId.Value)
                            .ToList();

                        if (linkedTransactions.Any())
                        {
                            foreach (var trans in linkedTransactions)
                            {
                                trans.DeletedDate = DateTime.Now;
                                trans.DeletedBy = currentUser.Id;
                            }
                        }
                    }

                    db.SaveChanges();
                    Emails.EmailsReqTransferBudgetForApprover(id: transferId, TT: transferRecord, reject: true);
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
            if (!Guid.TryParse(Request.QueryString["id"], out _transferId)) return;
            string remarks = hdnRemarks.Value;

            using (var db = new AppDbContext())
            {
                var transferRecordCheck = db.TransfersTransaction.FirstOrDefault(x => x.Id == _transferId);

                if (transferRecordCheck == null || transferRecordCheck.status != "Submitted")
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Transfer record not found or has already been processed.");
                    Response.Redirect("~/Budget/Transfer/TransferApplication/Default");
                    return;
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var transferRecord = db.TransfersTransaction.FirstOrDefault(x => x.Id == _transferId);

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
                            Amount = 0,
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
                            string idKey = $"allocations[{index}][BudgetId]";
                            string amountKey = $"allocations[{index}][Amount]";

                            if (string.IsNullOrEmpty(Request.Form[idKey])) break;

                            Guid budgetSourceId = Guid.Parse(Request.Form[idKey]);
                            decimal.TryParse(Request.Form[amountKey], out decimal amount);

                            if (amount > 0)
                            {
                                var txn = new Transaction
                                {
                                    Id = Guid.NewGuid(),
                                    FromId = budgetSourceId,
                                    FromType = "Budget",
                                    ToId = newBudget.Id,
                                    ToType = "Budget",
                                    Amount = amount,
                                    Date = DateTime.Now,
                                    Name = remarks.Trim(),
                                    Ref = transferRecord.RefNo,
                                    CreatedBy = Auth.User().Id,
                                    CreatedDate = DateTime.Now
                                };
                                db.Transactions.Add(txn);
                                totalAllocated += amount;
                            }
                            index++;
                        }

                        // 3. Update Transfer Transaction Status to "Completed"
                        transferRecord.status = "UnderReview";
                        transferRecord.UpdatedBy = Auth.User().Id;
                        transferRecord.UpdatedDate = DateTime.Now;

                        // 4. Log Action
                        var log = new TransferApprovalLog
                        {
                            BudgetTransferId = _transferId,
                            StepNumber = 0, // Final Step
                            RoleName = Auth.User().CCMSRoleCode,
                            UserId = Auth.User().Id,
                            ActionType = "Allocation Budget",
                            ActionDate = DateTime.Now,
                            Status = "UnderReview",
                            Remarks = remarks.Trim()
                        };
                        db.TransferApprovalLog.Add(log);

                        db.SaveChanges();
                        transaction.Commit();

                        Emails.EmailsReqTransferBudgetForFirstApprover(_transferId, transferRecord);
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Transfer finalized successfully.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error: " + ex.Message);
                    }
                }
            }
            Response.Redirect("~/Budget/Transfer/TransferApplication/Default");
        }

        #endregion
    }
}