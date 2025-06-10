using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models.ViewModels
{
    public class FormListViewModel
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string BizAreaCode { get; set; }
        public string BizAreaName { get; set; }
        public string Date { get; set; }
        public string Ref { get; set; }
        public string Details { get; set; }
        public string JustificationOfNeed { get; set; }
        public string Remarks { get; set; }
        public string Amount { get; set; }
        public string ProcurementType { get; set; }
        public string Justification { get; set; }
        public string CurrentYearActualYTD { get; set; }
        public string CurrentYearBudget { get; set; }
        public string PreviousYearActualYTD { get; set; }
        public string PreviousYearActual { get; set; }
        public string PreviousYearBudget { get; set; }
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
        public string D { get; set; }
        public string Status { get; set; }

        public bool IsEditable { get; set; }
    }
}