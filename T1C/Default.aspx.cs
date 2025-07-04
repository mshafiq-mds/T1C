using CustomGuid.AspNet.Identity;
using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using Prodata.WebForm.Helpers;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.T1C
{
    public partial class Default : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btnAdd.Visible = Auth.User().Can("t1c-add");

            if (!IsPostBack)
            {
                BindControl();
                BindData(!string.IsNullOrEmpty(ddlYear.SelectedValue) ? int.Parse(ddlYear.SelectedValue) : (int?)null);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData(
                !string.IsNullOrEmpty(ddlYear.SelectedValue) ? int.Parse(ddlYear.SelectedValue) : (int?)null,
                txtRef.Text.Trim(),
                !string.IsNullOrEmpty(txtStartDate.Text) ? DateTime.Parse(txtStartDate.Text) : (DateTime?)null,
                !string.IsNullOrEmpty(txtEndDate.Text) ? DateTime.Parse(txtEndDate.Text) : (DateTime?)null,
                !string.IsNullOrEmpty(txtMinAmount.Text) ? decimal.Parse(txtMinAmount.Text.Replace(",", "")) : (decimal?)null,
                !string.IsNullOrEmpty(txtMaxAmount.Text) ? decimal.Parse(txtMaxAmount.Text.Replace(",", "")) : (decimal?)null
                );
            divCardSearch.Attributes["class"] = "card card-outline";
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)((LinkButton)sender).NamingContainer;
            string formId = ((HiddenField)row.FindControl("hdnFormId")).Value;
            Response.Redirect(Request.Url.GetCurrentUrl() + "/Edit?Id=" + formId);
        }

        protected void btnDeleteRecord_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var form = db.Forms.Find(Guid.Parse(hdnRecordId.Value));
                    bool isSuccess = db.SoftDelete(form);
                    if (isSuccess)
                    {
                        // Soft delete transactions related to the form
                        form.SoftDeleteTransactions();

                        // Delete form budgets related to the form
                        foreach (var formBudget in form.FormBudgets)
                        {
                            db.FormBudgets.Remove(formBudget);
                        }
                        db.SaveChanges();

                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Info, "Form deleted.");
                    }
                    else
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Failed to delete form.");
                    }
                }
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
            }

            Response.Redirect(Request.Url.GetCurrentUrl());
        }

        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["pageIndex"] = e.NewPageIndex.ToString();
            BindData(
                !string.IsNullOrEmpty(ddlYear.SelectedValue) ? int.Parse(ddlYear.SelectedValue) : (int?)null,
                txtRef.Text.Trim(),
                !string.IsNullOrEmpty(txtStartDate.Text) ? DateTime.Parse(txtStartDate.Text) : (DateTime?)null,
                !string.IsNullOrEmpty(txtEndDate.Text) ? DateTime.Parse(txtEndDate.Text) : (DateTime?)null,
                !string.IsNullOrEmpty(txtMinAmount.Text) ? decimal.Parse(txtMinAmount.Text.Replace(",", "")) : (decimal?)null,
                !string.IsNullOrEmpty(txtMaxAmount.Text) ? decimal.Parse(txtMaxAmount.Text.Replace(",", "")) : (decimal?)null
                );
        }

        private void BindControl()
        {
            int startYear = 2020;
            int currentYear = DateTime.Now.Year;

            for (int year = currentYear; year >= startYear; year--)
            {
                ListItem item = new ListItem(year.ToString(), year.ToString());
                if (year == currentYear)
                {
                    item.Selected = true; // auto select current year
                }
                ddlYear.Items.Add(item);
            }
        }

        private void BindData(int? year = null, string refNo = null, DateTime? startDate = null, DateTime? endDate = null, decimal? minAmount = null, decimal? maxAmount = null)
        {
            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            var bizAreaCode = Auth.User().iPMSBizAreaCode;

            var form = new Class.Form();
            var formList = form.GetForms(year: year, bizAreaCode: bizAreaCode, refNo: refNo, startDate: startDate, endDate: endDate, amountMin: minAmount, amountMax: maxAmount);

            gvData.DataSource = formList;
            gvData.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
            gvData.DataBind();
        }
    }
}