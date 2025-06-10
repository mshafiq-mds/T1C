using CustomGuid.AspNet.Identity;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Class
{
    public class Form
    {
        public List<Models.ViewModels.FormListViewModel> GetForms(Guid? entityId = null, Guid? typeId = null, string bizAreaCode = null, string refNo = null, decimal? amountMin = null, decimal? amountMax = null, string procurementType = null)
        {
            using (var db = new AppDbContext())
            {
                var query = from f in db.Forms.ExcludeSoftDeleted()
                            join t in db.FormTypes on f.TypeId equals t.Id into ft
                            from t in ft.DefaultIfEmpty()
                            select new
                            {
                                f.Id,
                                f.EntityId,
                                f.TypeId,
                                Type = t.Name,
                                f.BizAreaCode,
                                f.BizAreaName,
                                f.Date,
                                f.Ref,
                                f.Details,
                                f.JustificationOfNeed,
                                f.Remarks,
                                f.Amount,
                                f.ProcurementType,
                                f.Justification,
                                f.CurrentYearActualYTD,
                                f.CurrentYearBudget,
                                f.PreviousYearActualYTD,
                                f.PreviousYearActual,
                                f.PreviousYearBudget,
                                f.A,
                                f.B,
                                f.C,
                                f.D,
                                f.Status
                            };

                if (entityId.HasValue)
                    query = query.Where(q => q.EntityId == entityId);

                if (typeId.HasValue)
                    query = query.Where(q => q.TypeId == typeId);

                if (!string.IsNullOrEmpty(bizAreaCode))
                    query = query.Where(q => q.BizAreaCode == bizAreaCode);

                if (!string.IsNullOrEmpty(refNo))
                    query = query.Where(q => q.Ref.Contains(refNo));

                if (amountMin.HasValue)
                    query = query.Where(q => q.Amount >= amountMin);

                if (amountMax.HasValue)
                    query = query.Where(q => q.Amount <= amountMax);

                if (!string.IsNullOrEmpty(procurementType))
                    query = query.Where(q => q.ProcurementType.Equals(procurementType, StringComparison.OrdinalIgnoreCase));

                var query2 = query.OrderBy(q => q.Ref)
                    .ToList();

                return query2.Select(q => new Models.ViewModels.FormListViewModel
                {
                    Id = q.Id,
                    Type = q.Type,
                    BizAreaCode = q.BizAreaCode,
                    BizAreaName = q.BizAreaName,
                    Date = q.Date.HasValue ? q.Date.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Ref = q.Ref,
                    Details = q.Details,
                    JustificationOfNeed = q.JustificationOfNeed,
                    Remarks = q.Remarks,
                    Amount = q.Amount.HasValue ? q.Amount.Value.ToString("#,##0.00") : string.Empty,
                    ProcurementType = q.ProcurementType,
                    Justification = q.Justification,
                    CurrentYearActualYTD = q.CurrentYearActualYTD.HasValue ? q.CurrentYearActualYTD.Value.ToString("#,##0.00") : string.Empty,
                    CurrentYearBudget = q.CurrentYearBudget.HasValue ? q.CurrentYearBudget.Value.ToString("#,##0.00") : string.Empty,
                    PreviousYearActualYTD = q.PreviousYearActualYTD.HasValue ? q.PreviousYearActualYTD.Value.ToString("#,##0.00") : string.Empty,
                    PreviousYearActual = q.PreviousYearActual.HasValue ? q.PreviousYearActual.Value.ToString("#,##0.00") : string.Empty,
                    PreviousYearBudget = q.PreviousYearBudget.HasValue ? q.PreviousYearBudget.Value.ToString("#,##0.00") : string.Empty,
                    A = q.A.HasValue ? q.A.Value.ToString("#,##0.00") : string.Empty,
                    B = q.B,
                    C = q.C.HasValue ? q.C.Value.ToString("#,##0.00") : string.Empty,
                    D = q.D.HasValue ? q.D.Value.ToString("#,##0.00") : string.Empty,
                    Status = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(q.Status), //q.Status,
                    IsEditable = q.Status.Equals("draft", StringComparison.OrdinalIgnoreCase)
                }).ToList();
            }
        }

        public List<Models.ViewModels.FormListViewModel> GetFormsForApproval(string ipmsRoleCode = null)
        {
            using (var db = new AppDbContext())
            {
                // Step 1: Materialize latest approvals
                var latestApprovals = db.Approvals
                    .Where(a => a.ObjectType == "Form")
                    .GroupBy(a => a.ObjectId)
                    .Select(g => new
                    {
                        ObjectId = g.Key,
                        LastApprovalOrder = g.OrderByDescending(x => x.CreatedDate).Select(x => x.Order).FirstOrDefault()
                    })
                    .ToList(); // in-memory after this point

                // Step 2: Materialize only the approval limits needed
                var approvalLimits = db.ApprovalLimits
                    .Where(al => al.ApproverType.ToLower() == "ipms_role" &&
                                 (ipmsRoleCode == null || al.ApproverCode.ToLower() == ipmsRoleCode.ToLower()))
                    .ToList(); // in-memory

                // Step 3: Materialize forms + types (before joining with in-memory data)
                var formsWithTypes = (
                    from f in db.Forms.ExcludeSoftDeleted()
                    join t in db.FormTypes on f.TypeId equals t.Id into ft
                    from t in ft.DefaultIfEmpty()
                    select new
                    {
                        Form = f,
                        TypeName = t.Name
                    }
                ).ToList(); // materialized before joining with in-memory collections

                // Step 4: In-memory join with latestApprovals and approvalLimits
                var query =
                    from fwt in formsWithTypes
                    let f = fwt.Form
                    let tName = fwt.TypeName
                    join la in latestApprovals on f.Id equals la.ObjectId into laGroup
                    from a in laGroup.DefaultIfEmpty()
                    let lastOrder = (a != null && a.LastApprovalOrder.HasValue) ? a.LastApprovalOrder.Value : 0
                    let nextOrder = lastOrder + 1
                    from al in approvalLimits
                    where al.Order == nextOrder &&
                          f.Amount >= al.AmountMin &&
                          (al.AmountMax == null || f.Amount <= al.AmountMax)
                    select new
                    {
                        f.Id,
                        f.EntityId,
                        f.TypeId,
                        Type = tName,
                        f.BizAreaCode,
                        f.BizAreaName,
                        f.Date,
                        f.Ref,
                        f.Details,
                        f.JustificationOfNeed,
                        f.Remarks,
                        f.Amount,
                        f.ProcurementType,
                        f.Justification,
                        f.CurrentYearActualYTD,
                        f.CurrentYearBudget,
                        f.PreviousYearActualYTD,
                        f.PreviousYearActual,
                        f.PreviousYearBudget,
                        f.A,
                        f.B,
                        f.C,
                        f.D,
                        f.Status,
                        ApprovalLimitOrder = al.Order,
                        ApprovalOrder = a.LastApprovalOrder
                    };

                // Step 5: Ordering and projection to ViewModel
                var query2 = query.OrderBy(q => q.Date).ToList();

                var result = query.Select(q => new Models.ViewModels.FormListViewModel
                {
                    Id = q.Id,
                    Type = q.Type,
                    BizAreaCode = q.BizAreaCode,
                    BizAreaName = q.BizAreaName,
                    Date = q.Date.HasValue ? q.Date.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Ref = q.Ref,
                    Details = q.Details,
                    JustificationOfNeed = q.JustificationOfNeed,
                    Remarks = q.Remarks,
                    Amount = q.Amount.HasValue ? q.Amount.Value.ToString("#,##0.00") : string.Empty,
                    ProcurementType = q.ProcurementType,
                    Justification = q.Justification,
                    CurrentYearActualYTD = q.CurrentYearActualYTD.HasValue ? q.CurrentYearActualYTD.Value.ToString("#,##0.00") : string.Empty,
                    CurrentYearBudget = q.CurrentYearBudget.HasValue ? q.CurrentYearBudget.Value.ToString("#,##0.00") : string.Empty,
                    PreviousYearActualYTD = q.PreviousYearActualYTD.HasValue ? q.PreviousYearActualYTD.Value.ToString("#,##0.00") : string.Empty,
                    PreviousYearActual = q.PreviousYearActual.HasValue ? q.PreviousYearActual.Value.ToString("#,##0.00") : string.Empty,
                    PreviousYearBudget = q.PreviousYearBudget.HasValue ? q.PreviousYearBudget.Value.ToString("#,##0.00") : string.Empty,
                    A = q.A.HasValue ? q.A.Value.ToString("#,##0.00") : string.Empty,
                    B = q.B,
                    C = q.C.HasValue ? q.C.Value.ToString("#,##0.00") : string.Empty,
                    D = q.D.HasValue ? q.D.Value.ToString("#,##0.00") : string.Empty,
                    Status = q.Status
                }).ToList();

                return result;
            }
        }

        public bool IsFormHasNextApprover(Guid formId)
        {
            using (var db = new AppDbContext())
            {
                var form = db.Forms.FirstOrDefault(f => f.Id == formId);
                if (form == null || !form.Amount.HasValue)
                {
                    return false; // Form not found or no amount set
                }

                var latestApproval = db.Approvals
                    .Where(a => a.ObjectId == formId && a.ObjectType.ToLower() == "form")
                    .OrderByDescending(a => a.CreatedDate)
                    .FirstOrDefault();

                int nextOrder = (latestApproval?.Order ?? 0) + 1;

                var nextApprover = db.ApprovalLimits
                    .Where(al => al.Order == nextOrder &&
                                 form.Amount.Value >= al.AmountMin &&
                                 (al.AmountMax == null || form.Amount.Value <= al.AmountMax))
                    .FirstOrDefault();

                return nextApprover != null;
            }
        }
    }
}