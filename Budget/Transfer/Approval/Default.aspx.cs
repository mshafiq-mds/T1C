using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.Transfer.Approval
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
        private void BindTransfers(string statusFilter = "EditableOnly")
        {
            string ba = Auth.User().iPMSBizAreaCode;
            string userRole = Auth.User().iPMSRoleCode;

            List<string> accessibleBizAreas = !string.IsNullOrEmpty(ba)
                ? new Class.IPMSBizArea().GetBizAreaCodes(ba)
                : new List<string>();

            using (var db = new AppDbContext())
            {
                var limits = !string.IsNullOrEmpty(userRole)
                    ? db.TransferApprovalLimits
                        .Where(m => m.TransApproverCode == userRole && m.DeletedDate == null)
                        .ToList()
                    : new List<TransferApprovalLimits>();

                var query = db.TransfersTransaction.AsQueryable();

                if (!string.IsNullOrEmpty(ba))
                {
                    query = accessibleBizAreas.Any()
                        ? query.Where(x => accessibleBizAreas.Contains(x.BA))
                        : query.Where(x => x.BA == ba);
                }

                var transfers = query
                    .OrderByDescending(x => x.Date)
                    .ToList()
                    .Select(x =>
                    {
                        int currentLevel = Class.Budget.GetTransferApprovalLevel(db, x.Id);
                        var matchingLimit = Class.Budget.GetMatchingTransferLimit(limits, x.FromTransfer ?? 0m);
                        int userLevel = matchingLimit?.Order ?? 0;

                        bool canEdit = Class.Budget.CanEditTransfer(matchingLimit, userLevel, currentLevel, x.DeletedDate);
                        string status = Class.Budget.GetStatusName(x.status, x.DeletedDate);

                        return new
                        {
                            x.BA,
                            x.Id,
                            x.RefNo,
                            x.Project,
                            x.Date,
                            x.EstimatedCost,
                            x.FromTransfer,
                            Status = status,
                            CanEdit = canEdit
                        };
                    })
                    .Where(x =>
                        statusFilter == "" ||
                        (statusFilter == "EditableOnly" && x.CanEdit) ||
                        x.Status == statusFilter)
                    .OrderByDescending(x => x.Date)
                    .ThenByDescending(x => x.RefNo)
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
