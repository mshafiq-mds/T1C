using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models
{
    public class FormBudget
    {
        [Key, Column(Order = 0)]
        public Guid FormId { get; set; }

        [Key, Column(Order = 1)]
        public Guid BudgetId { get; set; }

        public decimal? Amount { get; set; }

        [ForeignKey("FormId")]
        public virtual Form Form { get; set; }

        [ForeignKey("BudgetId")]
        public virtual Budget Budget { get; set; }
    }
}