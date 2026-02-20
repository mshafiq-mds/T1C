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
    public partial class CumEdit : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["id"] != null && Guid.TryParse(Request.QueryString["id"], out Guid id))
                {
                    BindControl();
                    LoadData(id);
                }
                else
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Invalid ID.");
                    Response.Redirect("~/MasterData/AdditionalBudgetApprover/Default");
                }
            }
        }

        private void LoadData(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var data = db.AdditionalCumulativeLimits.FirstOrDefault(x => x.Id == id);
                if (data != null)
                {
                    hdnId.Value = data.Id.ToString();

                    // Populate Fields
                    // Assuming 'AmountCumulative' maps to 'Min Value' based on previous logic
                    txtMinValue.Text = data.AmountCumulative.HasValue ? data.AmountCumulative.Value.ToString("N2") : "";
                    txtMaxValue.Text = data.AmountMax.HasValue ? data.AmountMax.Value.ToString("N2") : "";

                    txtSection.Text = data.Section;
                    txtOrder.Text = data.Order.ToString();

                    if (ddlIPMSRole.Items.FindByValue(data.CumulativeApproverCode) != null)
                    {
                        ddlIPMSRole.SelectedValue = data.CumulativeApproverCode;
                    }
                }
                else
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Record not found.");
                    Response.Redirect("~/MasterData/AdditionalBudgetApprover/Default");
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                bool isSuccess = false;
                Guid id = Guid.Parse(hdnId.Value);

                decimal amountCumulative = !string.IsNullOrEmpty(txtMinValue.Text.Trim()) ? decimal.Parse(txtMinValue.Text.Trim().Replace(",", "")) : 0;
                decimal? amountMax = !string.IsNullOrEmpty(txtMaxValue.Text.Trim()) ? decimal.Parse(txtMaxValue.Text.Trim().Replace(",", "")) : (decimal?)null;
                string section = txtSection.Text.Trim();
                int order = !string.IsNullOrEmpty(txtOrder.Text.Trim()) ? int.Parse(txtOrder.Text.Trim()) : 0;
                string roleCode = ddlIPMSRole.SelectedValue;
                string roleName = ddlIPMSRole.SelectedItem.Text;

                using (var db = new AppDbContext())
                {
                    try
                    {
                        var limit = db.AdditionalCumulativeLimits.FirstOrDefault(x => x.Id == id);
                        if (limit != null)
                        {
                            limit.CumulativeApproverCode = roleCode;
                            limit.CumulativeApproverName = roleName;
                            limit.AmountCumulative = amountCumulative;
                            limit.AmountMax = amountMax;
                            limit.Section = section;
                            limit.Order = order;

                            limit.UpdatedBy = Auth.User().Id;
                            limit.UpdatedDate = DateTime.Now;

                            db.SaveChanges();
                            isSuccess = true;
                        }
                        else
                        {
                            SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Record no longer exists.");
                        }
                    }
                    catch (Exception ex)
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
                    }
                }

                if (isSuccess)
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Cumulative approver updated.");
                    Response.Redirect("~/MasterData/AdditionalBudgetApprover/Default");
                }
            }
        }

        private void BindControl()
        {
            var roles = new Class.IPMSRole().GetIPMSRoles();
            ddlIPMSRole.DataSource = roles;
            ddlIPMSRole.DataValueField = "Code";
            ddlIPMSRole.DataTextField = "DisplayName";
            ddlIPMSRole.DataBind();
            ddlIPMSRole.Items.Insert(0, new ListItem("", ""));
        }
    }
}