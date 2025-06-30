using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.Additional.Approval.Cumulative
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
        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedStatus = ddlStatusFilter.SelectedValue;
            BindTransfers(selectedStatus);
        }
        private void BindTransfers(string statusFilter = "All")
        {
            string ba = Auth.User().iPMSBizAreaCode;
            string userRole = Auth.User().iPMSRoleCode;

            List<string> accessibleBizAreas = !string.IsNullOrEmpty(ba)
                ? new Class.IPMSBizArea().GetBizAreaCodes(ba)
                : new List<string>();

            using (var db = new AppDbContext())
            {
                // Load all active cumulative limits ordered by level (Order)
                var limits = db.AdditionalCumulativeLimits
                    .Where(m => m.DeletedDate == null)
                    .OrderBy(m => m.Order)
                    .ToList();

                var query = db.AdditionalBudgetRequests.AsQueryable();

                if (!string.IsNullOrEmpty(ba))
                {
                    if (accessibleBizAreas.Count > 0)
                        query = query.Where(x => accessibleBizAreas.Contains(x.BA));
                    else
                        query = query.Where(x => x.BA == ba);
                }

                var transfers = query
                    .OrderByDescending(x => x.ApplicationDate)
                    .ToList()
                    .Select(x =>
                    {
                        int currentLevelApproval = db.AdditionalBudgetLog
                            .Where(w => w.BudgetTransferId == x.Id)
                            .OrderByDescending(w => w.CreatedDate)
                            .Select(w => w.StepNumber)
                            .FirstOrDefault();

                        // Determine the correct approver based on cumulative limits
                        var eligibleLimit = limits
                            .Where(l => l.AmountCumulativeBalance.GetValueOrDefault() >= x.AdditionalBudget)
                            .OrderBy(l => l.Order)
                            .FirstOrDefault();

                        bool canEdit = (x.DeletedDate == null &&
                            eligibleLimit != null &&
                            eligibleLimit.CumulativeApproverCode == userRole &&
                            eligibleLimit.Order == currentLevelApproval + 1);

                        return new
                        {
                            x.BA,
                            x.Id,
                            x.RefNo,
                            x.Project,
                            x.ApplicationDate,
                            x.AdditionalBudget,
                            Status =
                                x.DeletedDate != null ? "Deleted" :
                                x.Status == 0 ? "Resubmit" :
                                x.Status == 1 ? "Submitted" :
                                x.Status == 2 ? "Under Review" :
                                x.Status == 3 ? "Completed" :
                                x.Status == 4 ? "Finalized" :
                                "Unknown",
                            CanEdit = canEdit
                        };
                    })
                    .Where(x =>
                                statusFilter == "All" ||
                                (statusFilter == "EditableOnly" && x.CanEdit) ||
                                x.Status == statusFilter
                          )
                    .ToList();

                gvAdditionalBudgetList.DataSource = transfers;
                gvAdditionalBudgetList.DataBind();
            }
        }


    }
}
