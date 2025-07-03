using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Class
{
    public class Approval
    {
        public List<Models.ViewModels.ApprovalListViewModel> GetApprovals(Guid? formId = null, string type = null)
        {
            using (var db = new AppDbContext())
            {
                var query = db.Approvals.AsQueryable();

                if (formId.HasValue)
                    query = query.Where(q => q.ObjectId == formId);
                if (!string.IsNullOrEmpty(type))
                    query = query.Where(a => a.ObjectType.Equals(type, StringComparison.OrdinalIgnoreCase));

                // Materialize the data first, then format
                return query
                    .OrderByDescending(q => q.CreatedDate)
                    .ToList() // ← Fetch from DB first
                    .Select(a => new Models.ViewModels.ApprovalListViewModel
                    {
                        ActionByName = a.ActionByName,
                        ActionByRole = a.ActionByCode,
                        Action = a.Action,
                        Remark = a.Remark,
                        Datetime = a.CreatedDate.ToString("dd/MM/yyyy h:mm tt")
                    })
                    .ToList();
            }
        }
    }
}