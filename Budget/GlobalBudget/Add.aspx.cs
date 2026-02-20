using FGV.Prodata.Web.UI;
using Org.BouncyCastle.Asn1.Ocsp;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.GlobalBudget
{
    public partial class Add : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindYear();
                BindBudgetType();

                // Check for ID to determine Edit Mode
                if (Request.QueryString["Id"] != null && Guid.TryParse(Request.QueryString["Id"], out Guid id))
                {
                    BindData(id);
                }
            }
        }

        private void BindYear()
        {
            int currentYear = DateTime.Now.Year;
            ddlYear.Items.Clear();
            // Show range: Last year to +1 year ahead
            for (int i = currentYear - 3; i <= currentYear + 1; i++)
            {
                ddlYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
            ddlYear.SelectedValue = currentYear.ToString();
        }

        private void BindBudgetType()
        {
            using (var db = new AppDbContext())
            {
                var types = db.BudgetTypes
                    .Where(x => x.DeletedDate == null && x.BudgetCategories == 3)
                    .OrderBy(x => x.Name)
                    .Select(x => new { x.Id, x.Name })
                    .ToList();

                ddlBudgetType.DataSource = types;
                ddlBudgetType.DataValueField = "Id";
                ddlBudgetType.DataTextField = "Name";
                ddlBudgetType.DataBind();
                ddlBudgetType.Items.Insert(0, new ListItem("-- Select Type --", ""));
            }
        }

        private void BindData(Guid id)
        {
            using (var db = new AppDbContext())
            {
                // Changed from PoolBudgets to Budgets
                var budget = db.Budgets.Find(id);
                if (budget != null)
                {
                    // Set Year (Extract from Date)
                    int year = budget.Date.HasValue ? budget.Date.Value.Year : DateTime.Now.Year;

                    if (ddlYear.Items.FindByValue(year.ToString()) == null)
                    {
                        ddlYear.Items.Add(new ListItem(year.ToString(), year.ToString()));
                    }
                    ddlYear.SelectedValue = year.ToString();

                    // Set Budget Type (Mapped to TypeId)
                    if (budget.TypeId.HasValue && ddlBudgetType.Items.FindByValue(budget.TypeId.ToString()) != null)
                    {
                        ddlBudgetType.SelectedValue = budget.TypeId.ToString();
                    }

                    // Set Amount and Description (Mapped to Details)
                    txtAmount.Text = budget.Amount.HasValue ? budget.Amount.Value.ToString("N2") : "0.00";
                    txtDescription.Text = budget.Details;
                }
                else
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Record not found.");
                    Response.Redirect("Default.aspx");
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                int year = int.Parse(ddlYear.SelectedValue);
                string amountStr = txtAmount.Text.Replace(",", "").Trim();
                string budgetTypeIdStr = ddlBudgetType.SelectedValue;

                if (string.IsNullOrEmpty(budgetTypeIdStr))
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Warning, "Please select a Budget Type.");
                    return;
                }

                if (!decimal.TryParse(amountStr, out decimal amount) || amount <= 0)
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Warning, "Please enter a valid amount greater than 0.");
                    return;
                }

                Guid budgetTypeId = Guid.Parse(budgetTypeIdStr);

                // Determine if we are in Edit Mode
                Guid? editId = null;
                if (Request.QueryString["Id"] != null && Guid.TryParse(Request.QueryString["Id"], out Guid parsedId))
                {
                    editId = parsedId;
                }

                using (var db = new AppDbContext())
                {
                    // Check for duplicates in Budgets table
                    // Logic: Same Year (via Date), Same Type, Not Deleted
                    var duplicateQuery = db.Budgets
                        .Where(x => x.Date.HasValue && x.Date.Value.Year == year
                                    && x.TypeId == budgetTypeId
                                    && x.DeletedDate == null);

                    // If Editing, exclude self from duplicate check
                    if (editId.HasValue)
                    {
                        duplicateQuery = duplicateQuery.Where(x => x.Id != editId.Value);
                    }

                    var existing = duplicateQuery.FirstOrDefault();

                    if (existing != null)
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, $"A Pool Budget for {year} with this Type already exists (Amount: RM {existing.Amount:N2}). Duplicates are not allowed.");
                        return;
                    }

                    if (editId.HasValue)
                    {
                        // --- UPDATE LOGIC ---
                        var budget = db.Budgets.Find(editId.Value);
                        if (budget == null) throw new Exception("Budget record not found.");

                        // Map UI to Model
                        budget.Date = new DateTime(year, 1, 1); // Set to Jan 1st of the selected year
                        budget.TypeId = budgetTypeId;
                        budget.Amount = amount;
                        budget.Details = string.IsNullOrWhiteSpace(txtDescription.Text) ? $"Pool Budget {year}" : txtDescription.Text.Trim();
                        // Ref update if necessary
                        budget.Ref = ddlBudgetType.SelectedItem + $"-POOL-{year}";

                        budget.UpdatedBy = Auth.User().Id;
                        budget.UpdatedDate = DateTime.Now;

                        db.Entry(budget).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Pool Budget updated successfully.");
                    }
                    else
                    {
                        // --- INSERT LOGIC ---
                        var budget = new Prodata.WebForm.Models.Budget
                        {
                            Id = Guid.NewGuid(),
                            Date = new DateTime(year, 1, 1),
                            TypeId = budgetTypeId,
                            Details = string.IsNullOrWhiteSpace(txtDescription.Text) ? $"Pool Budget {year}" : txtDescription.Text.Trim(),
                            Amount = amount,
                            Ref = ddlBudgetType.SelectedItem + $"-POOL-{year}",
                            // Status = "Active", // Optional: if your Budget model has status
                            CreatedBy = Auth.User().Id,
                            CreatedDate = DateTime.Now
                        };

                        db.Budgets.Add(budget);
                        db.SaveChanges();

                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, $"Pool Budget for {year} initialized successfully.");

                        // Reset form only on Add
                        txtAmount.Text = "";
                        txtDescription.Text = "";
                        ddlBudgetType.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error saving budget: " + ex.Message);
            }

            Response.Redirect("Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}