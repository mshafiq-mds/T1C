using CustomGuid.AspNet.Identity;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Class
{
	public class Budget
	{
        // ===========================================
        // ✅ Shared status label function
        // ===========================================
        public static string GetStatusName(int? status, DateTime? deletedDate)
        {
            if (deletedDate != null) return "Deleted";

            switch (status)
            {
                case 0: return "Resubmit";
                case 1: return "Submitted";
                case 2: return "Under Review";
                case 3: return "Completed";
                case 4: return "Finalized";
                default: return "Unknown";
            }
        }

        // ===========================================
        // ✅ Additional Budget / Finance Logic
        // ===========================================
        public static AdditionalLoaFinanceLimits GetMatchingFinanceLimit(List<AdditionalLoaFinanceLimits> limits, decimal? amount)
        {
            return limits.FirstOrDefault(l =>
                l.AmountMin <= (amount ?? 0m) &&
                (l.AmountMax == null || l.AmountMax >= (amount ?? 0m)));
        }

        public static bool CanEditFinanceRequest(AdditionalLoaFinanceLimits limit, int userLevel, int currentLevel, DateTime? deletedDate)
        {
            return deletedDate == null && limit != null && userLevel == currentLevel + 1;
        }

        // ===========================================
        // ✅ Additional Budget / COGS Logic
        // ===========================================
        public static AdditionalLoaCogsLimits GetMatchingAdditionalLimit(List<AdditionalLoaCogsLimits> limits, decimal? amount)
        {
            return limits.FirstOrDefault(l =>
                l.AmountMin <= (amount ?? 0m) &&
                (l.AmountMax == null || l.AmountMax >= (amount ?? 0m)));
        }

        public static bool CanEditAdditionalBudget(AdditionalLoaCogsLimits limit, int userLevel, int currentLevel, DateTime? deletedDate)
        {
            return deletedDate == null && limit != null && userLevel == currentLevel + 1;
        }

        // ===========================================
        // ✅ Additional Budget / Cumulative Logic
        // ===========================================
        public static AdditionalCumulativeLimits GetEligibleCumulativeLimit(List<AdditionalCumulativeLimits> limits, decimal? requestedAmount)
        {
            decimal amount = requestedAmount ?? 0m;
            return limits
                .Where(l => (l.AmountCumulativeBalance ?? 0m) >= amount)
                .OrderBy(l => l.Order)
                .FirstOrDefault();
        }

        public static bool CanEditCumulativeRequest(AdditionalCumulativeLimits limit, string userRole, int currentLevel, DateTime? deletedDate)
        {
            return deletedDate == null &&
                   limit != null &&
                   limit.CumulativeApproverCode == userRole &&
                   limit.Order == currentLevel + 1;
        }

        // ===========================================
        // ✅ Shared for Additional Budget Logs
        // ===========================================
        public static int GetAdditionalBudgetApprovalLevel(AppDbContext db, Guid budgetId)
        {
            return db.AdditionalBudgetLog
                .Where(w => w.BudgetTransferId == budgetId)
                .OrderByDescending(w => w.CreatedDate)
                .Select(w => w.StepNumber)
                .FirstOrDefault();
        }

        // ===========================================
        // ✅ Transfer Budget Logic
        // ===========================================
        public static TransferApprovalLimits GetMatchingTransferLimit(List<TransferApprovalLimits> limits, decimal amount)
        {
            return limits.FirstOrDefault(l =>
                l.AmountMin <= amount &&
                (l.AmountMax == null || l.AmountMax >= amount));
        }

        public static bool CanEditTransfer(TransferApprovalLimits limit, int userLevel, int currentLevel, DateTime? deletedDate)
        {
            return deletedDate == null && limit != null && userLevel == currentLevel + 1;
        }

        public static int GetTransferApprovalLevel(AppDbContext db, Guid transferId)
        {
            return db.TransferApprovalLog
                .Where(w => w.BudgetTransferId == transferId)
                .OrderByDescending(w => w.CreatedDate)
                .Select(w => w.StepNumber)
                .FirstOrDefault();
        }


        // ===========================================
        // ✅ T1C Budget Logic
        // ===========================================

        public List<Models.ViewModels.BudgetListViewModel> GetBudgets(
            Guid? entityId = null, 
            Guid? typeId = null, 
            int? year = null, 
            int? month = null, 
            string bizAreaCode = null, 
            string refNo = null, 
            decimal? amountMin = null, 
            decimal? amountMax = null,
            Guid? excludedFormId = null)
		{
			using (var db = new AppDbContext())
			{
                var query = from b in db.Budgets.ExcludeSoftDeleted()
                            join t in db.BudgetTypes on b.TypeId equals t.Id into bt
                            from t in bt.DefaultIfEmpty()
                            select new
                            {
                                b.Id,
                                b.EntityId,
                                b.TypeId,
                                Type = t.Name,
                                b.BizAreaCode,
                                b.BizAreaName,
                                b.Date,
                                b.Month,
                                b.Num,
                                b.Ref,
                                b.Name,
                                b.Details,
                                b.Wages,
                                b.Purchase,
                                b.Amount,
                                b.Vendor,
                                b.Status
                            };

                if (entityId.HasValue)
                    query = query.Where(q => q.EntityId == entityId);

				if (typeId.HasValue)
                    query = query.Where(q => q.TypeId == typeId);

                if (year.HasValue)
                    query = query.Where(q => q.Date.HasValue && q.Date.Value.Year == year);

                if (month.HasValue)
                    query = query.Where(q => q.Month == month);

                if (!string.IsNullOrEmpty(bizAreaCode))
                    query = query.Where(q => q.BizAreaCode == bizAreaCode);

                if (!string.IsNullOrEmpty(refNo))
                    query = query.Where(q => q.Ref.Contains(refNo));

                if (amountMin.HasValue)
                    query = query.Where(q => q.Amount >= amountMin);

                if (amountMax.HasValue)
                    query = query.Where(q => q.Amount <= amountMax);

                var query2 = query.OrderBy(q => q.Num)
                    .ThenBy(q => q.Ref)
                    .ToList();

                // ✅ Step 1: Extract budget IDs
                var budgetIds = query2.Select(q => q.Id).ToList();

                // ✅ Step 2: Batch load utilized amounts per budget
                var utilizedMap = db.Transactions.ExcludeSoftDeleted()
                    .Where(t => budgetIds.Contains(t.FromId.Value)
                             && t.FromType.Equals("Budget", StringComparison.OrdinalIgnoreCase)
                             && (excludedFormId == null || (t.ToId != excludedFormId.Value && t.ToType.Equals("Form", StringComparison.OrdinalIgnoreCase)))
                             && !t.Status.Equals("rejected", StringComparison.OrdinalIgnoreCase))
                    .GroupBy(t => t.FromId)
                    .Select(g => new
                    {
                        BudgetId = g.Key,
                        Utilized = g.Sum(t => t.Amount) ?? 0m
                    })
                    .ToDictionary(x => x.BudgetId, x => x.Utilized);

                // ✅ Step 3: Project into view model with balance
                return query2.Select(q =>
                {
                    decimal amount = q.Amount ?? 0m;
                    decimal utilized = utilizedMap.ContainsKey(q.Id) ? utilizedMap[q.Id] : 0m;
                    decimal balance = amount - utilized;

                    return new Models.ViewModels.BudgetListViewModel
                    {
                        Id = q.Id,
                        Type = q.Type,
                        BizAreaCode = q.BizAreaCode,
                        BizAreaName = q.BizAreaName,
                        Date = q.Date.HasValue ? q.Date.Value.ToString("dd/MM/yyyy") : string.Empty,
                        Month = q.Month.HasValue && q.Month.Value >= 1 && q.Month.Value <= 12
                            ? new DateTime(2000, q.Month.Value, 1).ToString("MMM", CultureInfo.CurrentCulture).ToUpper()
                            : string.Empty,
                        Ref = q.Ref,
                        Name = q.Name,
                        DisplayName = q.Ref + " - " + q.Details + " (RM" + balance.ToString("#,##0.00") + ")",
                        Details = q.Details,
                        Wages = q.Wages.HasValue ? q.Wages.Value.ToString("#,##0.00") : string.Empty,
                        Purchase = q.Purchase.HasValue ? q.Purchase.Value.ToString("#,##0.00") : string.Empty,
                        Amount = balance.ToString("#,##0.00"), // ✅ Use balance instead of amount
                        Vendor = q.Vendor,
                        Status = q.Status
                    };
                }).ToList();
            }
		}
    }
}