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
    public partial class Edit : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControl();

                if (Request.QueryString["id"] != null)
                {
                    string idStr = Request.QueryString["id"];
                    if (Guid.TryParse(idStr, out Guid id))
                    {
                        LoadData(id);
                    }
                    else
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Invalid Record ID.");
                        Response.Redirect("~/MasterData/AOW/Default.aspx");
                    }
                }
                else
                {
                    Response.Redirect("~/MasterData/AOW/Default.aspx");
                }
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

        private void LoadData(Guid id)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var rule = db.AssetWriteOffApprovalLimits.FirstOrDefault(x => x.Id == id);

                    if (rule != null)
                    {
                        hdnId.Value = rule.Id.ToString();

                        txtMinAmount.Text = rule.AmountMin.ToString();
                        txtMaxAmount.Text = rule.AmountMax.HasValue ? rule.AmountMax.ToString() : "";
                        txtLevel.Text = rule.Order.ToString();

                        if (ddlRoleCode.Items.FindByValue(rule.AWOApproverCode) != null)
                        {
                            ddlRoleCode.SelectedValue = rule.AWOApproverCode;
                        } 
                    }
                    else
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Approval limit rule not found.");
                        Response.Redirect("~/MasterData/AOW/Default.aspx");
                    }
                }
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error loading record: " + ex.Message);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            // Backend validation
            if (string.IsNullOrWhiteSpace(txtMinAmount.Text) ||
                string.IsNullOrWhiteSpace(txtLevel.Text) ||
                string.IsNullOrWhiteSpace(ddlRoleCode.SelectedValue)  )
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Warning, "Please fill in all required fields.");
                return;
            }

            try
            {
                Guid id = Guid.Parse(hdnId.Value);

                using (var db = new AppDbContext())
                {
                    var rule = db.AssetWriteOffApprovalLimits.FirstOrDefault(x => x.Id == id);

                    if (rule != null)
                    {
                        rule.AmountMin = Convert.ToDecimal(txtMinAmount.Text.Trim());
                        rule.AmountMax = string.IsNullOrWhiteSpace(txtMaxAmount.Text) ? (decimal?)null : Convert.ToDecimal(txtMaxAmount.Text.Trim());
                        rule.Order = Convert.ToInt32(txtLevel.Text.Trim());
                        rule.AWOApproverCode = ddlRoleCode.SelectedValue;

                        // Update Audit Tracking (Using Guid for Auth.User().Id)
                        rule.UpdatedBy = Auth.User().Id;
                        rule.UpdatedDate = DateTime.Now;

                        db.SaveChanges();


                        Session["SweetAlertMessage"] = "Approval limit updated successfully.";
                        Response.Redirect("~/MasterData/AOW/Default.aspx", false);
                        Context.ApplicationInstance.CompleteRequest();
                    }
                    else
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Record no longer exists.");
                    }
                }
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error updating record: " + ex.Message);
            }
        }
    }
}