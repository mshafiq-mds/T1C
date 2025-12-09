using CustomGuid.AspNet.Identity;
using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using Prodata.WebForm.Helpers;
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
                        BindDataGV();
                    }
                }
                else
                {
                    Response.Redirect(Request.Url.GetCurrentUrl().Replace("Edit.aspx", ""));
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

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)((LinkButton)sender).NamingContainer;
            string budgetTypeId = ((HiddenField)row.FindControl("hdnId")).Value;
            Response.Redirect(Request.Url.GetCurrentUrl() + "/Edit?Id=" + budgetTypeId);
        }
        protected void gvBudgetType_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["pageIndex"] = e.NewPageIndex.ToString();
            BindDataGV();
        }
        private void BindDataGV()
        { 
            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            var list = GetPurchaseTypes();

            gvBudgetType.DataSource = list;
            gvBudgetType.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
            gvBudgetType.DataBind();
        }
        private List<Models.MasterData.PurchaseTypes> GetPurchaseTypes()
        {
            using (var db = new AppDbContext())
            {
                Guid id = Guid.Parse(hdnId.Value);
                return db.PurchaseTypes
                    .ExcludeSoftDeleted()
                    .Where(b =>
                        b.BudgetTypeID == id)
                    .OrderBy(b => b.Code)
                    .ToList();
            }
        }
        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            hdnEditId.Value = "";
            txtptcode.Text = "";
            txtptname.Text = "";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowModal", "$('#purchaseTypeModal').modal('show');", true);
        }
        // Open Edit Modal
        protected void btnEdit_Command(object sender, CommandEventArgs e)
        {
            Guid id = Guid.Parse(e.CommandArgument.ToString());
            using (var db = new AppDbContext())
            {
                var item = db.PurchaseTypes.FirstOrDefault(x => x.Id == id);
                if (item != null)
                {
                    hdnEditId.Value = item.Id.ToString();
                    txtptcode.Text = item.Code;
                    txtptname.Text = item.Name;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowModal", "$('#purchaseTypeModal').modal('show');", true);
                }
            }
        }
        protected void btnSave_ClickPT(object sender, EventArgs e)
        {
            string msg = "";
            using (var db = new AppDbContext())
            {
                Models.MasterData.PurchaseTypes item;
                if (string.IsNullOrEmpty(hdnEditId.Value))
                {
                    // Add new
                    item = new Models.MasterData.PurchaseTypes
                    {
                        Id = Guid.NewGuid(),
                        Code = txtptcode.Text.Trim(),
                        Name = txtptname.Text.Trim(),
                        CreatedBy = Auth.User().Id,
                        CreatedDate = DateTime.Now,
                        BudgetTypeID = Guid.Parse(hdnId.Value)
                    };
                    db.PurchaseTypes.Add(item);
                    msg = "Purchase Types Succesfully Created.";
                }
                else
                {
                    // Edit existing
                    Guid id = Guid.Parse(hdnEditId.Value);
                    item = db.PurchaseTypes.FirstOrDefault(x => x.Id == id);
                    if (item != null)
                    {
                        item.Code = txtptcode.Text.Trim();
                        item.Name = txtptname.Text.Trim();
                        item.UpdatedBy = Auth.User().Id;
                        item.CreatedDate = DateTime.Now;
                        item.BudgetTypeID = Guid.Parse(hdnId.Value);
                    }
                    msg = "Purchase Types Succesfully Edited.";
                }

                db.SaveChanges();
            }

            BindDataGV();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "HideModal", "$('#purchaseTypeModal').modal('hide');", true);
            SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, msg);
        }

        // Delete
        protected void btnDelete_Command(object sender, CommandEventArgs e)
        { 
            Guid id = Guid.Parse(e.CommandArgument.ToString());
            using (var db = new AppDbContext())
            {
                var form = db.PurchaseTypes.Find(id);
                bool isSuccess = db.SoftDelete(form);
                if (isSuccess)
                { 
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Info, "Purchase types deleted.");
                }
                else
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Failed to delete purchase types.");
                }
            } 
            BindDataGV();
        }
        protected void gvBudgetType_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton btnDelete = (LinkButton)e.Row.FindControl("btnDelete");
                if (btnDelete != null)
                {
                    btnDelete.Attributes["data-uniqueid"] = btnDelete.UniqueID;
                }
            }
        }

    }
}