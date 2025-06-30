using FGV.Prodata.Web.UI;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.AddBudget
{
    public partial class Add : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindBALabel();

                txtEVisa.Text = txtRefNo.Text = Functions.GetGeneratedRefNo("TB", true);
                txtEVisa.Enabled = txtRefNo.Enabled = false;

                txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Guid newId;

            using (var db = new AppDbContext())
            {
                do
                {
                    newId = Guid.NewGuid();
                } while (db.AdditionalBudgetRequests.Any(x => x.Id == newId));

                string refNo = Functions.GetGeneratedRefNo("PB", false);

                var model = new AdditionalBudgetRequests
                {
                    Id = newId,

                    // Application Metadata
                    RefNo = refNo,
                    Project = txtProject.Text.Trim(),
                    ApplicationDate = DateTime.TryParse(txtDate.Text, out var appDate) ? appDate : DateTime.Today,
                    BudgetType = rdoOpex.Checked ? "OPEX" : "CAPEX",
                    CheckType = rdoFinance.Checked ? "FINANCE" : "COGS",
                    EstimatedCost = decimal.TryParse(txtBudgetEstimate.Text, out var estimated) ? estimated : 0,
                    EVisaNo = refNo,

                    // Main Justification
                    RequestDetails = txtRequestDetails.Text.Trim(),
                    Reason = txtReason.Text.Trim(),

                    // Additional Budget Breakdown
                    CostCentre = txtCostCentre.Text.Trim(),
                    GLCode = txtGL.Text.Trim(),
                    ApprovedBudget = decimal.TryParse(txtApprovedBudget.Text, out var approved) ? approved : 0,
                    NewTotalBudget = decimal.TryParse(txtNewTotalBudget.Text, out var newTotal) ? newTotal : 0,
                    AdditionalBudget = decimal.TryParse(txtAdditionalBudget.Text, out var additional) ? additional : 0,

                    // Tracking
                    BA = LblBA.Text, // Static or you can bind dynamically
                    Status = 1,
                    //Nota
                    //status == 0 ? "Resubmit" :
                    //status == 1 ? "Submitted" :
                    //status == 2 ? "Under Review" :
                    //status == 3 ? "Completed" :

                    CreatedBy = Auth.Id(),
                    CreatedDate = DateTime.Now
                };

                db.AdditionalBudgetRequests.Add(model);
                db.SaveChanges();

                if (fuDocument.HasFile)
                {
                    using (var binaryReader = new System.IO.BinaryReader(fuDocument.PostedFile.InputStream))
                    {
                        byte[] fileData = binaryReader.ReadBytes(fuDocument.PostedFile.ContentLength);

                        var document = new AdditionalBudgetDocuments
                        {
                            Id = Guid.NewGuid(),
                            TransferId = newId,
                            FileName = fuDocument.FileName,
                            ContentType = fuDocument.PostedFile.ContentType,
                            FileData = fileData,
                            UploadedBy = Auth.Id(),
                            UploadedDate = DateTime.Now
                        };

                        db.AdditionalBudgetDocuments.Add(document);
                        db.SaveChanges();
                    }
                }
            }

            SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Additional Budget Requested.");
            Response.Redirect("~/Budget/Additional");
        }
        private void BindBALabel()
        {
            string ba = Auth.User().iPMSBizAreaCode;

            bool isEmptyBA = string.IsNullOrWhiteSpace(ba);

            phBA.Visible = !isEmptyBA;
            phBADropdown.Visible = isEmptyBA;

            if (!isEmptyBA)
            {
                LblBA.Text = ba;
                LblBAName.Text = new Class.IPMSBizArea().GetNameByCode(ba) ?? "";
                //LblBAName.Text = new Class.IPMSBizArea().GetIPMSBizAreaNameByCode(Auth.User().iPMSBizAreaCode);
            }
        }
    }
}