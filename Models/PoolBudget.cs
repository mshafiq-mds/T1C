using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models
{
    [Table("PoolBudgets")]
    public class PoolBudgets : BaseModel
    {
        [Required]
        public int Year { get; set; }

        public Guid BudgetTypeId { get; set; }

        public decimal Amount { get; set; } // Total Allocation

        public string Description { get; set; }

        public bool IsActive { get; set; }

        //public Guid CreatedBy { get; set; }

        //public DateTime CreatedDate { get; set; }

        //public Guid? UpdatedBy { get; set; }

        //public DateTime? UpdatedDate { get; set; }

        //public Guid? DeletedBy { get; set; }

        //public DateTime? DeletedDate { get; set; }
    }
}