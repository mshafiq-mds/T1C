using CustomGuid.AspNet.Identity;
using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
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
                BindData();
            }
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
            BindData();
        }

        private void BindData()
        {
            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            var bizAreaCode = Auth.User().iPMSBizAreaCode;

            var form = new Class.Form();
            var formList = form.GetForms(bizAreaCode: bizAreaCode);

            gvData.DataSource = formList;
            gvData.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
            gvData.DataBind();
        }
    }
}