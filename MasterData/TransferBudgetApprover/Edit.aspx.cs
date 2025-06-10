using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Prodata.WebForm.MasterData.TransferBudgetApprover
{
    public partial class Edit : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControl();
                if (Request.QueryString["Id"] != null)
                {
                    string id = Request.QueryString["Id"].ToString();
                    Guid guid;
                    if (Guid.TryParse(id, out guid))
                    {
                        hdnId.Value = id;
                        BindData(id);
                    }
                }
                else
                {
                    Response.Redirect("~/MasterData/TransferBudgetApprover");
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                bool isSuccess = false;
                Guid id = Guid.Parse(hdnId.Value);
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
                        var budgetApprover = db.TransferApprovalLimits.Find(id);
                        budgetApprover.AmountMin = minValue;
                        budgetApprover.AmountMax = maxValue;
                        budgetApprover.Section = section;
                        budgetApprover.Order = order;
                        budgetApprover.TransApproverCode = roleCode;
                        budgetApprover.TransApproverName = roleName;
                        db.Entry(budgetApprover).State = System.Data.Entity.EntityState.Modified;
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
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Budget approver updated.");
                    Response.Redirect("~/MasterData/TransferBudgetApprover");
                }
                else
                {
                    Response.Redirect(Request.Url.GetCurrentUrl(true));
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

        private void BindData(string id = null)
        {
            if (!string.IsNullOrEmpty(id))
            {
                using (var db = new AppDbContext())
                {
                    var budgetApprover = db.TransferApprovalLimits.Find(Guid.Parse(id));
                    if (budgetApprover != null)
                    {
                        txtMinValue.Text = budgetApprover.AmountMin.HasValue ? budgetApprover.AmountMin.Value.ToString("#,##0.00") : string.Empty;
                        txtMaxValue.Text = budgetApprover.AmountMax.HasValue ? budgetApprover.AmountMax.Value.ToString("#,##0.00") : string.Empty;
                        txtSection.Text = budgetApprover.Section;
                        txtOrder.Text = budgetApprover.Order.ToString();
                        ddlIPMSRole.SelectedValue = budgetApprover.TransApproverCode;
                    }
                    else
                    {
                        Response.Redirect("~/MasterData/TransferBudgetApprover");
                    }
                }
            }
        }
    }
}