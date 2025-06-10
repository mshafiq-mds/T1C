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
		public List<Models.ViewModels.BudgetListViewModel> GetBudgets(Guid? entityId = null, Guid? typeId = null, int? month = null, string bizAreaCode = null, string refNo = null, decimal? amountMin = null, decimal? amountMax = null)
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

                return query2.Select(q => new Models.ViewModels.BudgetListViewModel
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
                    DisplayName = q.Ref + " - " + q.Details + " (RM" + (q.Amount.HasValue ? q.Amount.Value.ToString("#,##0.00") : "0.00") + ")",
                    Details = q.Details,
                    Wages = q.Wages.HasValue ? q.Wages.Value.ToString("#,##0.00") : string.Empty,
                    Purchase = q.Purchase.HasValue ? q.Purchase.Value.ToString("#,##0.00") : string.Empty,
                    Amount = q.Amount.HasValue ? q.Amount.Value.ToString("#,##0.00") : string.Empty,
                    Vendor = q.Vendor,
                    Status = q.Status
                }).ToList();
            }
		}
    }
}