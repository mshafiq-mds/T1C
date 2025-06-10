using FGV.Prodata.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.T1C.Approval
{
    public partial class Default : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData(Auth.User().iPMSRoleCode);
            }
        }

        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["pageIndex"] = e.NewPageIndex.ToString();
            BindData(Auth.User().iPMSRoleCode);
        }

        private void BindData(string ipmsRoleCode = null)
        {
            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            gvData.DataSource = new Class.Form().GetFormsForApproval(ipmsRoleCode);
            gvData.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
            gvData.DataBind();
        }
    }
}