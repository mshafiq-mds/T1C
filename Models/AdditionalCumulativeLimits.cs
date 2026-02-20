using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models
{
    [Table("AdditionalCumulativeLimits")]
    public class AdditionalCumulativeLimits : BaseModel
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? EntityId { get; set; }

        public Guid? ObjectId { get; set; }

        [StringLength(200)]
        public string ObjectType { get; set; }

        public Guid? CumulativeApproverId { get; set; }

        [StringLength(100)]
        public string CumulativeApproverType { get; set; }

        [StringLength(100)]
        public string CumulativeApproverCode { get; set; }

        [StringLength(300)]
        public string CumulativeApproverName { get; set; }

        public decimal? AmountMax { get; set; }

        public decimal? AmountCumulative { get; set; }
        public decimal? AmountCumulativeBalance { get; set; }

        [StringLength(100)]
        public string Section { get; set; }

        [StringLength(100)]
        public string Status { get; set; }

        public int Order { get; set; }
        public AdditionalCumulativeLimits()
        {
            Id = Guid.NewGuid();
            CreatedBy = Prodata.WebForm.Auth.Id();
            CreatedDate = DateTime.Now;
            UpdatedBy = Prodata.WebForm.Auth.Id();
            UpdatedDate = DateTime.Now;
        }
    }
}