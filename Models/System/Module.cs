using Prodata.WebForm.Models.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models.System
{
	public class Module
	{
		[Key]
		public Guid Id { get; set; }

		[ForeignKey("ParentId")]
		public virtual Module ParentModule { get; set; }
		public Guid? ParentId { get; set; }

		[Required]
		[StringLength(255)]
		public string Name { get; set; }

        [Required]
        [StringLength(225)]
        public string Slug { get; set; }

        [StringLength(255)]
        public string Url { get; set; }

        [StringLength(50)]
        public string Icon { get; set; }

        public int Order { get; set; }

        public bool IsActive { get; set; }

        public bool IsMenu { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User Creater { get; set; }
        public Guid? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual ICollection<Module> Children { get; set; }

        public virtual ICollection<RoleModule> RoleModules { get; set; }
    }
}