using CustomGuid.AspNet.Identity;
using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.UploadFGVPISB
{
    public partial class Edit : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindYears();

                if (Request.QueryString["id"] != null)
                {
                    string idStr = Request.QueryString["id"];
                    if (Guid.TryParse(idStr, out Guid id))
                    {
                        LoadData(id);
                        BindAuditLog(id); // Bind Audit Grid
                    }
                    else
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Invalid Record ID.");
                        Response.Redirect("View.aspx");
                    }
                }
                else
                {
                    Response.Redirect("View.aspx");
                }
            }
        }

        private void BindYears()
        {
            int currentYear = DateTime.Now.Year;
            for (int i = currentYear - 2; i <= currentYear + 2; i++)
            {
                ddlYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }

        private void LoadData(Guid id)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var budget = db.Budgets.Find(id);
                    if (budget != null)
                    {
                        hdnId.Value = budget.Id.ToString();

                        // Text Fields
                        txtRef.Text = budget.Ref;
                        txtBACode.Text = budget.BizAreaCode;
                        txtProject.Text = budget.BizAreaName;
                        txtDetails.Text = budget.Details;
                        txtVendor.Text = budget.Vendor;

                        // Numbers (Format to 2 decimal places)
                        txtWages.Text = budget.Wages.HasValue ? budget.Wages.Value.ToString("F2") : "0.00";
                        txtPurchase.Text = budget.Purchase.HasValue ? budget.Purchase.Value.ToString("F2") : "0.00";
                        txtAmount.Text = budget.Amount.HasValue ? budget.Amount.Value.ToString("F2") : "0.00";

                        // Date / Month Logic
                        if (budget.Month.HasValue)
                        {
                            ddlMonth.SelectedValue = budget.Month.Value.ToString();
                        }

                        if (budget.Date.HasValue)
                        {
                            string year = budget.Date.Value.Year.ToString();
                            if (ddlYear.Items.FindByValue(year) != null)
                            {
                                ddlYear.SelectedValue = year;
                            }
                            else
                            {
                                // If year doesn't exist in standard list, add it
                                ddlYear.Items.Add(new ListItem(year, year));
                                ddlYear.SelectedValue = year;
                            }
                        }
                    }
                    else
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Record not found.");
                        Response.Redirect("View.aspx");
                    }
                }
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error loading data: " + ex.Message);
            }
        }

        private void BindAuditLog(Guid budgetId)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    // Perform a Left Join between Budgets_Audit and Users to get the Name
                    var auditData = (from audit in db.Set<Budgets_Audit>()
                                     join user in db.Users on audit.ActionBy equals user.Id into userJoin
                                     from u in userJoin.DefaultIfEmpty() // Handle cases where user might be null or deleted
                                     where audit.BudgetId == budgetId
                                     orderby audit.ActionDate descending
                                     select new
                                     {
                                         ActionDate = audit.ActionDate,
                                         // Map 'Action' from model to 'ActionType' expected by GridView
                                         ActionType = audit.Action,
                                         // Display User Name if found, else "-"
                                         ActionBy = u != null ? u.Name : "-",
                                         // Map 'Details' from model to 'Remarks' expected by GridView
                                         Remarks = audit.Details
                                     })
                                     .ToList();

                    gvAudit.DataSource = auditData;
                    gvAudit.DataBind();
                }
            }
            catch (Exception ex)
            {
                // Silently fail for audit logs or log to console so the main page still functions
                System.Diagnostics.Debug.WriteLine("Audit Error: " + ex.Message);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                Guid id = Guid.Parse(hdnId.Value);

                using (var db = new AppDbContext())
                {
                    var budget = db.Budgets.Find(id);
                    if (budget != null)
                    {
                        // 1. Update Basic Fields on the main Budget object
                        budget.Ref = txtRef.Text.Trim();
                        budget.BizAreaCode = txtBACode.Text.Trim();
                        budget.BizAreaName = txtProject.Text.Trim();
                        budget.Details = txtDetails.Text.Trim();
                        budget.Vendor = txtVendor.Text.Trim();

                        // 2. Update Financials
                        decimal wages = 0, purchase = 0, amount = 0;
                        decimal.TryParse(txtWages.Text, out wages);
                        decimal.TryParse(txtPurchase.Text, out purchase);
                        decimal.TryParse(txtAmount.Text, out amount);

                        budget.Wages = wages;
                        budget.Purchase = purchase;
                        budget.Amount = amount;

                        // 3. Update Date Logic
                        int year = int.Parse(ddlYear.SelectedValue);
                        int month = int.Parse(ddlMonth.SelectedValue);
                        int day = DateTime.DaysInMonth(year, month); // Get last day of that month

                        budget.Month = month;
                        budget.Date = new DateTime(year, month, day);

                        // 4. Update Meta fields
                        budget.UpdatedBy = Auth.Id();
                        budget.UpdatedDate = DateTime.Now;

                        // 5. Create Audit Trail Snapshot
                        var audit = new Budgets_Audit
                        {
                            AuditId = Guid.NewGuid(),
                            BudgetId = budget.Id,
                            Action = "UPDATE",
                            ActionBy = Auth.Id(),
                            ActionDate = DateTime.Now,

                            // Snapshot Data
                            Ref = budget.Ref,
                            BizAreaCode = budget.BizAreaCode,
                            BizAreaName = budget.BizAreaName,
                            Details = budget.Details,
                            Vendor = budget.Vendor,
                            Wages = budget.Wages,
                            Purchase = budget.Purchase,
                            Amount = budget.Amount,
                            Month = budget.Month,
                            Date = budget.Date,

                            // Audit Meta
                            UpdatedBy = budget.UpdatedBy,
                            UpdatedDate = budget.UpdatedDate
                        };

                        // Add audit record to context
                        db.Set<Budgets_Audit>().Add(audit);

                        // Save changes (both Budget update and Audit insert)
                        db.SaveChanges();

                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Budget updated successfully.");

                        // Redirect after short delay or immediately
                        Response.Redirect("View.aspx", false);
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
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error saving data: " + ex.Message);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("View.aspx");
        }
    }
}