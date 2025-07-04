using CustomGuid.AspNet.Identity;
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

        public static string GetLatestFormRemark(Guid formId)
        {
            using (var db = new AppDbContext())
            {
                var approval = db.Approvals.ExcludeSoftDeleted()
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
                    .Where(t => t.FromId == form.Id && t.FromType.Equals("Form", StringComparison.OrdinalIgnoreCase))
                    .ToList();
                foreach (var transaction in transactions)
                {
                    db.SoftDelete(transaction);
                }
            }
        }
    }
}