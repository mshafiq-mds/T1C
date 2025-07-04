using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.Additional.Approval.Finance
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
                var limits = !string.IsNullOrEmpty(userRole)
                    ? db.AdditionalLoaFinanceLimits
                        .Where(m => m.FinanceApproverCode == userRole && m.DeletedDate == null)
                        .ToList()
                    : new List<AdditionalLoaFinanceLimits>();

                var query = db.AdditionalBudgetRequests
                              .Where(x => x.CheckType == "FINANCE");

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
                        int currentLevel = Class.Budget.GetAdditionalBudgetApprovalLevel(db, x.Id);
                        var matchingLimit = Class.Budget.GetMatchingFinanceLimit(limits, x.AdditionalBudget);
                        int userLevel = matchingLimit?.Order ?? 0;

                        bool canEdit = Class.Budget.CanEditFinanceRequest(matchingLimit, userLevel, currentLevel, x.DeletedDate);
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
