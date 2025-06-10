using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.AddBudget
{
    public partial class Default : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindTransfers();
            }
        }
        private void BindTransfers()
        {
            string ba = Auth.User().iPMSBizAreaCode;

            using (var db = new AppDbContext())
            {
                var query = db.AdditionalBudgetRequests
                              .Where(x => x.DeletedDate == null);

                // If user has a specific BizAreaCode, filter by it
                if (!string.IsNullOrEmpty(ba))
                {
                    query = query.Where(x => x.BA == ba);
                }

                var transfers = query
                    .OrderByDescending(x => x.CreatedDate)
                    .Select(x => new
                    {
                        x.BA,
                        x.Id,
                        x.RefNo,
                        x.Project,
                        x.CreatedDate,
                        x.EstimatedCost,
                        Status =
                            x.Status == 0 ? "Resubmit" :
                            //x.status == 1 ? "Submitted" :
                            x.Status == 2 ? "Under Review" :
                            x.Status == 3 ? "Completed" :
                            "Submitted"
                    })
                    .ToList();

                gvBudgetList.DataSource = transfers;
                gvBudgetList.DataBind();
            }
        }
    }

    // Temporary model for testing
    
}
