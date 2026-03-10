using FGV.Prodata.Web.UI;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.ModelAWO;
using System;
using System.Linq;

namespace Prodata.WebForm.AssetWriteOff
{
    public partial class Add : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string baCode = Auth.User().CCMSBizAreaCode;
                string baName = new Class.IPMSBizArea().GetNameByCode(baCode) ?? "";

                lblBA.Text = $"{baCode} - {baName}";
                txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtRequestNo.Text = Functions.GetGeneratedRefNo("WO", true);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
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

            Guid newWriteOffId = Guid.NewGuid();
            decimal highestNetBookValue = 0;
            Guid currentUserId = Auth.User().Id;

            using (var db = new AppDbContext())
            {
                // 1. Create Master
                var master = new Models.ModelAWO.AssetWriteOff
                {
                    Id = newWriteOffId,
                    RequestNo = Functions.GetGeneratedRefNo("AW", false),
                    Project = txtProject.Text.Trim(),
                    BACode = Auth.User().CCMSBizAreaCode,
                    Date = string.IsNullOrWhiteSpace(txtDate.Text) ? DateTime.Today : DateTime.Parse(txtDate.Text),
                    Justification = txtJustification.Text.Trim(),
                    Status = "Submitted",
                    CurrentApprovalLevel = 1,
                    CreatedBy = currentUserId,
                    CreatedDate = DateTime.Now
                };
                db.AssetWriteOffs.Add(master);

                // 2. Create Details
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
                        WriteOffId = newWriteOffId,
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

                // 3. Create Audit Log
                var submitLog = new Models.ModelAWO.AssetWriteOffApprovalLog
                {
                    Id = Guid.NewGuid(),
                    WriteOffId = newWriteOffId,
                    StepNumber = 1,
                    RoleName = Auth.User().CCMSRoleCode,
                    UserId = currentUserId,
                    ActionType = "Submit",
                    ActionDate = DateTime.Now,
                    Status = "Submitted",
                    Remarks = "Application submitted for approval.",
                    CreatedBy = currentUserId,
                    CreatedDate = DateTime.Now
                };
                db.AssetWriteOffApprovalLogs.Add(submitLog);

                // 4. FIX: SAVE DOCUMENT (Was missing in your code block)
                // Assuming your FileUpload control ID is fuDocument
                var fuDocument = (System.Web.UI.WebControls.FileUpload)Master.FindControl("MainContent").FindControl("fuDocument");
                if (fuDocument != null && fuDocument.HasFile)
                {
                    using (var binaryReader = new System.IO.BinaryReader(fuDocument.PostedFile.InputStream))
                    {
                        byte[] fileData = binaryReader.ReadBytes(fuDocument.PostedFile.ContentLength);

                        var doc = new Models.ModelAWO.AssetWriteOffDocument
                        {
                            Id = Guid.NewGuid(),
                            WriteOffId = newWriteOffId,
                            FileName = fuDocument.FileName,
                            ContentType = fuDocument.PostedFile.ContentType,
                            FileData = fileData,
                            UploadedBy = currentUserId,
                            UploadedDate = DateTime.Now
                        };
                        db.AssetWriteOffDocuments.Add(doc);
                    }
                }

                // 5. Commit all
                db.SaveChanges();

                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Asset Write-Off Request submitted successfully.");
                Response.Redirect("~/AssetWriteOff/Default", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }
    }
}