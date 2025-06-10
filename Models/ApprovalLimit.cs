using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models
{
    public class ApprovalLimit : BaseModel
    {
        public Guid? EntityId { get; set; }

        public Guid? ObjectId { get; set; }

        [StringLength(100)]
        public string ObjectType { get; set; }

        public Guid? ApproverId { get; set; }

        [StringLength(100)]
        public string ApproverType { get; set; }

        [StringLength(50)]
        public string ApproverCode { get; set; }

        [StringLength(255)]
        public string ApproverName { get; set; }

        public decimal? AmountMin { get; set; }

        public decimal? AmountMax { get; set; }

        [StringLength(255)]
        public string Section { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public int? Order { get; set; }

        public ApprovalLimit()
        {
            Id = Guid.NewGuid();
            CreatedBy = Prodata.WebForm.Auth.Id();
            CreatedDate = DateTime.Now;
            UpdatedBy = Prodata.WebForm.Auth.Id();
            UpdatedDate = DateTime.Now;
        }
    }
}