using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Prodata.WebForm.Models.Auth;

namespace Prodata.WebForm.Models
{
    public class TransferApprovalLog : BaseModel
    {
        [Key]
        public new Guid Id { get; set; }  // Overrides BaseModel's Id with [Key]

        [Required]
        public Guid BudgetTransferId { get; set; }

        public int StepNumber { get; set; }

        [StringLength(100)]
        public string RoleName { get; set; }

        public Guid UserId { get; set; }

        [StringLength(20)]
        public string ActionType { get; set; }  // E.g., 'Reviewed', 'Endorsed', 'Approved'

        public DateTime? ActionDate { get; set; }

        [StringLength(20)]
        public string Status { get; set; }  // E.g., 'Pending', 'Approved', 'Rejected'

        public string Remarks { get; set; }

        // Optional navigation property for User if needed (not strongly typed in original)
        // [ForeignKey("UserId")]
        // public virtual User User { get; set; }

        public TransferApprovalLog()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            UpdatedDate = DateTime.Now;
        }
    }
}
