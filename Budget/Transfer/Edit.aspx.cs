using FGV.Prodata.Web.UI;
using Prodata.WebForm.Helpers;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.Transfer
{
    public partial class Edit : ProdataPage
    {
        private Guid _transferId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Guid.TryParse(Request.QueryString["Id"], out _transferId))
                {
                    LoadTransfer(_transferId);
                    //BindControl();
                    LoadDocument(_transferId);
                    Loadhistory(_transferId);
                }
                else
                {
                    Response.Redirect("~/Budget/Transfer");
                }
            }
        }

        private void LoadTransfer(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var transfer = db.TransfersTransaction.FirstOrDefault(x => x.Id == id);
                if (transfer == null)
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Transfer not found.");
                    Response.Redirect("~/Budget/Transfer");
                    return;
                }

                txtRefNo.Text = transfer.RefNo;
                txtProject.Text = transfer.Project;
                txtDate.Text = transfer.Date.ToString("yyyy-MM-dd");
                txtEstimatedCost.Text = transfer.EstimatedCost.ToString("F2");
                txtEVisa.Text = transfer.EVisaNo;
                txtWorkDetails.Text = transfer.WorkDetails;
                txtJustification.Text = transfer.Justification;

                rdoOpex.Checked = transfer.BudgetType == "OPEX";
                rdoCapex.Checked = transfer.BudgetType == "CAPEX";


                Guid fromGLGuid = transfer.FromGL;
                var fromGLBudgetType = db.BudgetTypes
                    .Where(x => x.Id == fromGLGuid)
                    .Select(x => x.Name)
                    .FirstOrDefault();
                txtFromGL.Text = fromGLBudgetType ?? "Unknown";
                ddFromBA.Text = transfer.FromBA;
                ddFromBAName.Text = new Class.IPMSBizArea().GetNameByCode(transfer.FromBA);
                txtFromBudget.Text = (transfer.FromBudget ?? 0).ToString("F2");
                txtFromBalance.Text = (transfer.FromBalance ?? 0).ToString("F2");
                txtFromTransfer.Text = (transfer.FromTransfer ?? 0).ToString("F2");
                txtFromAfter.Text = (transfer.FromAfter ?? 0).ToString("F2");

                Guid toGLGuid = transfer.ToGL;
                var toGLBudgetType = db.BudgetTypes
                    .Where(x => x.Id == toGLGuid)
                    .Select(x => x.Name)
                    .FirstOrDefault();
                txtToGL.Text = toGLBudgetType ?? "Unknown";
                ddToBA.Text = transfer.ToBA;
                ddToBAName.Text = new Class.IPMSBizArea().GetNameByCode(transfer.ToBA);
                txtToBudget.Text = (transfer.ToBudget ?? 0).ToString("F2");
                txtToBalance.Text = (transfer.ToBalance ?? 0).ToString("F2");
                txtToTransfer.Text = (transfer.ToTransfer ?? 0).ToString("F2");
                txtToAfter.Text = (transfer.ToAfter ?? 0).ToString("F2");
                LblBA.Text = transfer.BA;
            }
        }

        protected void btnSubmit_Click1(object sender, EventArgs e)
        {
            if (Guid.TryParse(Request.QueryString["Id"], out _transferId))
            {
                using (var db = new AppDbContext())
                {
                    var model = db.TransfersTransaction.FirstOrDefault(x => x.Id == _transferId);
                    if (model == null)
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Transfer not found.");
                        return;
                    }

                    //model.RefNo = txtRefNo.Text.Trim();
                    //model.Project = txtProject.Text.Trim();
                    //model.Date = DateTime.Parse(txtDate.Text);
                    //model.BudgetType = rdoOpex.Checked ? "OPEX" : "CAPEX";
                    //model.EstimatedCost = string.IsNullOrWhiteSpace(txtEstimatedCost.Text) ? 0 : Convert.ToDecimal(txtEstimatedCost.Text);
                    //model.EVisaNo = txtEVisa.Text.Trim();
                    //model.WorkDetails = txtWorkDetails.Text.Trim();
                    //model.Justification = txtJustification.Text.Trim();

                    //model.FromGL = Guid.Parse(txtFromGL.Text.Trim());
                    //model.FromBA = ddFromBA.SelectedValue;
                    //model.FromBudget = Convert.ToDecimal(txtFromBudget.Text);
                    //model.FromBalance = Convert.ToDecimal(txtFromBalance.Text);
                    //model.FromTransfer = Convert.ToDecimal(txtFromTransfer.Text);
                    //model.FromAfter = Convert.ToDecimal(txtFromAfter.Text);

                    //model.ToGL = Guid.Parse(txtToGL.Text.Trim());
                    //model.ToBA = ddToBA.SelectedValue;
                    //model.ToBudget = Convert.ToDecimal(txtToBudget.Text);
                    //model.ToBalance = Convert.ToDecimal(txtToBalance.Text);
                    //model.ToTransfer = Convert.ToDecimal(txtToTransfer.Text);
                    //model.ToAfter = Convert.ToDecimal(txtToAfter.Text);

                    model.UpdatedBy = Auth.User().Id; // Or your method to get current user
                    model.UpdatedDate = DateTime.Now;

                    db.SaveChanges();

                    if (fuDocument.HasFile)
                    {
                        using (var binaryReader = new System.IO.BinaryReader(fuDocument.PostedFile.InputStream))
                        {
                            byte[] fileData = binaryReader.ReadBytes(fuDocument.PostedFile.ContentLength);

                            // Check if a document already exists
                            // Always insert a new document
                            var newDoc = new TransferDocument
                            {
                                Id = Guid.NewGuid(),
                                TransferId = _transferId,
                                FileName = fuDocument.FileName,
                                ContentType = fuDocument.PostedFile.ContentType,
                                FileData = fileData,
                                UploadedBy = Auth.Id(),
                                UploadedDate = DateTime.Now
                            };
                            db.TransferDocuments.Add(newDoc);

                            db.SaveChanges();
                        }
                    }

                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Transfer Budget updated.");
                    Response.Redirect("~/Budget/Transfer");
                }
            }
        }

        //private void BindControl()
        //{
        //    ddFromBA.DataSource = new Class.IPMSBizArea().GetIPMSBizAreas();
        //    ddFromBA.DataValueField = "Code";
        //    ddFromBA.DataTextField = "DisplayName";
        //    ddFromBA.DataBind();
        //    ddFromBA.Items.Insert(0, new ListItem("", ""));

        //    ddToBA.DataSource = new Class.IPMSBizArea().GetIPMSBizAreas();
        //    ddToBA.DataValueField = "Code";
        //    ddToBA.DataTextField = "DisplayName";
        //    ddToBA.DataBind();
        //    ddToBA.Items.Insert(0, new ListItem("", ""));
        //}
        private void LoadDocument(Guid transferId)
        {
            using (var db = new AppDbContext())
            {
                var documents = db.TransferDocuments
                    .Where(d => d.TransferId == transferId)
                    .OrderByDescending(d => d.UploadedDate)
                    .ToList();

                if (documents.Any())
                {
                    pnlUploadedDocument.Visible = true;
                    phDocumentList.Controls.Clear();

                    foreach (var doc in documents)
                    {
                        var panel = new Panel { CssClass = "mb-2 d-flex align-items-center" };

                        // View link
                        var link = new HyperLink
                        {
                            NavigateUrl = "~/DocumentHandler.ashx?id=" + doc.Id + "&module=TransferDocuments",
                            CssClass = "btn btn-outline-success mr-2",
                            Target = "_blank",
                            Text = "<i class='fas fa-external-link-alt'></i> View"
                        };

                        // File name
                        var fileLabel = new Label
                        {
                            Text = $"<i class='fas fa-file-alt text-success mr-2'></i> {doc.FileName}",
                            CssClass = "text-dark font-weight-bold",
                            EnableViewState = false
                        };

                        // Add View then FileName (both on left)
                        panel.Controls.Add(link);
                        panel.Controls.Add(fileLabel);
                        phDocumentList.Controls.Add(panel);
                    }
                }
                else
                {
                    pnlUploadedDocument.Visible = false;
                }
            }
        }

        private void Loadhistory(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var query = db.TransferApprovalLog
                              .Where(x => x.DeletedDate == null && x.BudgetTransferId == id);

                var transfers = query
                    .OrderByDescending(x => x.ActionDate)
                    .Select(x => new
                    {
                        x.Id,
                        x.ActionDate,
                        x.ActionType,
                        x.RoleName,
                        x.Status,
                        x.Remarks
                    })
                    .ToList();

                if (transfers.Any())
                {
                    pnHistoryApproval.Visible = true;
                }
                gvHistory.DataSource = transfers;
                gvHistory.DataBind();
            }
        }
    }
}