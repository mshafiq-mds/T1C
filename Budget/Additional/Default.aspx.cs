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

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedStatus = ddlStatusFilter.SelectedValue;
            BindTransfers(selectedStatus);
        }
        private void BindTransfers(string statusFilter = "")
        {
            string ba = Auth.User().CCMSBizAreaCode;

            using (var db = new AppDbContext())
            {
                var query = db.AdditionalBudgetRequests.AsQueryable();
                              //.Where(x => x.DeletedDate == null);

                // If user has a specific BizAreaCode, filter by it
                if (!string.IsNullOrEmpty(ba))
                {
                    query = query.Where(x => x.BA == ba);
                }

                var transfers = query
                    .OrderByDescending(x => x.CreatedDate)
                    .Select(x => new
                    {
                        x.Id,
                        x.RefNo,
                        x.CreatedDate,
                        x.BA,
                        x.Project,
                        x.EstimatedCost, 
                        Status =
                            x.DeletedDate != null ? "Deleted" :
                            x.Status == 0 ? "Resubmit" :
                            x.Status == 2 ? "Under Review" :
                            x.Status == 3 ? "Completed" :
                            "Submitted"
                    })
                    .Where(x => statusFilter == "" || x.Status == statusFilter)
                    .OrderByDescending(x => x.CreatedDate)
                    .ThenByDescending(x => x.RefNo)
                    .ToList();

                gvBudgetList.DataSource = transfers;
                gvBudgetList.DataBind();
            }
        }
        protected void gvList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBudgetList.PageIndex = e.NewPageIndex;
            string selectedStatus = ddlStatusFilter.SelectedValue;
            BindTransfers(selectedStatus);
        }
        protected void btnDeleteConfirmed_Click(object sender, EventArgs e)
        {
            try
            {
                Guid id = Guid.Parse(hdnDeleteId.Value);
                Guid userId = Auth.Id();
                string roleCode = Auth.User().CCMSRoleCode;
                string remarks = hdnDeleteRemarks.Value;

                using (var db = new AppDbContext())
                {
                    var record = db.AdditionalBudgetRequests.Find(id);

                    if (record == null)
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Warning, "Record not found.");
                        return;
                    }

                    db.AdditionalBudgetLog.Add(new AdditionalBudgetLog
                    {
                        BudgetTransferId = id,
                        StepNumber = 100,
                        RoleName = roleCode,
                        UserId = userId,
                        ActionType = "Delete",
                        ActionDate = DateTime.Now,
                        Status = "Delete",
                        Remarks = remarks
                    });

                    record.DeletedBy = userId;
                    record.DeletedDate = DateTime.Now;

                    db.SaveChanges();

                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Info, "Record deleted successfully.");
                }
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "An error occurred while deleting the record.");
            }

            BindTransfers();
        }
    }
     
}
