using CustomGuid.AspNet.Identity;
using Microsoft.AspNet.Identity;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Helpers
{
    public static class FormHelper
    {
        public static bool IsFormEditable(Guid formId)
        {
            using (var db = new AppDbContext())
            {
                var form = db.Forms.Find(formId);
                if (form != null)
                {
                    return form.IsFormEditable();
                }
            }
            return false;
        }

        public static bool IsFormEditable(this Models.Form form)
        {
            if (form.Status.Equals("draft", StringComparison.OrdinalIgnoreCase) || form.Status.Equals("sentback", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public static bool IsFormSentBack(Guid formId)
        {
            using (var db = new AppDbContext())
            {
                var form = db.Forms.Find(formId);
                if (form != null)
                {
                    return form.Status.Equals("sentback", StringComparison.OrdinalIgnoreCase);
                }
            }
            return false;
        }

        public static bool IsFormSentBack(this Models.Form form)
        {
            return form.Status.Equals("sentback", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsFormPendingUserAction(Guid formId, Guid? userId = null)
        {
            using (var db = new AppDbContext())
            {
                var user = userId.HasValue ? db.Users.Find(userId.Value) : Auth.User();
                if (user == null) return false;

                var form = db.Forms.Find(formId);
                if (form == null) return false;

                var approvalLimits = db.ApprovalLimits.ExcludeSoftDeleted()
                    .Where(al => al.AmountMin <= form.Amount && form.Amount <= al.AmountMax)
                    .OrderBy(al => al.Order)
                    .ToList();

                if (!approvalLimits.Any()) return false;

                var approvals = db.Approvals
                    .Where(a => a.ObjectId == formId && a.ObjectType.Equals("Form", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(a => a.CreatedDate)
                    .ToList();

                if (!approvals.Any()) return false;

                // Step 1: Find the last "Submitted"
                var lastSubmittedIndex = approvals.FindLastIndex(a => a.Action.Equals("Submitted", StringComparison.OrdinalIgnoreCase));
                if (lastSubmittedIndex == -1)
                    return false;

                // Step 2: Find approvals made AFTER the last submission
                var approvalsAfterLastSubmit = approvals
                    .Skip(lastSubmittedIndex + 1)
                    .Where(a => a.Action.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var approvedCodes = approvalsAfterLastSubmit
                    .Select(a => a.ActionByCode)
                    .ToList();

                // Step 3: Find the next approver in the sequence who hasn't approved since last submit
                foreach (var limit in approvalLimits)
                {
                    if (!approvedCodes.Contains(limit.ApproverCode, StringComparer.OrdinalIgnoreCase))
                    {
                        // This approver is next in line
                        return string.Equals(user.iPMSRoleCode, limit.ApproverCode, StringComparison.OrdinalIgnoreCase);
                    }
                }

                return false;
            }
        }


        public static bool IsFormPendingUserAction(this Models.Form form)
        {
            return IsFormPendingUserAction(form.Id);
        }

        public static bool IsFormProcumentPendingUserAction(this Models.FormsProcurement form)
        {
            return IsFormPendingUserAction(form.Id);
        }

        public static string GetLatestFormRemark(Guid formId)
        {
            using (var db = new AppDbContext())
            {
                var approval = db.Approvals
                    .Where(a => a.ObjectId == formId && a.ObjectType.Equals("Form", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(a => a.CreatedDate)
                    .FirstOrDefault();
                if (approval != null)
                {
                    return approval.Remark;
                }
            }
            return string.Empty;
        }

        public static string GetLatestFormRemark(this Models.Form form)
        {
            return GetLatestFormRemark(form.Id);
        }

        public static void SoftDeleteTransactions(this Models.Form form)
        {
            using (var db = new AppDbContext())
            {
                var transactions = db.Transactions.ExcludeSoftDeleted()
                    .Where(t => t.ToId == form.Id && t.ToType.Equals("Form", StringComparison.OrdinalIgnoreCase))
                    .ToList();
                foreach (var transaction in transactions)
                {
                    db.SoftDelete(transaction);
                }
            }
        }
    }
}