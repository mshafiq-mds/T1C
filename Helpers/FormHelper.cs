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
            if (form.Status.Equals("draft", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }
    }
}