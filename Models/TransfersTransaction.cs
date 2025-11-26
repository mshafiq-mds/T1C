using Prodata.WebForm.Models.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models
{
    [Table("TransfersTransaction")]
    public class TransfersTransaction : BaseModel
    {
        public Guid Id { get; set; }
        public string RefNo { get; set; }
        public string Project { get; set; }
        public DateTime Date { get; set; }
        public string BudgetType { get; set; }
        public decimal EstimatedCost { get; set; }
        public string Justification { get; set; }

        public string EVisaNo { get; set; }
        public string WorkDetails { get; set; }

        public string FromGL { get; set; }
        public Guid FromBudgetType { get; set; }
        public string FromBA { get; set; }
        public decimal? FromBudget { get; set; }
        public decimal? FromBalance { get; set; }
        public decimal? FromTransfer { get; set; }
        public decimal? FromAfter { get; set; }

        public string ToGL { get; set; }
        public Guid ToBudgetType { get; set; }
        public string ToBA { get; set; }
        public decimal? ToBudget { get; set; }
        public decimal? ToBalance { get; set; }
        public decimal? ToTransfer { get; set; }
        public decimal? ToAfter { get; set; }

        public Guid? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? status { get; set; }
        public string BA { get; set; }
    }
}