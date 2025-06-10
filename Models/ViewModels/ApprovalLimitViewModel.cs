using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models.ViewModels
{
    public class ApprovalLimitListViewModel
    {
        public Guid Id { get; set; }
        public string ApproverType { get; set; }
        public string ApproverCode { get; set; }
        public string ApproverName { get; set; }
        public string AmountMin { get; set; }
        public string AmountMax { get; set; }
        public string Section {  get; set; }
        public string Status { get; set; }
        public string Order { get; set; }
    }
}