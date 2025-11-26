using FGV.Prodata.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NPOI.SS.Formula;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.X509;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.Auth;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Util;
using System.Xml.Linq;

namespace Prodata.WebForm.Budget.Transfer.TransferApplication
{
    public partial class Application : ProdataPage
    {
        private Guid _transferId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //decimal requestedAmount = 1234.56M; // replace with your actual value
                //hdnServerAmount.Value = requestedAmount.ToString("0.00");

                string idStr = Request.QueryString["id"];
                if (Guid.TryParse(idStr, out _transferId))
                {
                    LoadTransfer(_transferId);
                    BindControl();
                }
                else
                {
                    Response.Redirect("~/Budget/Transfer/TransferApplication");
                }
            }
        }
        private void BindControl()
        {
            using (var db = new AppDbContext())
            {
                var ba = Auth.User().iPMSBizAreaCode;

                // Parse lblDate.Text (format: dd/MM/yyyy)
                if (!DateTime.TryParseExact(lblDate.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime selectedDate))
                {
                    // Handle parsing error (optional)
                    return;
                }

                string sql = @"
                    SELECT 
                        b.Id,
                        b.Ref,
                        b.Amount 
                        + ISNULL(SUM(
                            CASE 
                                WHEN b.Id = t.ToId THEN ISNULL(t.Amount, 0)
                                WHEN b.Id = t.FromId THEN -ISNULL(t.Amount, 0)
                                ELSE 0
                            END
                        ), 0) AS Balance,
                        b.Ref + ' - ' + b.Details AS BaseDisplay
                    FROM Budgets b
                    LEFT JOIN Transactions t 
                        ON b.Id = t.FromId OR b.Id = t.ToId
                    WHERE b.BizAreaCode = @p0 
                      AND b.DeletedDate IS NULL 
                      AND (t.Date IS NULL OR YEAR(t.Date) = @p1)
                    GROUP BY b.Id, b.Ref, b.Details, b.Amount
                ";

                int selectedYear = selectedDate.Year;

                var rawBudgets = db.Database.SqlQuery<RawBudgetDTO>(sql, ba, selectedYear).ToList();

                var budgets = rawBudgets
                .Where(b => b.Balance != 0)
                .Select(b => new BudgetDTO
                {
                    Id = b.Id,
                    Ref = b.Ref,
                    Balance = b.Balance,
                    Display = $"{b.BaseDisplay} (RM {b.Balance:N2})"
                }).OrderBy(b => b.Display).ToList();

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(budgets);
                ClientScript.RegisterStartupScript(this.GetType(), "budgetData", $"var budgetList = {json};", true);
            }
        }


        // DTO used to fetch raw SQL result
        private class RawBudgetDTO
        {
            public Guid Id { get; set; }
            public string Ref { get; set; }
            public decimal Balance { get; set; }
            public string BaseDisplay { get; set; }
        }

        // Final DTO for JS use
        private class BudgetDTO
        {
            public Guid Id { get; set; }
            public string Ref { get; set; }
            public decimal Balance { get; set; }
            public string Display { get; set; }
        }

        //private void BindControl()
        //{
        //    using (var db = new AppDbContext())
        //    {
        //        var ba = Auth.User().iPMSBizAreaCode;

        //        // Raw SQL to get budget balances
        //        string sql = @"
        //            SELECT 
        //                b.Id,
        //                b.Ref,
        //                (b.Ref + ' - ' + b.Details) AS Display,
        //                b.Amount 
        //                + ISNULL(SUM(
        //                    CASE 
        //                        WHEN b.Id = t.ToId THEN ISNULL(t.Amount, 0)
        //                        WHEN b.Id = t.FromId THEN -ISNULL(t.Amount, 0)
        //                        ELSE 0
        //                    END
        //                ), 0) AS Balance
        //            FROM Budgets b
        //            LEFT JOIN Transactions t 
        //                ON b.Id = t.FromId OR b.Id = t.ToId
        //            WHERE b.BizAreaCode = @p0 AND b.DeletedDate IS NULL 
        //            GROUP BY b.Id, b.Ref, b.Details, b.Amount
        //            ORDER BY Display
        //        ";
        //        //string sql = @"
        //        //    SELECT 
        //        //        b.Id,
        //        //        b.Ref,
        //        //        (b.Ref + ' - ' + b.Details) AS Display,
        //        //        (b.Amount - ISNULL(SUM(ISNULL(t.Amount, 0)), 0)) AS Balance
        //        //    FROM Budgets b
        //        //    LEFT JOIN Transactions t ON b.Id = t.FromId
        //        //    WHERE b.BizAreaCode = @p0 AND b.DeletedDate IS NULL
        //        //    AND FromType = 'Budget'
        //        //    GROUP BY b.Id, b.Ref, b.Details, b.Amount
        //        //    ORDER BY Display
        //        //";

        //        var budgets = db.Database.SqlQuery<BudgetDTO>(sql, ba).ToList();

        //        string json = Newtonsoft.Json.JsonConvert.SerializeObject(budgets);
        //        ClientScript.RegisterStartupScript(this.GetType(), "budgetData", $"var budgetList = {json};", true);
        //    }
        //}
        //public class BudgetDTO
        //{
        //    public Guid Id { get; set; }
        //    public string Ref { get; set; }
        //    public string Display { get; set; }
        //    public decimal Balance { get; set; }
        //}

        protected void btnSubmit_Click(object sender, EventArgs e)
        { 

            //Proceed to save allocations
            using (var db = new AppDbContext())
            {
                //////////Create Guid For Budget
                Guid newIdBudget; 
                string idStr = Request.QueryString["id"];
                if (Guid.TryParse(idStr, out _transferId))
                {

                    //////////Create Budget
                    var TT = db.TransfersTransaction.Where(x => x.Id == _transferId).FirstOrDefault();
                    var newBudget = new Prodata.WebForm.Models.Budget
                    { 
                        TypeId = TT.ToBudgetType,
                        BizAreaCode = TT.ToBA,
                        BizAreaName = new Class.IPMSBizArea().GetNameByCode(TT.ToBA ?? "") ?? "-",
                        Date = DateTime.Now,
                        Month = DateTime.Now.Month,
                        Num = (db.Budgets.OrderByDescending(x => x.Num).Select(x => x.Num).FirstOrDefault()) + 1,
                        Ref = TT.RefNo,
                        Details = "TRANSFER BUDGET FROM : " + TT.FromBA,
                        Wages = 0.0m,
                        Purchase = 0.0m,
                        Amount = 0.0m,
                        Vendor = "LUAR",
                    };
                    db.Budgets.Add(newBudget);
                    db.SaveChanges();
                    newIdBudget = newBudget.Id;

                    int index = 0;
                    while (true)
                    {
                        //////////Create Transaction
                        string budgetKey = $"allocationTable[{index}][budgetRef]";
                        string amountKey = $"allocationTable[{index}][amount]";
                        string IdRefKey = $"allocationTable[{index}][budgetId]";

                        if (!Request.Form.AllKeys.Contains(budgetKey))
                            break;

                        string budgetRef = Request.Form[budgetKey];
                        string amountStr = Request.Form[amountKey];
                        string IdRefKeyStr = Request.Form[IdRefKey];

                        if (!string.IsNullOrWhiteSpace(budgetRef) && decimal.TryParse(amountStr, out decimal amount))
                        {
                        
                            var alloc = new Transaction
                            {
                                FromId = Guid.Parse(IdRefKeyStr),
                                FromType = "Budget",
                                ToId = newIdBudget,
                                ToType = "TransfersTransaction",
                                Amount = amount,
                                Date = DateTime.Now,
                                Name = txtRemarks.Text,
                                Ref = budgetRef
                            };

                            db.Transactions.Add(alloc);
                        }

                        index++;
                    }

                }

                //////////Update Logs
                string roleCode = Auth.User().iPMSRoleCode;
                Guid userId = Auth.User().Id;
                var logEntry = new TransferApprovalLog
                {
                    BudgetTransferId = _transferId,
                    StepNumber = 100,
                    RoleName = roleCode,
                    UserId = userId,
                    ActionType = "Transfer",
                    ActionDate = DateTime.Now,
                    Status = "Finalized",
                    Remarks = txtRemarks.Text?.Trim()
                };
                db.TransferApprovalLog.Add(logEntry);

                var model = db.TransfersTransaction.FirstOrDefault(x => x.Id == _transferId);
                model.UpdatedBy = Auth.User().Id; // Or your method to get current user
                model.UpdatedDate = DateTime.Now;
                model.status = 4; //closed
                db.SaveChanges();
            }
            SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Success Transfer.");
            Response.Redirect("~/Budget/Transfer/TransferApplication");
        }

        private void LoadTransfer(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var transfer = db.TransfersTransaction.FirstOrDefault(x => x.Id == id);
                if (transfer == null)
                {
                    Response.Redirect("~/Budget/Transfer/TransferApplication");
                    return;
                }

                // for status makeup
                string statusText, statusColor;

                if (transfer.DeletedDate != null)
                {
                    statusText = "Deleted";
                    statusColor = "gray";
                }
                else
                {
                    switch (transfer.status)
                    {
                        case 0:
                            statusText = "Resubmit";
                            statusColor = "orange"; // Or use "#FFA500" for a more consistent tone
                            break; 
                        case 2:
                            statusText = "Under Review";
                            statusColor = "blue"; // Optional: "#007BFF" (Bootstrap primary)
                            break;
                        case 3:
                            statusText = "Completed";
                            statusColor = "green"; // Optional: "#28A745" (Bootstrap success)
                            break;
                        case 4:
                            statusText = "Finalized"; // Formerly "Closed"
                            statusColor = "gray"; // Optional: "#6C757D" (Bootstrap muted)
                            break;
                        case 5:
                            statusText = "Deleted";
                            statusColor = "red"; // Optional: "#DC3545"
                            break;
                        default:
                            statusText = "Submitted";
                            statusColor = "black";
                            break;
                    }

                }

                lblStatus.Text = $"<span style='color:{statusColor}; font-weight:600'>{statusText}</span>";
                lblStatus.EnableViewState = false; // optional

                lblRef.Text = transfer.RefNo;
                lblDate.Text = transfer.Date.ToString("dd/MM/yyyy");
                lblApplicantName.Text = db.Users.Where(u => u.Id == transfer.CreatedBy).Select(u => u.Name).FirstOrDefault();
                lblBA.Text = transfer.FromBA.ToString();
                LblBAName.Text = new Class.IPMSBizArea().GetNameByCode(transfer.FromBA ?? "") ?? "-";
                lblAmount.Text = "RM " + transfer.FromTransfer.ToString();
                lblReason.Text = transfer.Justification.ToString();
                 
                string From = db.BudgetTypes.Where(u => u.Id == transfer.FromBudgetType).Select(u => u.Name).FirstOrDefault();
                string To = db.BudgetTypes.Where(u =>u.Id == transfer.ToBudgetType).Select(u => u.Name).FirstOrDefault();
                string FromBA = new Class.IPMSBizArea().GetNameByCode(transfer.FromBA) ?? "unknown";
                string ToBA = new Class.IPMSBizArea().GetNameByCode(transfer.ToBA) ?? "unknown";
                lblTransferFrom.Text = FromBA + "(" + transfer.FromBA + ") - " + From;
                lblTransferTo.Text = ToBA + "(" + transfer.ToBA + ") - " + To;
            }
        }
         
    }
}