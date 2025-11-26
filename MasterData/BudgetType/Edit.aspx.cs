using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.MasterData;
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
                        BindFormCategories();
                        BindBudgetCategories();
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

                            int formCategory = int.Parse(ddlFormCategories.SelectedValue);
                            budgetType.FormCategories = formCategory;

                            int BudgetCategory = int.Parse(ddlBudgetCategories.SelectedValue);
                            budgetType.BudgetCategories = BudgetCategory;

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

                    // Preselect FormCategories dropdown
                    if (budgetType.FormCategories != null)
                        ddlFormCategories.SelectedValue = budgetType.FormCategories.ToString();

                    if (budgetType.BudgetCategories != null)
                        ddlBudgetCategories.SelectedValue = budgetType.BudgetCategories.ToString();
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
        private void BindDropdown(DropDownList ddl, List<dynamic> data)
        {
            ddl.DataSource = data;
            ddl.DataTextField = "Text";
            ddl.DataValueField = "Value";
            ddl.DataBind();

            ddl.Items.Insert(0, new ListItem("-- Select --", ""));
        }
        private void BindFormCategories()
        {
            var categories = new List<dynamic>
            {
                new { Value = 1, Text = "Details Form" },
                new { Value = 2, Text = "Others Form" },
                new { Value = 3, Text = "Pool Form" }
            };

            BindDropdown(ddlFormCategories, categories);
        }
        private void BindBudgetCategories()
        {
            var categories = new List<dynamic>
            {
                new { Value = 1, Text = "Details Budget" },
                new { Value = 2, Text = "Others Budget" },
                new { Value = 3, Text = "Pool Budget" }
            };

            BindDropdown(ddlBudgetCategories, categories);
        }


    }
}