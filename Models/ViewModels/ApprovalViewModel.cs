using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Models.ViewModels
{
    public class ApprovalListViewModel
    {
        public string ActionByName { get; set; }
        public string ActionByRole { get; set; }
        public string Action { get; set; }
        public string Datetime { get; set; }
    }
}