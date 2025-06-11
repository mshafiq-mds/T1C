using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models
{
    public class AdditionalLoaFinanceLimits : BaseModel
    {
        public Guid? EntityId { get; set; }

        public Guid? ObjectId { get; set; }

        [StringLength(100)]
        public string ObjectType { get; set; }

        public Guid? FinanceApproverId { get; set; }

        [StringLength(100)]
        public string FinanceApproverType { get; set; }

        [StringLength(50)]
        public string FinanceApproverCode { get; set; }

        [StringLength(255)]
        public string FinanceApproverName { get; set; }

        public decimal? AmountMin { get; set; }

        public decimal? AmountMax { get; set; }

        [StringLength(255)]
        public string Section { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public int? Order { get; set; }

        public AdditionalLoaFinanceLimits()
        {
            Id = Guid.NewGuid();
            CreatedBy = Prodata.WebForm.Auth.Id();
            CreatedDate = DateTime.Now;
            UpdatedBy = Prodata.WebForm.Auth.Id();
            UpdatedDate = DateTime.Now;
        }
    }
}