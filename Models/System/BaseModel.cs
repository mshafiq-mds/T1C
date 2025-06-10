using Prodata.WebForm.Models.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models
{
	public class BaseModel
	{
		[Key]
		public Guid Id { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User Creater { get; set; }
        public Guid? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [ForeignKey("UpdatedBy")]
        public virtual User Updater { get; set; }
        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [ForeignKey("DeletedBy")]
        public virtual User Deleter { get; set; }
        public Guid? DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        public BaseModel()
        {
            Id = Guid.NewGuid();
            CreatedBy = Prodata.WebForm.Auth.Id();
            CreatedDate = DateTime.Now;
            UpdatedBy = Prodata.WebForm.Auth.Id();
            UpdatedDate = DateTime.Now;
        }
    }
}