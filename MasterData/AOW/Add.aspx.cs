using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;

namespace Prodata.WebForm.MasterData.AOW
{
    public partial class Add : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControl();
            }
        }

        private void BindControl()
        {
            // Fetch roles dynamically from your existing IPMSRole class
            ddlRoleCode.DataSource = new Class.IPMSRole().GetIPMSRoles();
            ddlRoleCode.DataValueField = "Code";
            ddlRoleCode.DataTextField = "DisplayName";
            ddlRoleCode.DataBind();
            ddlRoleCode.Items.Insert(0, new ListItem("", "")); // Empty default for select2
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Backend validation
            if (string.IsNullOrWhiteSpace(txtMinAmount.Text) ||
                string.IsNullOrWhiteSpace(txtLevel.Text) ||
                string.IsNullOrWhiteSpace(ddlRoleCode.SelectedValue) ||
                string.IsNullOrWhiteSpace(txtActionType.Text))
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Warning, "Please fill in all required fields.");
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    // Create new Entity Model
                    var newLimit = new Prodata.WebForm.Models.ModelAWO.AssetWriteOffApprovalLimit
                    {
                        Id = Guid.NewGuid(),
                        AmountMin = Convert.ToDecimal(txtMinAmount.Text.Trim()),
                        AmountMax = string.IsNullOrWhiteSpace(txtMaxAmount.Text) ? (decimal?)null : Convert.ToDecimal(txtMaxAmount.Text.Trim()),
                        Order = Convert.ToInt32(txtLevel.Text.Trim()),
                        AWOApproverCode = ddlRoleCode.SelectedValue,
                        Section = txtActionType.Text,
                        CreatedBy = Auth.User().Id,
                        CreatedDate = DateTime.Now
                    };

                    // Add to database and save
                    db.AssetWriteOffApprovalLimits.Add(newLimit);
                    db.SaveChanges();

                    // Success alert and redirect 
                    Response.Redirect("~/MasterData/AOW/Default.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error saving record: " + ex.Message);
            }
        }
    }
}