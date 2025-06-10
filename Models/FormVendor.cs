using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models
{
    public class FormVendor
    {
        [Key, Column(Order = 0)]
        public Guid FormId { get; set; }

        [Key, Column(Order = 1)]
        public Guid VendorId { get; set; }

        [ForeignKey("FormId")]
        public virtual Form Form { get; set; }

        [ForeignKey("VendorId")]
        public virtual Vendor Vendor { get; set; }
    }
}