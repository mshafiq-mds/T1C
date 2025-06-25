using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.Additional.Approval.COGS
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

            // Handle null gracefully: null ba = access all, null role = no approval limit filter
            List<string> accessibleBizAreas = !string.IsNullOrEmpty(ba)
                ? new Class.IPMSBizArea().GetBizAreaCodes(ba)
                : new List<string>(); // Empty list means access all later
            //List<string> accessibleBizAreas = Auth.IPMSBizAreaCodes();

            using (var db = new AppDbContext())
            {
                // Get approval limits only if userRole is not null
                var limits = !string.IsNullOrEmpty(userRole)
                    ? db.AdditionalLoaCogsLimits
                        .Where(m => m.CogsApproverCode == userRole && m.DeletedDate == null)
                        .ToList()
                    : new List<AdditionalLoaCogsLimits>();

                // Query base transfers
                var query = db.AdditionalBudgetRequests
                              .Where(x => x.CheckType == "COGS");

                if (!string.IsNullOrEmpty(ba))
                {
                    // If BA is defined, filter by accessible BAs (or user's BA if none found)
                    if (accessibleBizAreas.Count > 0)
                        query = query.Where(x => accessibleBizAreas.Contains(x.BA));
                    else
                        query = query.Where(x => x.BA == ba);
                }
                // else: ba == null → no filtering, access all

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

                        var matchingLimit = limits.FirstOrDefault(l =>
                            l.AmountMin <= x.EstimatedCost &&
                            (l.AmountMax == null || l.AmountMax >= x.EstimatedCost));

                        int userLevelApproval = matchingLimit?.Order ?? 0;

                        bool canEdit = (x.DeletedDate == null && matchingLimit != null && userLevelApproval == currentLevelApproval + 1);//|| Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "admin-user-edit");

                        return new
                        {
                            x.BA,
                            x.Id,
                            x.RefNo,
                            x.Project,
                            x.ApplicationDate,
                            x.EstimatedCost,
                            Status =
                                        x.DeletedDate != null ? "Deleted" : 
                                        x.Status == 0 ? "Resubmit" :
                                        //x.status == 1 and null ? "Submitted" :
                                        x.Status == 2 ? "Under Review" :
                                        x.Status == 3 ? "Completed" :
                                        "Submitted",
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
