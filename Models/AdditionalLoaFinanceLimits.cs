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

        public Guid? TransApproverId { get; set; }

        [StringLength(100)]
        public string TransApproverType { get; set; }

        [StringLength(50)]
        public string TransApproverCode { get; set; }

        [StringLength(255)]
        public string TransApproverName { get; set; }

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