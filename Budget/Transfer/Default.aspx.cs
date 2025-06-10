using System;
using System.Linq;
using System.Web.UI;
using Prodata.WebForm.Models; // Your model namespace
using FGV.Prodata.App;
using FGV.Prodata.Web.UI;

namespace Prodata.WebForm.Budget.Transfer
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

        private void BindTransfers()
        {
            string ba = Auth.User().iPMSBizAreaCode;

            using (var db = new AppDbContext())
            {
                var query = db.TransfersTransaction
                              .Where(x => x.DeletedDate == null);

                // If user has a specific BizAreaCode, filter by it
                if (!string.IsNullOrEmpty(ba))
                {
                    query = query.Where(x => x.BA == ba);
                }

                var transfers = query
                    .OrderByDescending(x => x.Date)
                    .Select(x => new
                    {
                        x.BA,
                        x.Id,
                        x.RefNo,
                        x.Project,
                        x.Date,
                        x.EstimatedCost,
                        Status =
                            x.status == 0 ? "Resubmit" :
                            //x.status == 1 ? "Submitted" :
                            x.status == 2 ? "Under Review" :
                            x.status == 3 ? "Completed" :
                            "Submitted"
                    })
                    .ToList();

                gvTransfers.DataSource = transfers;
                gvTransfers.DataBind();
            }
        }


        protected void btnDeleteConfirmed_Click(object sender, EventArgs e)
        {
            try
            {
                Guid id = Guid.Parse(hdnDeleteId.Value);

                using (var db = new AppDbContext())
                {
                    var record = db.TransfersTransaction.Find(id);
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
