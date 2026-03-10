using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models.ModelAWO
{
    [Table("AssetWriteOffDetails")]
    public class AssetWriteOffDetail
    {
        [Key]
        public Guid Id { get; set; }

        public Guid WriteOffId { get; set; }

        [StringLength(50)]
        public string AssetCode { get; set; }

        [StringLength(200)]
        public string ItemDescription { get; set; }

        public DateTime? AcqDate { get; set; }

        public int AgeYears { get; set; }
        public int UsefulLife { get; set; }
        public int Quantity { get; set; }

        public decimal OriginalPrice { get; set; }
        public decimal AccDepreciation { get; set; }
        public decimal NetBookValue { get; set; }

        public string Reason { get; set; }
        public string Remarks { get; set; }
    }
}