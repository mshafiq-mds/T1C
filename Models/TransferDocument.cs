using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models
{
    public class TransferDocument : BaseModel
    {
        //public Guid Id { get; set; }
        public Guid TransferId { get; set; }

        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] FileData { get; set; }

        public Guid? UploadedBy { get; set; }
        public DateTime UploadedDate { get; set; }

        public virtual TransfersTransaction Transfer { get; set; }

        // Remove these if you don't want them
        //public Guid? CreatedBy { get; set; }
        //public DateTime? CreatedDate { get; set; }
        //public Guid? UpdatedBy { get; set; }
        //public DateTime? UpdatedDate { get; set; }
        //public Guid? DeletedBy { get; set; }
        //public DateTime? DeletedDate { get; set; }

    }
}