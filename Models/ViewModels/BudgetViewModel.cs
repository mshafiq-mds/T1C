using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models.ViewModels
{
	public class BudgetListViewModel
	{
		public Guid Id { get; set; }
		public int No { get; set; }
        public string Type { get; set; }
		public string BizAreaCode { get; set; }
		public string BizAreaName { get; set; }
		public string Date { get; set; }
		public string Month { get; set; }
        public string Ref { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Details { get; set; }
		public string Wages { get; set; }
        public string Purchase { get; set; }
        public string Amount { get; set; }
        public string Vendor { get; set; }
        public string Status { get; set; }
    }
}