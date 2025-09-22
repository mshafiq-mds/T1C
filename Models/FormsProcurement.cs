using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models
{
    [Table("FormsProcurement")]
    public class FormsProcurement : BaseModel
    { 
        public Guid? EntityId { get; set; }
        public Guid? TypeId { get; set; }
        public Guid? BizAreaId { get; set; }

        [StringLength(100)]
        public string BizAreaCode { get; set; }

        [StringLength(255)]
        public string BizAreaName { get; set; }
        public DateTime? Date { get; set; }
        [StringLength(100)]
        public string Ref { get; set; }
        public string Details { get; set; }
        public Guid? PurchaseType { get; set; }
        public string JustificationOfNeed { get; set; }
        public string Remarks { get; set; }
        public decimal? Amount { get; set; }
        public string ProcurementType { get; set; }

        [StringLength(50)]
        public string Status { get; set; }
        public decimal? ActualAmount { get; set; }
    }
} 