using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prodata.WebForm.Models
{
    [Table("Budgets_Audit")]
    public class Budgets_Audit
    {
        [Key]
        public Guid AuditId { get; set; }

        public Guid BudgetId { get; set; }

        [Required]
        [StringLength(50)]
        public string Action { get; set; }

        public Guid? ActionBy { get; set; }

        public DateTime? ActionDate { get; set; }

        public Guid? EntityId { get; set; }

        public Guid? TypeId { get; set; }

        public Guid? BizAreaId { get; set; }

        [StringLength(100)]
        public string BizAreaCode { get; set; }

        [StringLength(255)]
        public string BizAreaName { get; set; }

        public DateTime? Date { get; set; }

        public int? Month { get; set; }

        public int? Num { get; set; }

        [StringLength(100)]
        public string Ref { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public string Details { get; set; }

        public decimal? Wages { get; set; }

        public decimal? Purchase { get; set; }

        public decimal? Amount { get; set; }

        [StringLength(255)]
        public string Vendor { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid? DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}