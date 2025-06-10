using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.MasterData.BudgetType
{
	public partial class Edit : ProdataPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
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
                    Response.Redirect(Request.Url.GetCurrentUrl().Replace("Edit", ""));
                }
            }
		}

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                bool isSuccess = false;
                Guid id = Guid.Parse(hdnId.Value);
                string code = txtCode.Text.Trim();
                string name = txtName.Text.Trim();

                if (!RecordExists(id, code, name))
                {
                    try
                    {
                        using (var db = new AppDbContext())
                        {
                            var budgetType = db.BudgetTypes.Find(id);
                            budgetType.Code = code;
                            budgetType.Name = name;
                            db.Entry(budgetType).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            isSuccess = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
                    }
                }
                else
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Record already exists.");
                }

                if (isSuccess)
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Budget type updated.");
                    Response.Redirect("~/MasterData/BudgetType");
                }
                else
                {
                    Response.Redirect(Request.Url.GetCurrentUrl(true));
                }
            }
        }

        private void BindData(string id = null)
        {
            if (!string.IsNullOrEmpty(id))
            {
                using (var db = new AppDbContext())
                {
                    var budgetType = db.BudgetTypes.Find(Guid.Parse(id));

                    txtCode.Text = budgetType.Code;
                    txtName.Text = budgetType.Name;
                }
            }
        }

		private bool RecordExists(Guid id, string code = null, string name = null)
		{
            using (var db = new AppDbContext())
            {
                if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(name))
                    return false;

                return db.BudgetTypes.Any(b =>
                    b.Id != id && // Ignore the given id
                    (
                        (!string.IsNullOrEmpty(code) && b.Code.ToLower() == code.ToLower()) ||
                        (!string.IsNullOrEmpty(name) && b.Name.ToLower() == name.ToLower())
                    )
                );
            }
        }
    }
}