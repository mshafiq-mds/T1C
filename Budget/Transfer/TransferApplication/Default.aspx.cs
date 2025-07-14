using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.Transfer.TransferApplication
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
                    ? db.TransferApprovalLimits
                        .Where(m => m.TransApproverCode == userRole && m.DeletedDate == null)
                        .ToList()
                    : new List<TransferApprovalLimits>();

                // Query base transfers
                var query = db.TransfersTransaction.AsQueryable();
                //.Where(x => x.DeletedDate == null);

                if (!string.IsNullOrEmpty(ba))
                {
                    // If BA is defined, filter by accessible BAs (or user's BA if none found)
                    if (accessibleBizAreas.Count > 0)
                        query = query.Where(x => accessibleBizAreas.Contains(x.FromBA));
                    else
                        query = query.Where(x => x.FromBA == ba);
                }
                // else: ba == null → no filtering, access all

                var transfers = query
                    .OrderByDescending(x => x.Date)
                    .ToList()
                    .Select(x =>
                    {   
                        bool CanEdit = false;
                        return new
                        {
                            x.BA,
                            x.Id,
                            x.RefNo,
                            x.Project,
                            x.Date,
                            x.EstimatedCost,
                            Status =
                                        x.DeletedDate != null ? "Deleted" :
                                        x.status == 0 ? "Resubmit" :
                                        x.status == 1 ? "Submitted" :
                                        x.status == 2 ? "Under Review" :
                                        x.status == 3 ? "Completed" :
                                        x.status == 4 ? "Finalized" :
                                        "Unknown",
                            CanEdit =
                                        x.status == 3 ? true :
                                        false,
                        };
                    })
                    .Where(x =>
                                statusFilter == "All" ||
                                (statusFilter == "EditableOnly" && x.CanEdit) ||
                                x.Status == statusFilter
                            )
                    .OrderByDescending(x => x.RefNo)
                    .ToList();

                gvTransfers.DataSource = transfers;
                gvTransfers.DataBind();
            }
        }

        protected void gvList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTransfers.PageIndex = e.NewPageIndex;
            string selectedStatus = ddlStatusFilter.SelectedValue;
            BindTransfers(selectedStatus);
        }
    }
}
