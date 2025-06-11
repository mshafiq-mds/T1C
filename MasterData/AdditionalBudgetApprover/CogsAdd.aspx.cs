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
    public partial class CogsAdd : ProdataPage
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
                decimal minValue = !string.IsNullOrEmpty(txtMinValue.Text.Trim()) ? decimal.Parse(txtMinValue.Text.Trim().Replace(",", "")) : 0;
                decimal? maxValue = !string.IsNullOrEmpty(txtMaxValue.Text.Trim()) ? decimal.Parse(txtMaxValue.Text.Trim().Replace(",", "")) : (decimal?)null;
                string section = txtSection.Text.Trim();
                int order = !string.IsNullOrEmpty(txtOrder.Text.Trim()) ? int.Parse(txtOrder.Text.Trim()) : 0;
                string roleCode = ddlIPMSRole.SelectedValue;
                string roleName = ddlIPMSRole.SelectedItem.Text;

                using (var db = new AppDbContext())
                {
                    try
                    {
                        db.AdditionalLoaCogsLimits.Add(new Models.AdditionalLoaCogsLimits
                        {
                            CogsApproverType = "iPMS_Role",
                            CogsApproverCode = roleCode,
                            CogsApproverName = roleName,
                            AmountMin = minValue,
                            AmountMax = maxValue,
                            Section = section,
                            Order = order
                        });
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
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Budget COGS approver added.");
                    Response.Redirect("~/MasterData/AdditionalBudgetApprover");
                }
                else
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Failed to add budget COGS approver.");
                }
            }
        }

        private void BindControl()
        {
            ddlIPMSRole.DataSource = new Class.IPMSRole().GetIPMSRoles();
            ddlIPMSRole.DataValueField = "Code";
            ddlIPMSRole.DataTextField = "DisplayName";
            ddlIPMSRole.DataBind();
            ddlIPMSRole.Items.Insert(0, new ListItem("", ""));
        }
    }
}