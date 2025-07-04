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
                // Load all active cumulative limits
                var limits = db.AdditionalCumulativeLimits
                    .Where(m => m.DeletedDate == null)
                    .OrderBy(m => m.Order)
                    .ToList();

                var query = db.AdditionalBudgetRequests.AsQueryable();

                if (!string.IsNullOrEmpty(ba))
                {
                    query = accessibleBizAreas.Any()
                        ? query.Where(x => accessibleBizAreas.Contains(x.BA))
                        : query.Where(x => x.BA == ba);
                }

                var transfers = query
                    .OrderByDescending(x => x.ApplicationDate)
                    .ToList()
                    .Select(x =>
                    {
                        int currentLevel = db.AdditionalBudgetLog
                            .Where(w => w.BudgetTransferId == x.Id)
                            .OrderByDescending(w => w.CreatedDate)
                            .Select(w => w.StepNumber)
                            .FirstOrDefault();

                        var eligibleLimit = Class.Budget.GetEligibleCumulativeLimit(limits, x.AdditionalBudget);
                        bool canEdit = Class.Budget.CanEditCumulativeRequest(eligibleLimit, userRole, currentLevel, x.DeletedDate);
                        string status = Class.Budget.GetStatusName(x.Status, x.DeletedDate);

                        return new
                        {
                            x.BA,
                            x.Id,
                            x.RefNo,
                            x.Project,
                            x.ApplicationDate,
                            x.AdditionalBudget,
                            Status = status,
                            CanEdit = canEdit
                        };
                    })
                    .Where(x =>
                        statusFilter == "All" ||
                        (statusFilter == "EditableOnly" && x.CanEdit) ||
                        x.Status == statusFilter)
                    .ToList();

                gvAdditionalBudgetList.DataSource = transfers;
                gvAdditionalBudgetList.DataBind();
            }
        }

    }
}
