using FGV.Prodata.Web.UI;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.ModelAWO;
using System;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.AssetWriteOff
{
    public partial class Edit : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["id"] != null && Guid.TryParse(Request.QueryString["id"], out Guid writeOffId))
                {
                    LoadExistingData(writeOffId);
                }
                else
                {
                    Response.Redirect("~/AssetWriteOff/Default.aspx");
                }
            }
        }

        private void LoadExistingData(Guid writeOffId)
        {
            string baCode = Auth.User().CCMSBizAreaCode;
            string baName = new Class.IPMSBizArea().GetNameByCode(baCode) ?? "";
            lblBA.Text = $"{baCode} - {baName}";

            using (var db = new AppDbContext())
            {
                // 1. Load Master Record
                var master = db.AssetWriteOffs.Find(writeOffId);

                // Security Check: Only the creator can edit, and only if it's Sent Back (Clarification)
                if (master == null ||  master.Status != "SentBack")
                {
                    Response.Redirect("~/AssetWriteOff/Default.aspx");
                    return;
                }

                txtRequestNo.Text = master.RequestNo;
                txtDate.Text = master.Date.ToString("yyyy-MM-dd");
                txtProject.Text = master.Project;
                txtJustification.Text = master.Justification;

                // 2. Load Existing Asset Details into the HTML Table via Literal
                var details = db.AssetWriteOffDetails.Where(d => d.WriteOffId == writeOffId).ToList();
                StringBuilder sbRows = new StringBuilder();

                int rowIndex = 1;
                foreach (var item in details)
                {
                    string acqDateStr = item.AcqDate?.ToString("yyyy-MM-dd") ?? "";

                    sbRows.Append("<tr class='bg-light'>");
                    sbRows.Append($"<td class='text-center font-weight-bold row-number align-middle'>{rowIndex}</td>");
                    sbRows.Append($"<td><input type='text' name='AssetCode' class='form-control-table' oninput=\"this.value = this.value.replace(/[^0-9]/g, '');\" value='{item.AssetCode}' /></td>");
                    sbRows.Append($"<td><input type='text' name='ItemDescription' class='form-control-table' style='text-transform: uppercase;' oninput=\"this.value = this.value.toUpperCase();\" value='{item.ItemDescription}' /></td>");
                    sbRows.Append($"<td><input type='date' name='AcqDate' class='form-control-table' onchange='calculateAge(this)' value='{acqDateStr}' /></td>");
                    sbRows.Append($"<td><input type='number' name='AgeYears' class='form-control-table input-number txtreadonly' value='{item.AgeYears}' readonly /></td>");
                    sbRows.Append($"<td><input type='number' name='UsefulLife' class='form-control-table input-number' value='{item.UsefulLife}' /></td>");
                    sbRows.Append($"<td><input type='number' name='Quantity' class='form-control-table input-number' value='{item.Quantity}' /></td>");
                    sbRows.Append($"<td><input type='text' name='OriginalPrice' class='form-control-table input-number calc-orig text-primary font-weight-bold' oninput='formatCurrencyInput(this); calculateLiveValues();' onblur='formatCurrencyInput(this, true); calculateLiveValues();' value='{item.OriginalPrice:0.00}' /></td>");
                    sbRows.Append($"<td><input type='text' name='AccDepreciation' class='form-control-table input-number calc-acc text-danger font-weight-bold' oninput='formatCurrencyInput(this); calculateLiveValues();' onblur='formatCurrencyInput(this, true); calculateLiveValues();' value='{item.AccDepreciation:0.00}' /></td>");
                    sbRows.Append($"<td><input type='text' name='NetBookValue' class='form-control-table input-number txtreadonly calc-net text-success font-weight-bold' value='{item.NetBookValue:0.00}' readonly /></td>");
                    sbRows.Append($"<td><textarea name='Reason' class='form-control-table' rows='2' style='resize:vertical;'>{item.Reason}</textarea></td>");
                    sbRows.Append($"<td class='text-center align-middle'><button type='button' class='btn btn-outline-danger btn-sm' onclick='removeRow(this)'><i class='fas fa-trash'></i></button></td>");
                    sbRows.Append("</tr>");

                    rowIndex++;
                }
                litExistingRows.Text = sbRows.ToString();

                // 3. Load Documents
                var documents = db.AssetWriteOffDocuments.Where(d => d.WriteOffId == writeOffId).ToList();
                if (documents.Any())
                {
                    pnlExistingDocs.Visible = true;
                    foreach (var doc in documents)
                    {
                        var link = new HyperLink
                        {
                            NavigateUrl = $"~/DocumentHandler.ashx?id={doc.Id}&module=AssetWriteOffDocuments",
                            Target = "_blank",
                            Text = $"<i class='fas fa-download'></i> {doc.FileName} (Download to view)",
                            CssClass = "d-block text-primary mb-1"
                        };
                        phDocumentList.Controls.Add(link);
                    }
                }

                // 4. Load History / Approver Remarks
                var history = db.AssetWriteOffApprovalLogs
                                .Where(h => h.WriteOffId == writeOffId)
                                .OrderBy(h => h.ActionDate)
                                .ToList();
                gvHistory.DataSource = history;
                gvHistory.DataBind();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Guid.TryParse(Request.QueryString["id"], out Guid writeOffId)) return;

            string[] assetCodes = Request.Form.GetValues("AssetCode");
            string[] itemDescriptions = Request.Form.GetValues("ItemDescription");
            string[] acqDates = Request.Form.GetValues("AcqDate");
            string[] ageYears = Request.Form.GetValues("AgeYears");
            string[] usefulLifes = Request.Form.GetValues("UsefulLife");
            string[] quantities = Request.Form.GetValues("Quantity");
            string[] originalPrices = Request.Form.GetValues("OriginalPrice");
            string[] accDepreciations = Request.Form.GetValues("AccDepreciation");
            string[] reasons = Request.Form.GetValues("Reason");

            if (assetCodes == null || assetCodes.Length == 0 || assetCodes.All(string.IsNullOrWhiteSpace))
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Please enter at least one valid asset.");
                return;
            }

            decimal highestNetBookValue = 0;
            Guid currentUserId = Auth.User().Id;

            using (var db = new AppDbContext())
            {
                var master = db.AssetWriteOffs.Find(writeOffId);
                if (master == null) return;

                // 1. Update Master Details & Reset Workflow
                master.Project = txtProject.Text.Trim();
                master.Justification = txtJustification.Text.Trim();
                master.Status = "Pending"; // Puts it back in the queue
                master.CurrentApprovalLevel = 1; // Resets routing to the first approver
                master.UpdatedBy = currentUserId;
                master.UpdatedDate = DateTime.Now;

                // 2. WIPE OUT Old Details and REPLACE with new array data
                var oldDetails = db.AssetWriteOffDetails.Where(d => d.WriteOffId == writeOffId);
                db.AssetWriteOffDetails.RemoveRange(oldDetails);

                for (int i = 0; i < assetCodes.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(assetCodes[i]) && string.IsNullOrWhiteSpace(itemDescriptions[i])) continue;

                    decimal.TryParse(originalPrices[i], out decimal originalPrice);
                    decimal.TryParse(accDepreciations[i], out decimal accDepreciation);
                    decimal netBookValue = originalPrice - accDepreciation;

                    if (netBookValue > highestNetBookValue)
                    {
                        highestNetBookValue = netBookValue;
                    }

                    var detail = new AssetWriteOffDetail
                    {
                        Id = Guid.NewGuid(),
                        WriteOffId = master.Id,
                        AssetCode = assetCodes[i],
                        ItemDescription = itemDescriptions[i].ToUpper(),
                        AcqDate = string.IsNullOrWhiteSpace(acqDates[i]) ? (DateTime?)null : DateTime.Parse(acqDates[i]),
                        AgeYears = int.TryParse(ageYears[i], out int age) ? age : 0,
                        UsefulLife = int.TryParse(usefulLifes[i], out int ul) ? ul : 0,
                        Quantity = int.TryParse(quantities[i], out int qty) ? qty : 1,
                        OriginalPrice = originalPrice,
                        AccDepreciation = accDepreciation,
                        NetBookValue = netBookValue,
                        Reason = reasons[i]
                    };
                    db.AssetWriteOffDetails.Add(detail);
                }

                master.NetBookValue = highestNetBookValue;

                // 3. Create Audit Log for "Resubmission"
                var resubmitLog = new Models.ModelAWO.AssetWriteOffApprovalLog
                {
                    Id = Guid.NewGuid(),
                    WriteOffId = master.Id,
                    StepNumber = 1,
                    RoleName = Auth.User().CCMSRoleCode,
                    UserId = currentUserId,
                    ActionType = "Re-Submit",
                    ActionDate = DateTime.Now,
                    Status = "Submitted",
                    Remarks = "Application amended and re-submitted.",
                    CreatedBy = currentUserId,
                    CreatedDate = DateTime.Now
                };
                db.AssetWriteOffApprovalLogs.Add(resubmitLog);

                // 4. Handle Document Upload (Only if they chose a NEW file)
                var fuDocument = (System.Web.UI.WebControls.FileUpload)Master.FindControl("MainContent").FindControl("fuDocument");
                if (fuDocument != null && fuDocument.HasFile)
                {
                    // Wipe out the old document first so we don't clog the database
                    var oldDocs = db.AssetWriteOffDocuments.Where(d => d.WriteOffId == writeOffId);
                    db.AssetWriteOffDocuments.RemoveRange(oldDocs);

                    using (var binaryReader = new System.IO.BinaryReader(fuDocument.PostedFile.InputStream))
                    {
                        byte[] fileData = binaryReader.ReadBytes(fuDocument.PostedFile.ContentLength);

                        var newDoc = new Models.ModelAWO.AssetWriteOffDocument
                        {
                            Id = Guid.NewGuid(),
                            WriteOffId = master.Id,
                            FileName = fuDocument.FileName,
                            ContentType = fuDocument.PostedFile.ContentType,
                            FileData = fileData,
                            UploadedBy = currentUserId,
                            UploadedDate = DateTime.Now
                        };
                        db.AssetWriteOffDocuments.Add(newDoc);
                    }
                }

                db.SaveChanges();

                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Asset Write-Off Request re-submitted successfully.");

                Class.AWOEmails.SendToNextApprover(master.Id);

                Response.Redirect("~/AssetWriteOff/Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }
    }
}