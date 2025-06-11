using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models
{
    [Table("AdditionalBudgetRequests")]
    public class AdditionalBudgetRequests : BaseModel
    {
        [Key]
        public Guid Id { get; set; }

        // Application Metadata
        [Required]
        [MaxLength(100)]
        public string RefNo { get; set; }

        [Required]
        [MaxLength(255)]
        public string Project { get; set; }

        [Required]
        public DateTime ApplicationDate { get; set; }

        [Required]
        [MaxLength(10)]
        public string BudgetType { get; set; } // OPEX or CAPEX

        [Required]
        [MaxLength(10)]
        public string CheckType { get; set; } // OPEX or CAPEX

        [Required]
        public decimal EstimatedCost { get; set; }

        [MaxLength(100)]
        public string EVisaNo { get; set; }

        // Main Justification
        public string RequestDetails { get; set; }
        public string Reason { get; set; }

        // Additional Budget Table
        [MaxLength(100)]
        public string CostCentre { get; set; }

        [MaxLength(50)]
        public string GLCode { get; set; }

        public decimal? ApprovedBudget { get; set; }

        public decimal? NewTotalBudget { get; set; }

        public decimal? AdditionalBudget { get; set; }

        // Tracking
        [MaxLength(50)]
        public string BA { get; set; }

        public int? Status { get; set; }

        // Audit Fields
        public Guid? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid? DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}