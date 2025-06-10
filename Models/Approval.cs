using Prodata.WebForm.Models.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models
{
    public class Approval
    {
        public Guid Id { get; set; }

        public Guid? ObjectId { get; set; }

        [StringLength(100)]
        public string ObjectType { get; set; }

        public Guid? ActionById { get; set; }

        [StringLength(100)]
        public string ActionByType { get; set; }

        [StringLength(100)]
        public string ActionByCode { get; set; }

        [StringLength(255)]
        public string ActionByName { get; set; }

        [StringLength(100)]
        public string Action { get; set; }

        public string Remark { get; set; }

        [StringLength(100)]
        public string Section { get; set; }

        public int? Order { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User Creater { get; set; }
        public Guid? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Approval()
        {
            Id = Guid.NewGuid();
            CreatedBy = Prodata.WebForm.Auth.Id();
            CreatedDate = DateTime.Now;
        }
    }
}