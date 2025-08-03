using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models.ViewModels
{
    public class FormBudgetListViewModel
    {
        public Guid BudgetId { get; set; }
        public string Amount { get; set; }
        public string Type { get; set; }
    }
}