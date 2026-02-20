using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Prodata.WebForm.Models.System;

namespace Prodata.WebForm.Models.Auth
{
    [Table("UserRoles")]
    public class UserRole
    {
        [Key]
        [Column(Order = 1)]
        public Guid UserId { get; set; }

        [Key]
        [Column(Order = 2)]
        public Guid RoleId { get; set; }
    }
}