using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models.ViewModels
{
    public class FormsProcurementListViewModel
    {
        public Guid Id { get; set; }

        public string Type { get; set; }

        public Guid? BizAreaId { get; set; }
        public string BizAreaCode { get; set; }
        public string BizAreaName { get; set; }

        // Concatenated display (e.g., "BA01 - Finance")
        public string BizAreaDisplayName { get; set; }

        public string Date { get; set; }
        public string Ref { get; set; }
        public string Details { get; set; }
        public string JustificationOfNeed { get; set; }
        public string Remarks { get; set; }

        public string Amount { get; set; }
        public string ProcurementType { get; set; }
        public string Status { get; set; }

        public string ActualAmount { get; set; }

        // Flags for UI logic
        public bool IsEditable { get; set; }
        public bool IsPendingUserAction { get; set; }
    }
}
