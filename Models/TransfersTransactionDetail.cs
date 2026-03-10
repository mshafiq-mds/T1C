using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prodata.WebForm.Models
{
    [Table("TransfersTransactionDetails")]
    public class TransfersTransactionDetail
    {
        [Key]
        public Guid Id { get; set; }

        public Guid TransferId { get; set; }

        [StringLength(50)]
        public string FromGL { get; set; }

        public Guid? FromBudgetType { get; set; }
        public decimal FromBudget { get; set; }
        public decimal FromBalance { get; set; }
        public decimal FromTransfer { get; set; }
        public decimal FromAfter { get; set; }

        // Navigation Property (Optional but good practice)
        [ForeignKey("TransferId")]
        public virtual TransfersTransaction TransfersTransaction { get; set; }
    }
}