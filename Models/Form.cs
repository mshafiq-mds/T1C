using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models
{
    public class Form : BaseModel
    {
        public Guid? EntityId { get; set; }

        public Guid? TypeId { get; set; }

        public Guid? BizAreaId { get; set; }

        [StringLength(100)]
        public string BizAreaCode { get; set; }

        [StringLength(255)]
        public string BizAreaName { get; set; }

        public DateTime? Date { get; set; }

        [StringLength(100)]
        public string Ref { get; set; }

        public string Details { get; set; }

        public string JustificationOfNeed { get; set; }

        public string Remarks { get; set; }

        public decimal? Amount { get; set; }

        public decimal? ActualAmount { get; set; }

        [StringLength(100)]
        public string ProcurementType { get; set; }

        public string Justification { get; set; }

        public decimal? CurrentYearActualYTD { get; set; }

        public decimal? CurrentYearBudget { get; set; }

        public decimal? PreviousYearActualYTD { get; set; }

        public decimal? PreviousYearActual { get; set; }

        public decimal? PreviousYearBudget { get; set; }

        public decimal? A { get; set; }

        [StringLength(255)]
        public string B { get; set; }

        public decimal? C { get; set; }

        public decimal? D { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public virtual ICollection<FormBudget> FormBudgets { get; set; }
    }
}