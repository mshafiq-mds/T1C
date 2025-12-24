using FGV.Prodata.Web.UI;
using Newtonsoft.Json;
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
                        LoadBudgetDropdownData();
                    }
                }
                else
                {
                    Response.Redirect("~/Budget/Transfer/TransferApplication");
                }
            }
        }

        #region Data Loading

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
                string toBA = new Class.IPMSBizArea().GetNameByCode(transfer.ToBA) ?? transfer.ToBA;

                lblTransferInfo.Text = $"<strong>{transfer.FromBA}</strong> ({fromType}) <i class='fas fa-arrow-right mx-1'></i> <strong>{toBA}</strong> ({toType})";

                string badgeClass = "badge bg-secondary";
                string statusText = "Unknown";
                switch (transfer.status)
                {
                    case 0: statusText = "Resubmit"; badgeClass = "badge bg-warning text-dark"; break;
                    case 1: statusText = "Submitted"; badgeClass = "badge bg-info text-dark"; break;
                    case 2: statusText = "Under Review"; badgeClass = "badge bg-primary"; break;
                    case 3: statusText = "Completed"; badgeClass = "badge bg-success"; break;
                    case 4: statusText = "Finalized"; badgeClass = "badge bg-dark"; break;
                    case 5: statusText = "Deleted"; badgeClass = "badge bg-danger"; break;
                }
                litStatusHtml.Text = $"<span class='{badgeClass} badge-status rounded-pill'>{statusText}</span>";

                return true;
            }
        }

        private void LoadBudgetDropdownData()
        {
            using (var db = new AppDbContext())
            {
                var baCode = Auth.User().CCMSBizAreaCode;
                int year = DateTime.TryParse(lblDate.Text, out DateTime dt) ? dt.Year : DateTime.Now.Year;

                // Query to get current balance for each budget
                string sql = @"
                    SELECT 
                        b.Id,
                        b.Ref,
                        b.Details,
                        (b.Amount + 
                         ISNULL((SELECT SUM(ISNULL(ti.Amount, 0)) FROM Transactions ti WHERE ti.ToId = b.Id AND ti.ToType = 'Budget' AND ti.DeletedDate IS NULL), 0) - 
                         ISNULL((SELECT SUM(ISNULL(to_t.Amount, 0)) FROM Transactions to_t WHERE to_t.FromId = b.Id AND to_t.FromType = 'Budget' AND to_t.DeletedDate IS NULL), 0)
                        ) AS Balance
                    FROM Budgets b
                    WHERE b.BizAreaCode = @p0 
                      AND b.DeletedDate IS NULL 
                      AND YEAR(b.Date) = @p1
                ";

                var rawData = db.Database.SqlQuery<RawBudgetDTO>(sql, baCode, year).ToList();

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

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Guid.TryParse(Request.QueryString["id"], out _transferId))
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Invalid Transfer ID.");
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

                        // 1. Create Destination Budget
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
                            CreatedDate = DateTime.Now
                        };
                        db.Budgets.Add(newBudget);
                        db.SaveChanges();

                        // 2. Loop through dynamic rows
                        int index = 0;
                        decimal totalAllocated = 0;

                        while (true)
                        {
                            string idKey = $"allocations[{index}][BudgetId]";
                            string amountKey = $"allocations[{index}][Amount]";
                            string refKey = $"allocations[{index}][BudgetRef]";

                            if (string.IsNullOrEmpty(Request.Form[idKey])) break;

                            Guid budgetSourceId = Guid.Parse(Request.Form[idKey]);
                            string budgetSourceRef = Request.Form[refKey];
                            decimal amount = decimal.Parse(Request.Form[amountKey]);

                            if (amount > 0)
                            {
                                var txn = new Transaction
                                {
                                    Id = Guid.NewGuid(),
                                    FromId = budgetSourceId,
                                    FromType = "Budget",
                                    ToId = newBudget.Id,
                                    ToType = "Budget",
                                    //ToId = transferRecord.Id,
                                    //ToType = "TransfersTransaction",
                                    Amount = amount,
                                    Date = DateTime.Now,
                                    Name = txtRemarks.Text.Trim(),
                                    Ref = budgetSourceRef,
                                    CreatedBy = Auth.User().Id,
                                    CreatedDate = DateTime.Now
                                };
                                db.Transactions.Add(txn);
                                totalAllocated += amount;
                            }
                            index++;
                        }

                        // 3. Final Validation
                        decimal requestedAmt = transferRecord.FromTransfer ?? 0;
                        if (Math.Abs(totalAllocated - requestedAmt) > 0.01m)
                        {
                            throw new Exception("Allocation Mismatch on Server.");
                        }

                        // 4. Update Status
                        transferRecord.status = 4; // Finalized
                        transferRecord.UpdatedBy = Auth.User().Id;
                        transferRecord.UpdatedDate = DateTime.Now;

                        var log = new TransferApprovalLog
                        {
                            BudgetTransferId = _transferId,
                            StepNumber = 100,
                            RoleName = Auth.User().CCMSRoleCode,
                            UserId = Auth.User().Id,
                            ActionType = "Finalized",
                            ActionDate = DateTime.Now,
                            Status = "Finalized",
                            Remarks = txtRemarks.Text.Trim()
                        };
                        db.TransferApprovalLog.Add(log);

                        //newBudget.Amount = totalAllocated;
                        
                        db.SaveChanges();
                        transaction.Commit();

                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Transfer finalized successfully.");
                        Response.Redirect("~/Budget/Transfer/TransferApplication");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error: " + ex.Message);
                    }
                }
            }
        }
    }
}