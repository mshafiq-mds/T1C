using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FGV.Prodata.Web.UI;
using global::Prodata.WebForm.Models;

namespace Prodata.WebForm.MasterData.AOW
{
    public partial class Default : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();

                if (Session["SweetAlertMessage"] != null)
                {
                    string msg = Session["SweetAlertMessage"].ToString();

                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, msg);

                    Session.Remove("SweetAlertMessage");
                }

            }
        }

        private void BindGrid()
        {
            using (var db = new AppDbContext())
            {
                // Fetch from the new table and order so it visualizes the matrix flow
                var limitList = db.AssetWriteOffApprovalLimits
                                  .OrderBy(x => x.AmountMin)
                                  .ThenBy(x => x.Order)
                                  .ToList();

                gvLimit.DataSource = limitList;
                gvLimit.DataBind();
            }
        }

        protected void gvLimit_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRule")
            {
                Guid id = Guid.Parse(e.CommandArgument.ToString());

                using (var db = new AppDbContext())
                {
                    var rule = db.AssetWriteOffApprovalLimits.Find(id);
                    if (rule != null)
                    {
                        db.AssetWriteOffApprovalLimits.Remove(rule);
                        db.SaveChanges();

                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Approval limit deleted successfully.");
                        BindGrid(); // Refresh grid
                    }
                    else
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Rule not found.");
                    }
                }
            }
        }
    }
}