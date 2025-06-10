using CustomGuid.AspNet.Identity;
using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using NPOI.SS.UserModel;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.Upload
{
	public partial class Default : ProdataPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                BindData();
            }
        }

        protected void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            string filePath = Server.MapPath("~/Budget/Template_Budget.xlsx"); // Adjust path as needed

            if (File.Exists(filePath))
            {
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; // MIME type for .xlsx
                Response.AddHeader("Content-Disposition", "attachment; filename=Template_Budget.xlsx");
                Response.TransmitFile(filePath);
                Response.End();
            }
            else
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Template not found.");
                Response.Redirect(Request.Url.GetCurrentUrl());
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fuBudget.HasFile)
            {
                string filePath = Server.MapPath("~/Uploads/" + Path.GetFileName(fuBudget.FileName));
                fuBudget.SaveAs(filePath);

                ProcessExcelFile(filePath);

                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "File processed successfully.");
                Response.Redirect(Request.Url.GetCurrentUrl());
            }
            else
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "No file.");
                Response.Redirect(Request.Url.GetCurrentUrl());
            }
        }

        protected void btnDeleteRecord_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var budget = db.Budgets.Find(Guid.Parse(hdnRecordId.Value));
                    bool isSuccess = db.SoftDelete(budget);
                    if (isSuccess)
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Info, "Budget deleted.");
                    }
                    else
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Failed to delete budget.");
                    }
                }
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
            }

            Response.Redirect(Request.Url.GetCurrentUrl());
        }

        protected void gvBudget_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["pageIndex"] = e.NewPageIndex.ToString();
            BindData();
        }

        private void BindData()
        {
            ViewState["pageIndex"] = ViewState["pageIndex"] ?? "0";

            var budget = new Class.Budget();
            var list = budget.GetBudgets();

            gvBudget.DataSource = list;
            gvBudget.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
            gvBudget.DataBind();
        }

        #region Process excel file
        private void ProcessExcelFile(string filePath)
        {
            try
            {
                IWorkbook workbook;
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (Path.GetExtension(filePath).Equals(".xls"))
                    {
                        workbook = new NPOI.HSSF.UserModel.HSSFWorkbook(fs); // For .xls files
                    }
                    else
                    {
                        workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(fs); // For .xlsx files
                    }

                    ISheet sheet = workbook.GetSheetAt(0); // Read first sheet
                    using (var db = new AppDbContext())
                    {
                        int skipRows = 1; // Skip first rows (headers)
                        for (int rowNumber = skipRows; rowNumber <= sheet.LastRowNum; rowNumber++)
                        {
                            IRow row = sheet.GetRow(rowNumber);
                            if (row != null)
                            {
                                string reference = row.GetCell(1)?.ToString();
                                int? num = !string.IsNullOrEmpty(reference) ? int.Parse(reference.Split('/').Last()) : (int?)null;

                                string typeName = row.GetCell(5)?.ToString();
                                var type = db.BudgetTypes.FirstOrDefault(t => t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));

                                int? year = TryParseInt(row.GetCell(6));
                                int? month = ConvertMonthNameToNumber(row.GetCell(7)?.ToString()?.Trim());

                                DateTime? date = null;
                                if (year.HasValue && month.HasValue)
                                {
                                    int day = DateTime.DaysInMonth(year.Value, month.Value);
                                    date = new DateTime(year.Value, month.Value, day);
                                }

                                var budget = db.Budgets.ExcludeSoftDeleted().FirstOrDefault(b => b.Ref == reference);
                                if (budget != null)
                                {
                                    db.SoftDelete(budget);
                                }

                                budget = new Models.Budget
                                {
                                    TypeId = type?.Id,
                                    BizAreaCode = row.GetCell(2)?.ToString(),
                                    BizAreaName = row.GetCell(3)?.ToString(),
                                    Date = date,
                                    Month = ConvertMonthNameToNumber(row.GetCell(7)?.ToString()?.Trim()),
                                    Num = num,
                                    Ref = reference,
                                    Details = row.GetCell(4)?.ToString(),
                                    Wages = TryParseDecimal(row.GetCell(9)),
                                    Purchase = TryParseDecimal(row.GetCell(10)),
                                    Amount = TryParseDecimal(row.GetCell(11)),
                                    Vendor = row.GetCell(8)?.ToString()
                                };
                                db.Budgets.Add(budget);
                            }
                        }
                        db.SaveChanges(); // ✅ Save all records to the database
                    }
                }
            }
            catch (Exception ex)
            {
                // ✅ Log error for debugging (no UI notification)
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }
        }

        // Convert month name (Jan, Feb) to a number (1, 2)
        private int ConvertMonthNameToNumber(string monthName)
        {
            if (string.IsNullOrEmpty(monthName)) return 0;

            Dictionary<string, int> months = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                {"Jan", 1}, {"January", 1},
                {"Feb", 2}, {"February", 2},
                {"Mar", 3}, {"March", 3},
                {"Apr", 4}, {"April", 4},
                {"May", 5},
                {"Jun", 6}, {"June", 6},
                {"Jul", 7}, {"July", 7},
                {"Aug", 8}, {"August", 8},
                {"Sep", 9}, {"Sept", 9}, {"September", 9},
                {"Oct", 10}, {"October", 10},
                {"Nov", 11}, {"November", 11},
                {"Dec", 12}, {"December", 12}
            };

            return months.TryGetValue(monthName, out int monthNumber) ? monthNumber : 0;
        }

        // Helper method for safe int conversion
        private int? TryParseInt(ICell cell)
        {
            if (cell != null && int.TryParse(cell.ToString(), out int result))
            {
                return result;
            }
            return null; // Default value if conversion fails
        }

        // Helper method for safe decimal conversion
        private decimal? TryParseDecimal(ICell cell)
        {
            if (cell != null && decimal.TryParse(cell.ToString(), out decimal result))
            {
                return result;
            }
            return null; // Default value if conversion fails
        }
        #endregion
    }
}