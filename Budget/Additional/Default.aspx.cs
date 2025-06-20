﻿using FGV.Prodata.Web.UI;
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
        private void BindTransfers(string statusFilter = "All")
        {
            string ba = Auth.User().iPMSBizAreaCode;

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
                    .Where(x => statusFilter == "All" || x.Status == statusFilter)
                    .ToList();

                gvBudgetList.DataSource = transfers;
                gvBudgetList.DataBind();
            }
        }
        protected void btnDeleteConfirmed_Click(object sender, EventArgs e)
        {
            try
            {
                Guid id = Guid.Parse(hdnDeleteId.Value);

                using (var db = new AppDbContext())
                {
                    var record = db.AdditionalBudgetRequests.Find(id);
                    if (record != null)
                    {
                        record.DeletedBy = Auth.Id();
                        record.DeletedDate = DateTime.Now;

                        db.SaveChanges();
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Info, "Record deleted successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error deleting record: " + ex.Message);
            }

            BindTransfers();
        }
    }
     
}
