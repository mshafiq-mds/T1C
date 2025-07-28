using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models; // Your model namespace
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.Transfer
{
    public partial class Default : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlStatusFilter.SelectedValue = "All"; // Optional: Set default value
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
            string ba = Auth.User().iPMSBizAreaCode;

            using (var db = new AppDbContext())
            {
                var query = db.TransfersTransaction.AsQueryable();

                if (!string.IsNullOrEmpty(ba))
                {
                    query = query.Where(x => x.BA == ba);
                }

                var transfers = query
                    .OrderByDescending(x => x.Date)
                    .ToList()
                    .Select(x => new
                    {
                        x.BA,
                        x.Id,
                        x.RefNo,
                        x.Project,
                        x.Date,
                        x.EstimatedCost,
                        Status = Class.Budget.GetStatusName(x.status, x.DeletedDate)
                    })
                    .Where(x => statusFilter == "" || x.Status == statusFilter)
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
        protected void btnDeleteConfirmed_Click(object sender, EventArgs e)
        {
            try
            {
                Guid id = Guid.Parse(hdnDeleteId.Value);
                Guid userId = Auth.Id();
                string roleCode = Auth.User().iPMSRoleCode;
                string remarks = hdnDeleteRemarks.Value;

                using (var db = new AppDbContext())
                {
                    var record = db.TransfersTransaction.Find(id);

                    if (record == null)
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Warning, "Record not found.");
                        return;
                    }

                    db.TransferApprovalLog.Add(new TransferApprovalLog
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
