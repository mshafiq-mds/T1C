using CustomGuid.AspNet.Identity;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Helpers
{
    public static class BudgetHelper
    {
        public static decimal GetBudgetUtilized(Guid budgetId, Guid? excludedFormId = null)
        {
            using (var db = new AppDbContext())
            {
                var query = db.Transactions.ExcludeSoftDeleted()
                    .Where(t => t.FromId == budgetId
                             && t.FromType.Equals("Budget", StringComparison.OrdinalIgnoreCase)
                             && !t.Status.Equals("rejected", StringComparison.OrdinalIgnoreCase));

                if (excludedFormId.HasValue)
                {
                    query = query.Where(t => t.ToId != excludedFormId.Value 
                                          && t.ToType.Equals("Form", StringComparison.OrdinalIgnoreCase));
                }

                return query.Sum(t => t.Amount) ?? 0m;

            }
        }

        public static decimal GetBudgetUtilized(this Models.Budget budget)
        {
            if (budget == null || budget.Id == Guid.Empty)
            {
                return 0m;
            }
            return GetBudgetUtilized(budget.Id);
        }

        public static decimal GetBudgetBalance(Guid budgetId, Guid? excludedFormId = null)
        {
            using (var db = new AppDbContext())
            {
                var budget = db.Budgets.Find(budgetId);
                if (budget != null)
                {
                    decimal totalAmount = budget.Amount ?? 0m;
                    return totalAmount - GetBudgetUtilized(budgetId, excludedFormId);
                }
                return 0m;
            }
        }

        public static decimal GetBudgetBalance(this Models.Budget budget)
        {
            if (budget == null || budget.Id == Guid.Empty)
            {
                return 0m;
            }
            return GetBudgetBalance(budget.Id);
        }
    }
}