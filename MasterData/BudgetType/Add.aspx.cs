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
	public partial class Add : ProdataPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

        protected void btnSave_Click(object sender, EventArgs e)
        {
			if (IsValid)
			{
				bool isSuccess = false;
				string code = txtCode.Text.Trim();
				string name = txtName.Text.Trim();

				if (!RecordExists(code, name))
				{
					try
					{
						using (var db = new AppDbContext())
						{
							var budgetType = new Models.MasterData.BudgetType
							{
								Code = code,
								Name = name
							};
							db.BudgetTypes.Add(budgetType);
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
					SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Budget type added.");
					Response.Redirect(Request.Url.GetCurrentUrl().Replace("Add", ""));
				}
				else
				{
                    Response.Redirect(Request.Url.GetCurrentUrl());
                }
			}
        }

		private bool RecordExists(string code = null, string name = null)
		{
			using (var db = new AppDbContext())
			{
				if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(name))
					return true;

				return db.BudgetTypes.Any(b =>
					(!string.IsNullOrEmpty(code) && b.Code.ToLower().Equals(code.ToLower())) ||
					(!string.IsNullOrEmpty(name) && b.Name.ToLower().Equals(name.ToLower())));
			}
		}
    }
}