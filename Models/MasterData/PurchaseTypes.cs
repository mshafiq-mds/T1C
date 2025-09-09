using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models.MasterData
{
    public class PurchaseTypes : BaseModel
    {
        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(255)]
        public string Name { get; set; } 
    }
}