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
                BindData(Auth.User().iPMSRoleCode, Auth.IPMSBizAreaCodes());
            }
        }

        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["pageIndex"] = e.NewPageIndex.ToString();
            BindData(Auth.User().iPMSRoleCode, Auth.IPMSBizAreaCodes());
        }

        private void BindData(string ipmsRoleCode = null, List<string> ipmsBizAreaCodes = null)
        {
            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            gvData.DataSource = new Class.Form().GetFormsForApproval(ipmsRoleCode, ipmsBizAreaCodes);
            gvData.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
            gvData.DataBind();
        }
    }
}