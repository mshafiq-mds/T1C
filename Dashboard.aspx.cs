﻿using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using System;
using System.Linq;

namespace Prodata.WebForm
{
    public partial class Dashboard : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Page.ClientScript.RegisterStartupScript(this.GetType(), "BodyClass",
                "<script>document.body.classList.add('dashboard-lock');</script>");
            if (!IsPostBack)
            {
                BindControl();
            }
        }

        private void BindControl()
        {
            using (var db = new AppDbContext())
            {
                try
                {
                    // Additional Budget
                    LblAdditionalDeleted.Text = db.AdditionalBudgetRequests.Count(x => x.DeletedDate != null).ToString();
                    LblAdditionalComplete.Text = db.AdditionalBudgetRequests.Count(x => x.Status == 3).ToString();
                    LblAdditionalReview.Text = db.AdditionalBudgetRequests.Count(x => x.Status == 2).ToString();
                    LblAdditionalResubmit.Text = db.AdditionalBudgetRequests.Count(x => x.Status == 0).ToString();
                    LblAdditionalSubmitted.Text = db.AdditionalBudgetRequests.Count(x => x.Status == null).ToString();

                    // Transfers
                    LblTransferDeleted.Text = db.TransfersTransaction.Count(x => x.DeletedDate != null).ToString();
                    LblTransferComplete.Text = db.TransfersTransaction.Count(x => x.status == 3).ToString();
                    LblTransferReview.Text = db.TransfersTransaction.Count(x => x.status == 2).ToString();
                    LblTransferResubmit.Text = db.TransfersTransaction.Count(x => x.status == 0).ToString();
                    LblTransferSubmitted.Text = db.TransfersTransaction.Count(x => x.status == null).ToString();

                    // Forms
                    LblT1CTotal.Text = db.Forms.Count(x => x.DeletedDate == null).ToString();
                }
                catch (Exception ex)
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, ex.Message);
                }
            }
        }
    }
}
