using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models.ViewModels
{
    public class TransferApprovalLimitListViewModel
    {
        public Guid Id { get; set; }
        public string TransApproverType { get; set; }
        public string TransApproverCode { get; set; }
        public string TransApproverName { get; set; }
        public string AmountMin { get; set; }
        public string AmountMax { get; set; }
        public string Section { get; set; }
        public string Status { get; set; }
        public string Order { get; set; }
    }
}