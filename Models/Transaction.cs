using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models
{
    public class Transaction : BaseModel
    {
        public Guid? FromId { get; set; }

        [StringLength(50)]
        public string FromType { get; set; }

        public Guid? ToId { get; set; }

        [StringLength(50)]
        public string ToType { get; set; }

        public DateTime? Date { get; set; }

        [StringLength(100)]
        public string Ref { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public decimal? Amount { get; set; }

        [StringLength(50)]
        public string Status { get; set; }
    }
}