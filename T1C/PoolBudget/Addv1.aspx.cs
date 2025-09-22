using FGV.Prodata.Web.UI;
using NPOI.HPSF;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.T1C.PoolBudget
{
    public partial class Addv1 : ProdataPage
    {
        string reftypecode = "T2";

        protected void Page_Load(object sender, EventArgs e)
        {
            int formgourp = 2;
            if (!IsPostBack)
            {
                Label1.Text = Functions.GetGeneratedRefNo(reftypecode, true);
                BindBALabel();
                BindDropdown(ddlBT, Functions.GetBudgetTypes(), "ID", "DisplayName", formgourp);
            }
        } 
        /// <summary>
        /// Handle form button click (BudgetType = 5)
        /// </summary>
        protected void btnPnlForm_Click(object sender, EventArgs e)
        {

            CreateFormAndTransaction( 
                baCode: LblBA.Text.Trim(),
                baName: LblBAName.Text.Trim(),
                justification: txtJustification.Text.Trim(),
                amountText: txtAmount.Text.Trim(),
                PTID: ddlPT.SelectedValue,
                fromId: hdnGuidBudgetType.Value,
                fuDd: fuDocument
            );
        } 
        /// Common method to create FormsProcurement + Transaction
        private void CreateFormAndTransaction( 
            string baCode,
            string baName,
            string justification,
            string amountText,
            string PTID,
            string fromId,
            FileUpload fuDd
            )
        {
            using (var db = new AppDbContext())
            {
                try
                {
                    // Convert Guid
                    var GUIDBT = Guid.Parse(fromId);
                    var GUIDPT = Guid.Parse(PTID);
                    var PTName = db.PurchaseTypes.Where(x => x.Id.Equals(GUIDPT)).Select(x => x.Name).FirstOrDefault();
                    
                    // Parse amount safely
                    decimal amount = !string.IsNullOrEmpty(amountText)
                        ? decimal.Parse(amountText.Replace(",", ""))
                        : 0;

                    // Get BudgetType Id from code
                    var typeEntity = db.BudgetTypes
                        .Where(t => t.Id == GUIDBT)
                        .Select(t => t.Id)
                        .FirstOrDefault();

                    // Create procurement form record
                    var form = new Models.FormsProcurement
                    {
                        TypeId = typeEntity,
                        BizAreaCode = baCode,
                        BizAreaName = new Class.IPMSBizArea().GetNameByCode(baCode),
                        Date = DateTime.Now,
                        Ref = Functions.GetGeneratedRefNo(reftypecode, false), // unique ref no
                        JustificationOfNeed = justification,
                        Amount = amount,
                        Details = PTName,
                        PurchaseType = GUIDPT,
                        Status = "Pending"
                    };

                    db.FormsProcurement.Add(form);
                    db.SaveChanges();

                    // Create transaction linked to procurement form
                    var transaction = new Models.Transaction
                    {
                        FromId = GUIDBT,
                        FromType = "Budget",
                        ToId = form.Id,
                        ToType = "FormsProcurement",
                        Date = DateTime.Now,
                        Ref = form.Ref,
                        Name = "-",
                        Amount = amount,
                        Status = "Submitted"
                    };

                    db.Transactions.Add(transaction);
                    db.SaveChanges();

                    if (fuDd.HasFile)
                    {
                        using (var binaryReader = new System.IO.BinaryReader(fuDd.PostedFile.InputStream))
                        {
                            byte[] fileData = binaryReader.ReadBytes(fuDd.PostedFile.ContentLength);

                            var document = new FormsProcurementDocuments
                            {
                                TransferId = form.Id,
                                FileName = fuDd.FileName,
                                ContentType = fuDd.PostedFile.ContentType,
                                FileData = fileData,
                                UploadedBy = Auth.Id(),
                                UploadedDate = DateTime.Now
                            };

                            db.FormsProcurementDocuments.Add(document);
                            db.SaveChanges();
                        }
                    }

                    // Function Emails
                    Emails.EmailsT2ForNewRequest(form.Id, form, Auth.User().iPMSRoleCode);

                    // Show success alert
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Successful Create Transaction.");

                    // Redirect with slight delay → allow SweetAlert to display first
                    ScriptManager.RegisterStartupScript(
                        this,
                        GetType(),
                        "redirect",
                        "setTimeout(function(){ window.location = '" + ResolveUrl("~/T1C/PoolBudget") + "'; }, 100);",
                        true);
                }
                catch (Exception ex)
                {
                    // Show error if exception occurs
                    //SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, ex.Message);
                    string script = $"Swal.fire({{ icon: 'error', title: 'Error', text: '{ex.Message.Replace("'", "\\'")}' }});";
                    ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlertError", script, true);
                }
            }
        }

        private void BindPurchaseType(string guid = null)
        {
            using (var db = new AppDbContext())
            {
                IQueryable<PurchaseTypes> query = db.PurchaseTypes
                    .Where(x => x.DeletedDate == null);

                if (!string.IsNullOrEmpty(guid))
                {
                    Guid GUIDBT;
                    if (Guid.TryParse(guid, out GUIDBT))
                    {
                        query = query.Where(x => x.BudgetTypeID == GUIDBT);
                    }
                }

                var dataSource = query
                    .OrderBy(x => x.Code)
                    .ToList();

                ddlPT.DataSource = dataSource;
                ddlPT.DataValueField = "Id";   // better to use Id instead of Name
                ddlPT.DataTextField = "Name";
                ddlPT.DataBind();

                // Insert default option
                ddlPT.Items.Insert(0, new ListItem("-- Select Purchase Type --", ""));
            }
        }

        private List<BudgetResult> GetBudgetsForCategory(int formCategory)
        {
            string BA = Auth.User().iPMSBizAreaCode;
            int currentYear = DateTime.Today.Year;

            using (var db = new AppDbContext())
            {
                // 1. Get all BudgetTypes in this category
                var budgetTypes = db.BudgetTypes
                    .Where(b => b.FormCategories == formCategory && b.DeletedBy == null)
                    .Select(b => new { b.Id, b.Code, b.Name })
                    .ToList();

                var typeIds = budgetTypes.Select(x => x.Id).ToList();

                // 2. Get Budgets for these types
                var budgets = db.Budgets
                    .Where(b => b.TypeId.HasValue
                                && typeIds.Contains(b.TypeId.Value)
                                && b.BizAreaCode == BA
                                && b.Date.HasValue
                                && b.Date.Value.Year == currentYear
                                && b.DeletedBy == null)
                    .Select(b => new { b.Id, b.TypeId, b.Amount })
                    .ToList();

                var budgetIds = budgets.Select(b => b.Id).ToList();

                // 3. Get inflows & outflows in one go
                var transactions = db.Transactions
                    .Where(t => (t.FromId.HasValue && budgetIds.Contains(t.FromId.Value))
                             || (t.ToId.HasValue && budgetIds.Contains(t.ToId.Value)))
                    .Where(t => t.DeletedBy == null)
                    .GroupBy(t => new { t.FromId, t.ToId })
                    .Select(g => new
                    {
                        g.Key.FromId,
                        g.Key.ToId,
                        Total = g.Sum(x => x.Amount)
                    })
                    .ToList();

                // 4. Build results
                var results = new List<BudgetResult>();

                foreach (var bt in budgetTypes)
                {
                    var budget = budgets.FirstOrDefault(b => b.TypeId == bt.Id);
                    if (budget == null) continue;

                    decimal outflow = (decimal)transactions
                        .Where(t => t.FromId.HasValue && t.FromId.Value == budget.Id)
                        .Sum(t => t.Total);

                    decimal inflow = (decimal)transactions
                        .Where(t => t.ToId.HasValue && t.ToId.Value == budget.Id)
                        .Sum(t => t.Total);

                    results.Add(new BudgetResult
                    {
                        BudgetId = bt.Id,
                        Code = bt.Code,
                        Name = bt.Name,
                        Amount = budget.Amount ?? 0m,   // handle null
                        Balance = (budget.Amount ?? 0m) - outflow + inflow
                    });
                }

                return results;
            }
        }


        protected void BudgetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlBT.SelectedValue))
            {
                // Show panel and reset button when dropdown has a value
                PnlForm.Style["display"] = "block";
            }
            else
            {
                // Hide them if user clears the selection
                PnlForm.Style["display"] = "none";
            }

            BindPurchaseType(ddlBT.SelectedValue);

            var budgetsR = GetBudgetsForCategory(2);

            var ddltypeId = Guid.TryParse(ddlBT.SelectedValue, out var parsedTypeId)
                ? parsedTypeId
                : Guid.Empty;

            var balance = budgetsR.Where(x => x.BudgetId == ddltypeId)
                                 .Select(x => x.Balance)
                                 .FirstOrDefault();

            var amount = budgetsR.Where(x => x.BudgetId == ddltypeId)
                                .Select(x => x.Amount)
                                .FirstOrDefault();


            using (var db = new AppDbContext())
            {
                CardTitle.Text = db.BudgetTypes.Where(b => b.Id == ddltypeId).Select(b => b.Name).FirstOrDefault();

                string ba = Auth.User().iPMSBizAreaCode;
                hdnGuidBudgetType.Value = db.Budgets
                                    .Where(x => x.TypeId == ddltypeId && x.DeletedDate == null && x.BizAreaCode == ba)
                                    .Select(x => x.Id)
                                    .FirstOrDefault().ToString();
            }

            LblBalance.Text = balance.ToString("N2");  // formatted with 2 decimals
            LblBuget.Text = amount.ToString("N2"); 
        }


        private void BindBALabel()
        {
            string ba = Auth.User().iPMSBizAreaCode;

            bool isEmptyBA = string.IsNullOrWhiteSpace(ba);

            phBA.Visible = !isEmptyBA;
            phBAPh.Visible = isEmptyBA;

            if (!isEmptyBA)
            {
                LblBA.Text = ba;
                LblBAName.Text = new Class.IPMSBizArea().GetNameByCode(ba) ?? "";
            }
        }
        private void BindDropdown(ListControl ddl, object dataSource, string dataValueField, string dataTextField, int? formCategory = null)
        {
            // Try casting to List<BudgetType>
            if (dataSource is IEnumerable<BudgetType> list && formCategory.HasValue)
            {
                list = list.Where(x => x.FormCategories == formCategory.Value);
                dataSource = list;
            }

            ddl.DataSource = dataSource;
            ddl.DataValueField = dataValueField;
            ddl.DataTextField = dataTextField;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("", ""));
        }
        public class BudgetResult
        {
            public Guid BudgetId { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public decimal Amount { get; set; }
            public decimal Balance { get; set; }
        }
    }
}