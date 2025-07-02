using FGV.Prodata.Web.UI;
using NPOI.SS.Formula.Functions;
using Prodata.WebForm.Models;
using Prodata.WebForm.Class;
using Prodata.WebForm.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.Transfer
{
    public partial class Add : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControl();
                BindBALabel();

                txtEVisa.Text = txtRefNo.Text = Functions.GetGeneratedRefNo("PB", true);
                txtEVisa.ReadOnly = txtRefNo.ReadOnly = true;

                txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            }
        }
        protected void btnSubmit_Click1(object sender, EventArgs e)
        {
            Guid newId;

            using (var db = new AppDbContext())
            {
                do
                {
                    newId = Guid.NewGuid();
                } while (db.TransfersTransaction.Any(x => x.Id == newId));

                string refNo = Functions.GetGeneratedRefNo("PB", false);

                if(txtRefNo.Text != refNo)
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Ref No. " + txtRefNo.Text + " already exists and has been updated to a new Ref No: " + refNo + ".");

                var model = new TransfersTransaction
                {
                    BA = Auth.User().iPMSBizAreaCode,
                    Id = newId,
                    RefNo = refNo,
                    Project = txtProject.Text.Trim(),
                    Date = string.IsNullOrWhiteSpace(txtDate.Text) ? DateTime.Today : DateTime.Parse(txtDate.Text),
                    BudgetType = rdoOpex.Checked ? "OPEX" : "CAPEX",
                    EstimatedCost = string.IsNullOrWhiteSpace(txtEstimatedCost.Text) ? 0 : Convert.ToDecimal(txtEstimatedCost.Text),
                    Justification = txtJustification.Text.Trim(),
                    EVisaNo = refNo,
                    WorkDetails = txtWorkDetails.Text.Trim(),

                    FromGL = txtFromGL.Text.Trim(),
                    FromBA = ddFromBA.SelectedValue,
                    FromBudget = string.IsNullOrWhiteSpace(txtFromBudget.Text) ? 0 : Convert.ToDecimal(txtFromBudget.Text),
                    FromBalance = string.IsNullOrWhiteSpace(txtFromBalance.Text) ? 0 : Convert.ToDecimal(txtFromBalance.Text),
                    FromTransfer = string.IsNullOrWhiteSpace(txtFromTransfer.Text) ? 0 : Convert.ToDecimal(txtFromTransfer.Text),
                    FromAfter = string.IsNullOrWhiteSpace(txtFromAfter.Text) ? 0 : Convert.ToDecimal(txtFromAfter.Text),

                    ToGL = txtToGL.Text.Trim(),
                    ToBA = lblToBA.Text.Trim(),
                    ToBudget = string.IsNullOrWhiteSpace(txtToBudget.Text) ? 0 : Convert.ToDecimal(txtToBudget.Text),
                    ToBalance = string.IsNullOrWhiteSpace(txtToBalance.Text) ? 0 : Convert.ToDecimal(txtToBalance.Text),
                    ToTransfer = string.IsNullOrWhiteSpace(txtToTransfer.Text) ? 0 : Convert.ToDecimal(txtToTransfer.Text),
                    ToAfter = string.IsNullOrWhiteSpace(txtToAfter.Text) ? 0 : Convert.ToDecimal(txtToAfter.Text),
                    status = 1,
                    //Nota
                    //status == 0 ? "Resubmit" :
                    //status == 1 ? "Submitted" :
                    //status == 2 ? "Under Review" :
                    //status == 3 ? "Completed" :

                    CreatedBy = Auth.Id(),  
                    CreatedDate = DateTime.Now
                };

                db.TransfersTransaction.Add(model);
                db.SaveChanges();

                if (fuDocument.HasFile)
                {
                    using (var binaryReader = new System.IO.BinaryReader(fuDocument.PostedFile.InputStream))
                    {
                        byte[] fileData = binaryReader.ReadBytes(fuDocument.PostedFile.ContentLength);

                        var document = new TransferDocument
                        {
                            Id = Guid.NewGuid(),
                            TransferId = newId,
                            FileName = fuDocument.FileName,
                            ContentType = fuDocument.PostedFile.ContentType,
                            FileData = fileData,
                            UploadedBy = Auth.Id(),
                            UploadedDate = DateTime.Now
                        };

                        db.TransferDocuments.Add(document);
                        db.SaveChanges();
                    }
                }
                Emails.EmailsReqTransferBudget(newId, model, Auth.User().iPMSRoleCode);

                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Transfer Budget added.");
                Response.Redirect("~/Budget/Transfer");
            }
        }

        private void BindBALabel()
        {
            string ba = Auth.User().iPMSBizAreaCode;

            bool isEmptyBA = string.IsNullOrWhiteSpace(ba);

            phBA.Visible = !isEmptyBA;
            phBADropdown.Visible = isEmptyBA;

            if (!isEmptyBA) 
            {
                LblBA.Text = 
                    lblToBA.Text = ba;
                LblBAName.Text = new Class.IPMSBizArea().GetNameByCode(ba) ?? "";
            }
        }

        private void BindControl()
        {
            ddFromBA.DataSource = new Class.IPMSBizArea().GetIPMSBizAreas();
            ddFromBA.DataValueField = "Code";
            ddFromBA.DataTextField = "DisplayName";
            ddFromBA.DataBind();
            ddFromBA.Items.Insert(0, new ListItem("", ""));
        }
    }
}