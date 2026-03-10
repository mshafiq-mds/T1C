using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models
{
    [Table("EmailLog")]
    public class EmailLog
    {
        [Key] // This tells Entity Framework that LogID is the primary key
        public int LogID { get; set; }
        public string RecipientEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; } 
        public DateTime CreatedDate { get; set; }
    }
}