using Prodata.WebForm.Models.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models
{
    public class Attachment
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? EntityId { get; set; }

        public Guid? ObjectId { get; set; }

        [StringLength(100)]
        public string ObjectType { get; set; }

        [StringLength(100)]
        public string Type { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }

        [StringLength(100)]
        public string ContentType { get; set; }

        public byte[] Content { get; set; }

        [StringLength(20)]
        public string Ext { get; set; }

        public long? Size { get; set; }

        [StringLength(500)]
        public string Path { get; set; }

        [ForeignKey("UploadedBy")]
        public virtual User Uploader { get; set; }
        public Guid? UploadedBy { get; set; }

        public DateTime? UploadedDate { get; set; }

        public Attachment()
        {
            Id = Guid.NewGuid();
            UploadedBy = Prodata.WebForm.Auth.Id();
            UploadedDate = DateTime.Now;
        }
    }
}