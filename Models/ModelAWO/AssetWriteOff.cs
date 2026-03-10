using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prodata.WebForm.Models.ModelAWO
{
    [Table("AssetWriteOffs")]
    public class AssetWriteOff
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string RequestNo { get; set; }

        [StringLength(200)]
        public string Project { get; set; }

        [StringLength(50)]
        public string BACode { get; set; } // Used to match the Matrix 'ActionType'

        public DateTime Date { get; set; }

        public string Justification { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        // --- APPROVAL ROUTING ---
        public decimal NetBookValue { get; set; }
        public int CurrentApprovalLevel { get; set; }

        // --- AUDIT FIELDS ---
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}