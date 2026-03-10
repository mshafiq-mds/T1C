using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models.ModelAWO
{
    [Table("AssetWriteOffDocuments")]
    public class AssetWriteOffDocument
    {
        [Key]
        public Guid Id { get; set; }
        public Guid WriteOffId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] FileData { get; set; }
        public Guid UploadedBy { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}