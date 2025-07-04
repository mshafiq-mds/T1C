using Prodata.WebForm.Models.MasterData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models
{
	public class Budget : BaseModel
	{
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

		[ForeignKey("TypeId")]
        public virtual BudgetType Type { get; set; }

		public virtual ICollection<FormBudget> FormBudgets { get; set; }
    }
}