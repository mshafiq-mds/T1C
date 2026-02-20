using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
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
                // Optional: set a default filter if needed
                // ddlStatusFilter.SelectedValue = ""; 
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
                var query = db.TransfersTransaction.AsQueryable();

                if (!string.IsNullOrEmpty(ba))
                {
                    query = query.Where(x => x.BA == ba);
                }

                // Execute query and project
                var transfersList = query
                    .OrderByDescending(x => x.Date)
                    .ToList() // Materialize to memory to perform text transformation
                    .Select(x => new
                    {
                        x.BA,
                        x.Id,
                        x.RefNo,
                        x.Project,
                        x.Date,
                        x.EstimatedCost,
                        x.NextApprover,
                        // Update status logic: 
                        // If DeletedDate is present -> "Deleted"
                        // Else -> Use the string status directly from DB ("sentback", "Submitted", etc.)
                        Status = x.DeletedDate != null ? "Deleted" : (x.status ?? "Unknown")
                    })
                    .Where(x => statusFilter == "" || x.Status == statusFilter)
                    .OrderByDescending(x => x.Date)
                    .ThenByDescending(x => x.RefNo)
                    .ToList();

                gvTransfers.DataSource = transfersList;
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
                string roleCode = Auth.User().CCMSRoleCode;
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
                    record.status = "Deleted";


                    // 5. DELETE TRANSACTIONS DATA
                    if (record.NewBudgetId.HasValue)
                    {
                        var linkedTransactions = db.Transactions
                            .Where(t => t.ToId == record.NewBudgetId.Value)
                            .ToList();

                        if (linkedTransactions.Any())
                        {
                            //db.Transactions.RemoveRange(linkedTransactions);
                            // Option B: Soft Delete (If your Transactions table has these columns)
                            foreach (var trans in linkedTransactions)
                            {
                                trans.DeletedDate = DateTime.Now;
                                trans.DeletedBy = userId;
                            }
                        }
                    } 

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