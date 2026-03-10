using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prodata.WebForm.Models.ModelAWO
{
    [Table("AssetWriteOffApprovalLog")]
    public class AssetWriteOffApprovalLog
    {
        [Key]
        public Guid Id { get; set; }

        public Guid WriteOffId { get; set; }

        public int StepNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string RoleName { get; set; }

        public Guid? UserId { get; set; }

        [Required]
        [StringLength(20)]
        public string ActionType { get; set; }

        public DateTime? ActionDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        public string Remarks { get; set; }

        public Guid? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}