using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models
{
    [Table("BudgetsDetails")]
    public class BudgetsDetails : BaseModel
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? TypeId { get; set; }
        public Guid? FromId { get; set; }

        [StringLength(255)]
        public string ProjectName { get; set; }

        [StringLength(100)]
        public string BA { get; set; }

         
        public decimal? TotalAmount { get; set; }

         
        public decimal? Jan { get; set; }

         
        public decimal? Feb { get; set; }

         
        public decimal? Mar { get; set; }

         
        public decimal? Apr { get; set; }

         
        public decimal? May { get; set; }

         
        public decimal? Jun { get; set; }

         
        public decimal? Jul { get; set; }

         
        public decimal? Aug { get; set; }

         
        public decimal? Sep { get; set; }

         
        public decimal? Oct { get; set; }

         
        public decimal? Nov { get; set; }

         
        public decimal? Dec { get; set; }

        //public Guid? CreatedBy { get; set; }
        //public DateTime? CreatedDate { get; set; }

        //public Guid? UpdatedBy { get; set; }
        //public DateTime? UpdatedDate { get; set; }

        //public Guid? DeletedBy { get; set; }
        //public DateTime? DeletedDate { get; set; }
    }
}