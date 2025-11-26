using CustomGuid.AspNet.Identity;
using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using Org.BouncyCastle.Asn1.Ocsp;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.UploadFGVPISB
{
    public partial class Default : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
                BindDropdown(ddlBT , Functions.GetBudgetTypes(), "ID", "DisplayName");
            }
        }

        protected void btnDownloadTemplateMain_Click(object sender, EventArgs e)
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
        protected void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            string filePath = Server.MapPath("~/Budget/Template_BudgetFGVPISB.xlsx"); // Adjust path as needed

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
            if (string.IsNullOrEmpty(ddlBT.SelectedValue))
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Please select a type.");
                return;
            }

            if (fuBudget.HasFile)
            {
                try
                {
                    using (var stream = fuBudget.PostedFile.InputStream)
                    {
                        int budgetformtype;

                        using (var db = new AppDbContext())
                        {
                            var typeId = Guid.TryParse(ddlBT.SelectedValue, out var parsedTypeId) ? parsedTypeId : Guid.Empty;
                            budgetformtype = db.BudgetTypes
                                .Where(x => x.Id == typeId)
                                .Select(x => x.FormCategories)
                                .FirstOrDefault();
                        }

                        if (budgetformtype == 1)
                        {
                            ProcessExcelFileFullForm(stream, fuBudget.FileName); // Pass stream & filename (selenggaraan)
                        }
                        else
                        {
                            ProcessExcelFile(stream, fuBudget.FileName); // Pass stream & filename 
                        }

                    }

                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "File processed successfully.");
                }
                catch (Exception ex)
                {
                    string errmsg = "Error: " + ex.Message.Replace("'", "").Replace("\r", "").Replace("\n", "") ;
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, errmsg);
                }

                Response.Redirect(Request.Url.GetCurrentUrl());
            }
            else
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Please select a file.");
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
            //var list = budget.GetBudgets().Where(x => x.Vendor == null).ToList();
            int currentYear = DateTime.Now.Year;

            var list = budget.GetBudgets()
                .Where(x => !string.IsNullOrEmpty(x.Date) &&
                            DateTime.TryParseExact(x.Date, "dd/MM/yyyy",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out var d) &&
                            d.Year == currentYear)
                .ToList();


            gvBudget.DataSource = list;
            gvBudget.PageIndex = int.Parse(ViewState["pageIndex"].ToString());
            gvBudget.DataBind();
        }

        private void BindDropdown(ListControl ddl, object dataSource, string dataValueField, string dataTextField)
        {
            ddl.DataSource = dataSource;
            ddl.DataValueField = dataValueField;
            ddl.DataTextField = dataTextField;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("", ""));
        }

        #region Process excel file
        private void ProcessExcelFileFullForm(Stream stream, string fileName)
        {
            try
            {
                IWorkbook workbook;

                if (Path.GetExtension(fileName).Equals(".xls", StringComparison.OrdinalIgnoreCase))
                {
                    workbook = new NPOI.HSSF.UserModel.HSSFWorkbook(stream); // for .xls
                }
                else
                {
                    workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(stream); // for .xlsx
                }

                ISheet sheet = workbook.GetSheetAt(0); // read first sheet
                if (sheet == null)
                    throw new Exception("Sheet not found in Excel file.");

                // ✅ Step 1: check header
                IRow headerRow = sheet.GetRow(0);
                if (headerRow == null)
                    throw new Exception("Header row not found in Excel file.");

                string[] expectedHeaders = new[]
                {
                    "NO", "NO.RUJUKAN", "BA", "PROJEK", "PUSAT KOS", "CC",
                    "BUTIR-BUTIR KERJA", "BULAN", "VENDOR", "UPAH (RM)",
                    "BELIAN ALAT GANTI (RM)", "JUMLAH (RM)", "TAHUN"
                };

                for (int i = 0; i < expectedHeaders.Length; i++)
                {
                    var cellValue = headerRow.GetCell(i)?.ToString()?.Trim();
                    if (!string.Equals(cellValue, expectedHeaders[i], StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception($"Header mismatch at column {i + 1}. Expected '{expectedHeaders[i]}', found '{cellValue}'.");
                    }
                }

                // ✅ Step 2: process rows
                using (var db = new AppDbContext())
                {
                    int skipRows = 1; // skip header row
                    for (int rowNumber = skipRows; rowNumber <= sheet.LastRowNum; rowNumber++)
                    {
                        IRow row = sheet.GetRow(rowNumber);
                        if (row == null) continue;

                        // read cells
                        string reference = row.GetCell(1)?.ToString()?.Trim();
                        int? num = !string.IsNullOrEmpty(reference) ? int.Parse(reference.Split('/').Last()) : (int?)null;

                        var typeId = Guid.TryParse(ddlBT.SelectedValue, out var parsedTypeId) ? parsedTypeId : Guid.Empty;

                        int? year = TryParseInt(row.GetCell(12));
                        int? month = ConvertMonthNameToNumber(row.GetCell(7)?.ToString()?.Trim());

                        DateTime? date = null;
                        if (year.HasValue && month.HasValue)
                        {
                            int day = DateTime.DaysInMonth(year.Value, month.Value);
                            date = new DateTime(year.Value, month.Value, day);
                        }

                        // check if same Ref exists (and not soft deleted)
                        var oldBudget = db.Budgets.ExcludeSoftDeleted().FirstOrDefault(b => b.Ref == reference);
                        if (oldBudget != null)
                        {
                            db.SoftDelete(oldBudget); // keep your existing SoftDelete method
                        }

                        // insert new Budget
                        var budget = new Models.Budget
                        {
                            Id = Guid.NewGuid(),
                            TypeId = typeId,
                            BizAreaCode = row.GetCell(2)?.ToString(),
                            BizAreaName = row.GetCell(3)?.ToString(),
                            Date = date,
                            Month = month,
                            Num = num,
                            Ref = reference,
                            Details = row.GetCell(6)?.ToString(),
                            Wages = TryParseDecimal(row.GetCell(9)),
                            Purchase = TryParseDecimal(row.GetCell(10)),
                            Amount = TryParseDecimal(row.GetCell(11)),
                            Vendor = row.GetCell(8)?.ToString(),
                            CreatedDate = DateTime.Now,
                            CreatedBy = Auth.Id()
                        };
                        db.Budgets.Add(budget);
                    }

                    db.SaveChanges(); // save all at once
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error importing Excel: " + ex.Message);
                throw new Exception($"Data mismatch. Expected Error importing Excel: " + ex.Message); // rethrow so caller can handle (or show alert)
            }
        }


        private void ProcessExcelFile(Stream stream, string fileName)
        {
            try
            {
                IWorkbook workbook;

                if (Path.GetExtension(fileName).Equals(".xls", StringComparison.OrdinalIgnoreCase))
                {
                    workbook = new NPOI.HSSF.UserModel.HSSFWorkbook(stream); // for .xls
                }
                else
                {
                    workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(stream); // for .xlsx
                }

                ISheet sheet = workbook.GetSheetAt(0); // first sheet

                if (sheet == null)
                    throw new Exception("Sheet not found in Excel file.");

                IRow headerRow = sheet.GetRow(0);
                if (headerRow == null)
                    throw new Exception("Header row not found.");

                // ✅ Expected headers
                string[] expectedHeaders = new string[]
                {
                    "NO", "PROJEK", "BA", "JUMLAH",
                    "JAN", "FEB", "MAR", "APR", "MAY", "JUN",
                    "JUL", "AUG", "SEP", "OCT", "NOV", "DEC",
                    "YEAR"
                };

                for (int i = 0; i < expectedHeaders.Length; i++)
                {
                    string actualHeader = headerRow.GetCell(i)?.ToString()?.Trim().ToUpper() ?? "";
                    string expectedHeader = expectedHeaders[i].ToUpper();

                    if (actualHeader != expectedHeader)
                    {
                        throw new Exception($"Header mismatch at column {i + 1}. Expected {expectedHeader}, found {actualHeader.Replace("'", "")}");
                    }
                }

                var typeId = Guid.TryParse(ddlBT.SelectedValue, out var parsedTypeId) ? parsedTypeId : Guid.Empty;
                var currentUserId = Auth.Id(); // cache once

                using (var db = new AppDbContext())
                {
                    IRow rowCheckYear = sheet.GetRow(1);
                    if (rowCheckYear == null)
                    { 
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Year in Excel is missing or invalid."); 
                        return;
                    }

                    int? yearExcel= TryParseInt(rowCheckYear.GetCell(16)); //

                    // 🧹 Soft delete existing Budgets & BudgetsDetails for this TypeId
                    var oldBudgets = db.Budgets.Where(b => b.TypeId == typeId && b.DeletedDate == null && b.Date.Value.Year == yearExcel).ToList();

                    bool isSuccessbudget = true;

                    foreach (var budget in oldBudgets)
                    {
                        if (!db.SoftDelete(budget))
                        {
                            isSuccessbudget = false;
                            break; // stop on first failure if you want
                        }
                    }

                    if (!isSuccessbudget)
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Failed to delete old Budgets.");
                    }

                    //var oldDetails = db.BudgetsDetails.Where(d => d.TypeId == typeId && d.DeletedDate == null).ToList();

                    //bool isSuccessdetail = true;

                    //foreach (var detail in oldDetails)
                    //{
                    //    if (!db.SoftDelete(detail))
                    //    {
                    //        isSuccessdetail = false;
                    //        break;
                    //    }
                    //}

                    //if (!isSuccessdetail)
                    //{
                    //    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Failed to delete old Details Budget.");
                    //}

                    int year = yearExcel.Value;

                    var today = DateTime.Today;
                    // start from row 1 (after header)
                    for (int rowNumber = 1; rowNumber <= sheet.LastRowNum; rowNumber++)
                    {
                        IRow row = sheet.GetRow(rowNumber);
                        if (row == null) continue;

                        int? num = TryParseInt(row.GetCell(0)); // NO
                        string projek = row.GetCell(1)?.ToString()?.Trim(); // PROJEK
                        string ba = row.GetCell(2)?.ToString()?.Trim(); // BA
                        decimal? jumlah = TryParseDecimal(row.GetCell(3)); // JUMLAH

                        // validate
                        if (string.IsNullOrEmpty(ba) || !jumlah.HasValue) continue;

                        var refText = ddlBT.SelectedItem?.Text ?? string.Empty;
                        var detailsText = ddlBT.SelectedItem?.Text ?? string.Empty;

                        // insert into Budgets
                        var budget = new Models.Budget
                        {
                            Id = Guid.NewGuid(),
                            BizAreaCode = ba,
                            BizAreaName = projek,
                            Num = num,
                            TypeId = typeId,
                            Ref = refText,
                            Details = detailsText,
                            Amount = jumlah.Value,
                            CreatedDate = DateTime.Now,
                            CreatedBy = currentUserId,
                            Date = new DateTime(year, today.Month, today.Day) // ✅ year from excel, day/month today
                        };
                        db.Budgets.Add(budget);

                        // insert into BudgetsDetails
                        var detail = new Models.BudgetsDetails
                        {
                            Id = Guid.NewGuid(),
                            TypeId = typeId,
                            FromId = budget.Id,
                            ProjectName = projek,
                            BA = ba,
                            TotalAmount = jumlah.Value,
                            Jan = TryParseDecimal(row.GetCell(4)),
                            Feb = TryParseDecimal(row.GetCell(5)),
                            Mar = TryParseDecimal(row.GetCell(6)),
                            Apr = TryParseDecimal(row.GetCell(7)),
                            May = TryParseDecimal(row.GetCell(8)),
                            Jun = TryParseDecimal(row.GetCell(9)),
                            Jul = TryParseDecimal(row.GetCell(10)),
                            Aug = TryParseDecimal(row.GetCell(11)),
                            Sep = TryParseDecimal(row.GetCell(12)),
                            Oct = TryParseDecimal(row.GetCell(13)),
                            Nov = TryParseDecimal(row.GetCell(14)),
                            Dec = TryParseDecimal(row.GetCell(15)),
                            CreatedDate = DateTime.Now,
                            CreatedBy = currentUserId
                        };
                        db.BudgetsDetails.Add(detail);
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error importing Excel: " + ex.Message);
                throw; // let caller handle
            }
        }


        // Helper: safe parse decimal
        private decimal? TryParseDecimal(ICell cell)
        {
            if (cell == null) return null;
            if (cell.CellType == CellType.Numeric)
                return (decimal)cell.NumericCellValue;
            if (decimal.TryParse(cell.ToString().Replace(",", ""), out decimal result))
                return result;
            return null;
        }

        // Helper: safe parse int
        private int? TryParseInt(ICell cell)
        {
            if (cell == null) return null;
            if (cell.CellType == CellType.Numeric)
                return (int)cell.NumericCellValue;
            if (int.TryParse(cell.ToString(), out int result))
                return result;
            return null;
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
          
        #endregion
    }
}