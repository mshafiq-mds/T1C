using CustomGuid.AspNet.Identity; // Required for Auth
using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.MasterData.AdditionalBudgetApprover
{
    public partial class CumAdd : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControl();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                bool isSuccess = false;

                // Parse inputs
                decimal amountCumulative = !string.IsNullOrEmpty(txtMinValue.Text.Trim()) ? decimal.Parse(txtMinValue.Text.Trim().Replace(",", "")) : 0;
                decimal? amountMax = !string.IsNullOrEmpty(txtMaxValue.Text.Trim()) ? decimal.Parse(txtMaxValue.Text.Trim().Replace(",", "")) : (decimal?)null;
                string section = txtSection.Text.Trim();
                int order = !string.IsNullOrEmpty(txtOrder.Text.Trim()) ? int.Parse(txtOrder.Text.Trim()) : 0;
                string roleCode = ddlIPMSRole.SelectedValue;
                string roleName = ddlIPMSRole.SelectedItem.Text;

                Guid id = Auth.User().Id;

                using (var db = new AppDbContext())
                {
                    try
                    {
                        var newLimit = new AdditionalCumulativeLimits
                        {
                            Id = Guid.NewGuid(),
                            CumulativeApproverType = "iPMS_Role",
                            CumulativeApproverCode = roleCode,
                            CumulativeApproverName = roleName,
                            AmountCumulative = amountCumulative, // Mapping Min Value to Cumulative Threshold
                            AmountMax = amountMax,
                            Section = section,
                            Order = order,
                            Status = "Active"//,
                            // Fix: Explicitly set Created fields to prevent DbContext errors
                            //CreatedBy = id,
                            //CreatedDate = DateTime.Now
                        };

                        db.AdditionalCumulativeLimits.Add(newLimit);
                        db.SaveChanges();
                        isSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
                    }
                }

                if (isSuccess)
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Cumulative approver added.");
                    Response.Redirect("~/MasterData/AdditionalBudgetApprover/Default");
                }
            }
        }

        private void BindControl()
        {
            // Bind Role Dropdown
            var roles = new Class.IPMSRole().GetIPMSRoles();
            ddlIPMSRole.DataSource = roles;
            ddlIPMSRole.DataValueField = "Code";
            ddlIPMSRole.DataTextField = "DisplayName";
            ddlIPMSRole.DataBind();
            ddlIPMSRole.Items.Insert(0, new ListItem("", ""));
        }
    }
}