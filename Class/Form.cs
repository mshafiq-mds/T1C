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
        public List<Models.ViewModels.FormListViewModel> GetForms(
            Guid? entityId = null, 
            Guid? typeId = null, 
            string bizAreaCode = null,
            List<string> bizAreaCodes = null,
            int? year = null,
            string refNo = null, 
            DateTime? startDate = null,
            DateTime? endDate = null, 
            decimal? amountMin = null, 
            decimal? amountMax = null, 
            string procurementType = null,
            List<string> statuses = null
            )
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

                if (bizAreaCodes != null && bizAreaCodes.Any())
                    query = query.Where(q => bizAreaCodes.Contains(q.BizAreaCode));

                if (year.HasValue)
                    query = query.Where(q => q.Date.HasValue && q.Date.Value.Year == year.Value);

                if (!string.IsNullOrEmpty(refNo))
                    query = query.Where(q => q.Ref.Contains(refNo));

                if (startDate.HasValue)
                    query = query.Where(q => q.Date.HasValue && q.Date.Value >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(q => q.Date.HasValue && q.Date.Value <= endDate.Value);

                if (amountMin.HasValue)
                    query = query.Where(q => q.Amount >= amountMin);

                if (amountMax.HasValue)
                    query = query.Where(q => q.Amount <= amountMax);

                if (!string.IsNullOrEmpty(procurementType))
                    query = query.Where(q => q.ProcurementType.Equals(procurementType, StringComparison.OrdinalIgnoreCase));

                if (statuses != null && statuses.Any())
                {
                    var loweredStatuses = statuses.Select(s => s.ToLower()).ToList();
                    query = query.Where(q => q.Status != null && loweredStatuses.Contains(q.Status.ToLower()));
                }

                var query2 = query.OrderBy(q => q.Ref)
                    .ToList();

                return query2.Select(q => new Models.ViewModels.FormListViewModel
                {
                    Id = q.Id,
                    Type = q.Type,
                    BizAreaCode = q.BizAreaCode,
                    BizAreaName = q.BizAreaName,
                    BizAreaDisplayName = q.BizAreaCode + " - " + q.BizAreaName,
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
                    IsEditable = (q.Status.Equals("draft", StringComparison.OrdinalIgnoreCase) || q.Status.Equals("sentback", StringComparison.OrdinalIgnoreCase)),
                    IsPendingUserAction = Prodata.WebForm.Helpers.FormHelper.IsFormPendingUserAction(q.Id, Prodata.WebForm.Auth.User()?.Id)
                }).ToList();
            }
        }

        public List<Models.ViewModels.FormListViewModel> GetFormsForApproval(string ipmsRoleCode = null, List<string> ipmsBizAreaCodes = null)
        {
            using (var db = new AppDbContext())
            {
                // Step 1: Latest approvals per form
                var latestApprovals = db.Approvals
                    .Where(a => a.ObjectType == "Form")
                    .GroupBy(a => a.ObjectId)
                    .Select(g => new
                    {
                        ObjectId = g.Key,
                        LastApprovalOrder = g.OrderByDescending(x => x.CreatedDate)
                                              .Select(x => x.Order)
                                              .FirstOrDefault()
                    })
                    .ToList();

                // Step 2: Applicable approval limits
                var approvalLimits = db.ApprovalLimits
                    .Where(al => al.ApproverType.ToLower() == "ipms_role" &&
                                 (ipmsRoleCode == null || al.ApproverCode.ToLower() == ipmsRoleCode.ToLower()))
                    .ToList();

                // Step 3: Forms and their type names
                var formsWithTypes = (
                    from f in db.Forms.ExcludeSoftDeleted()
                    join t in db.FormTypes on f.TypeId equals t.Id into ft
                    from t in ft.DefaultIfEmpty()
                    select new
                    {
                        Form = f,
                        TypeName = t.Name
                    }
                ).ToList();

               // Step 4: Combine all data
               var query =
                   from fwt in formsWithTypes
                   let f = fwt.Form
                   let tName = fwt.TypeName
                   join la in latestApprovals on f.Id equals la.ObjectId into laGroup
                   from a in laGroup.DefaultIfEmpty()
                   let lastOrder = (a?.LastApprovalOrder ?? 0)
                   let nextOrder = lastOrder + 1
                   from al in approvalLimits
                   where al.Order == nextOrder &&
                         f.Amount >= al.AmountMin &&
                         (al.AmountMax == null || f.Amount <= al.AmountMax) &&
                         (ipmsBizAreaCodes == null || !ipmsBizAreaCodes.Any() || ipmsBizAreaCodes.Contains(f.BizAreaCode))// &&
                                                                                                                          //!new[] { "Draft", "SentBack", "Approved", "Rejected" }.Contains(f.Status)
                   orderby f.Date
                   select new Models.ViewModels.FormListViewModel
                   {
                       Id = f.Id,
                       Type = tName,
                       BizAreaCode = f.BizAreaCode,
                       BizAreaName = f.BizAreaName,
                       BizAreaDisplayName = f.BizAreaCode + " - " + f.BizAreaName,
                       Date = f.Date.HasValue ? f.Date.Value.ToString("dd/MM/yyyy") : string.Empty,
                       Ref = f.Ref,
                       Details = f.Details,
                       JustificationOfNeed = f.JustificationOfNeed,
                       Remarks = f.Remarks,
                       Amount = f.Amount.HasValue ? f.Amount.Value.ToString("#,##0.00") : string.Empty,
                       ProcurementType = f.ProcurementType,
                       Justification = f.Justification,
                       CurrentYearActualYTD = f.CurrentYearActualYTD.HasValue ? f.CurrentYearActualYTD.Value.ToString("#,##0.00") : string.Empty,
                       CurrentYearBudget = f.CurrentYearBudget.HasValue ? f.CurrentYearBudget.Value.ToString("#,##0.00") : string.Empty,
                       PreviousYearActualYTD = f.PreviousYearActualYTD.HasValue ? f.PreviousYearActualYTD.Value.ToString("#,##0.00") : string.Empty,
                       PreviousYearActual = f.PreviousYearActual.HasValue ? f.PreviousYearActual.Value.ToString("#,##0.00") : string.Empty,
                       PreviousYearBudget = f.PreviousYearBudget.HasValue ? f.PreviousYearBudget.Value.ToString("#,##0.00") : string.Empty,
                       A = f.A.HasValue ? f.A.Value.ToString("#,##0.00") : string.Empty,
                       B = f.B,
                       C = f.C.HasValue ? f.C.Value.ToString("#,##0.00") : string.Empty,
                       D = f.D.HasValue ? f.D.Value.ToString("#,##0.00") : string.Empty,
                       Status = f.Status
                   };

                return query.ToList();
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

        public List<Models.ViewModels.FormsProcurementListViewModel> GetFormsProcurement(
            Guid? entityId = null,
            Guid? typeId = null,
            string bizAreaCode = null,
            List<string> bizAreaCodes = null,
            int? year = null,
            string refNo = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            decimal? amountMin = null,
            decimal? amountMax = null,
            string procurementType = null,
            List<string> statuses = null
)
        {
            using (var db = new AppDbContext())
            {
                var query = from f in db.FormsProcurement.ExcludeSoftDeleted()
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
                                f.Status,
                                f.ActualAmount
                            };

                // Filtering
                if (entityId.HasValue)
                    query = query.Where(q => q.EntityId == entityId);

                if (typeId.HasValue)
                    query = query.Where(q => q.TypeId == typeId);

                if (!string.IsNullOrEmpty(bizAreaCode))
                    query = query.Where(q => q.BizAreaCode == bizAreaCode);

                if (bizAreaCodes != null && bizAreaCodes.Any())
                    query = query.Where(q => bizAreaCodes.Contains(q.BizAreaCode));

                if (year.HasValue)
                    query = query.Where(q => q.Date.HasValue && q.Date.Value.Year == year.Value);

                if (!string.IsNullOrEmpty(refNo))
                    query = query.Where(q => q.Ref.Contains(refNo));

                if (startDate.HasValue)
                    query = query.Where(q => q.Date.HasValue && q.Date.Value >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(q => q.Date.HasValue && q.Date.Value <= endDate.Value);

                if (amountMin.HasValue)
                    query = query.Where(q => q.Amount >= amountMin);

                if (amountMax.HasValue)
                    query = query.Where(q => q.Amount <= amountMax);

                if (!string.IsNullOrEmpty(procurementType))
                    query = query.Where(q => q.ProcurementType.Equals(procurementType, StringComparison.OrdinalIgnoreCase));

                if (statuses != null && statuses.Any())
                {
                    var loweredStatuses = statuses.Select(s => s.ToLower()).ToList();
                    query = query.Where(q => q.Status != null && loweredStatuses.Contains(q.Status.ToLower()));
                }

                var query2 = query.OrderByDescending(q => q.Ref).ToList();
                var Name = Auth.User().Name;
                var Id = Auth.User().Id;
                var iPMSRoleCode = Auth.User().iPMSRoleCode;
                var Roles = Auth.User().Roles;
                // Projection to ViewModel
                return query2.Select(q => new Models.ViewModels.FormsProcurementListViewModel
                {
                    Id = q.Id,
                    Type = q.Type,
                    BizAreaCode = q.BizAreaCode,
                    BizAreaName = q.BizAreaName,
                    BizAreaDisplayName = q.BizAreaCode + " - " + q.BizAreaName,
                    Date = q.Date.HasValue ? q.Date.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Ref = q.Ref,
                    Details = q.Details,
                    JustificationOfNeed = q.JustificationOfNeed,
                    Remarks = q.Remarks,
                    Amount = q.Amount.HasValue ? q.Amount.Value.ToString("#,##0.00") : string.Empty,
                    ProcurementType = q.ProcurementType,
                    Status = q.Status != null ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(q.Status) : string.Empty,
                    ActualAmount = q.ActualAmount.HasValue ? q.ActualAmount.Value.ToString("#,##0.00") : string.Empty,
                    IsEditable = (q.Status == "Pending" && (Auth.User().iPMSRoleCode == "MM" || Auth.User().iPMSRoleCode == "AM" || Auth.User().Name.Equals("Superadmin", StringComparison.OrdinalIgnoreCase))) ? true : false,
                    IsPendingUserAction = (q.Status == "Pending" && (Auth.User().iPMSRoleCode == "MM" || Auth.User().iPMSRoleCode == "AM" || Auth.User().Name.Equals("Superadmin", StringComparison.OrdinalIgnoreCase))) ? true : false
                }).ToList();
            }
        }

    }
}