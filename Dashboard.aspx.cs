using FGV.Prodata.Web.UI;
using NPOI.POIFS.Crypt;
using NPOI.XWPF.UserModel;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;

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
                    LblAdditionalFinalized.Text = db.AdditionalBudgetRequests.Count(x => x.Status == 4).ToString();
                    LblAdditionalComplete.Text = db.AdditionalBudgetRequests.Count(x => x.Status == 3).ToString();
                    LblAdditionalReview.Text = db.AdditionalBudgetRequests.Count(x => x.Status == 2).ToString();
                    LblAdditionalResubmit.Text = db.AdditionalBudgetRequests.Count(x => x.Status == 0).ToString();
                    //LblAdditionalSubmitted.Text = db.AdditionalBudgetRequests.Count(x => x.Status == null).ToString();
                    LblAdditionalSubmitted.Text = db.AdditionalBudgetRequests.Count(x => x.DeletedDate == null).ToString();

                    // Transfers
                    LblTransferDeleted.Text = db.TransfersTransaction.Count(x => x.DeletedDate != null).ToString();
                    LblTransferFinalized.Text = db.TransfersTransaction.Count(x => x.status == 4).ToString();
                    LblTransferComplete.Text = db.TransfersTransaction.Count(x => x.status == 3).ToString();
                    LblTransferReview.Text = db.TransfersTransaction.Count(x => x.status == 2).ToString();
                    LblTransferResubmit.Text = db.TransfersTransaction.Count(x => x.status == 0).ToString();
                    //LblTransferSubmitted.Text = db.TransfersTransaction.Count(x => x.status == null).ToString();
                    LblTransferSubmitted.Text = db.TransfersTransaction.Count(x => x.DeletedDate == null).ToString();

                    // Forms
                    //LblT1CTotal.Text = db.Forms.Count(x => x.DeletedDate == null).ToString();
                    LblT1CDeleted.Text = db.Forms.Count(x => x.DeletedDate != null).ToString();
                    LblT1CComplete.Text = db.Forms.Count(x => x.Status == "Approved").ToString();
                    LblT1CReview.Text = db.Forms.Count(x => x.Status == "Pending").ToString();
                    LblT1CResubmit.Text = db.Forms.Count(x => x.Status == "SentBack").ToString();
                    LblT1CSubmitted.Text = db.Forms.Count(x => x.DeletedDate == null).ToString();
                }
                catch (Exception ex)
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, ex.Message);
                }
            }
        }
    }
}
