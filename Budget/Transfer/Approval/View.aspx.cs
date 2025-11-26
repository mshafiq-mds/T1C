using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.Transfer.Approval
{
    public partial class View : ProdataPage
    {
        private Guid _transferId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Guid.TryParse(Request.QueryString["Id"], out _transferId))
                {
                    LoadTransfer(_transferId);
                    LoadDocument(_transferId);
                    Loadhistory(_transferId);
                }
                else
                {
                    Response.Redirect("~/Budget/Transfer/Approval");
                }
            }
        }
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

                        var link = new HyperLink
                        {
                            NavigateUrl = "~/DocumentHandler.ashx?id=" + doc.Id + "&module=TransferDocuments",
                            CssClass = "btn btn-outline-success mr-2",
                            Target = "_blank",
                            Text = "<i class='fas fa-external-link-alt'></i> View"
                        };

                        var fileLabel = new Label
                        {
                            Text = $"<i class='fas fa-file-alt text-success mr-2'></i> {doc.FileName}",
                            CssClass = "text-dark font-weight-bold",
                            EnableViewState = false
                        };

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
        private void LoadTransfer(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var transfer = db.TransfersTransaction.FirstOrDefault(x => x.Id == id);
                if (transfer == null)
                {
                    Response.Redirect("~/Budget/Transfer/Approval");
                    return;
                }

                // Header
                lblRefNo.Text = transfer.RefNo;
                lblProject.Text = transfer.Project;
                lblDate.Text = transfer.Date.ToString("yyyy-MM-dd");
                lblEstimatedCost.Text = transfer.EstimatedCost.ToString("F2");
                lblEVisa.Text = transfer.EVisaNo;
                lblBudgetType.Text = transfer.BudgetType;
                lblBA.Text = transfer.BA;

                // From Budget
                Guid FromBudgetTypeGuid = transfer.FromBudgetType;
                var FromBudgetType = db.BudgetTypes
                    .Where(x => x.Id == FromBudgetTypeGuid)
                    .Select(x => x.Name)
                    .FirstOrDefault();

                lblFromBudgetType.Text = FromBudgetType ?? "Unknown";
                lblFromBA.Text = transfer.FromBA;
                lblFromBudget.Text = (transfer.FromBudget ?? 0).ToString("F2");
                lblFromBalance.Text = (transfer.FromBalance ?? 0).ToString("F2");
                lblFromTransfer.Text = (transfer.FromTransfer ?? 0).ToString("F2");
                lblFromAfter.Text = (transfer.FromAfter ?? 0).ToString("F2");
                lblFromGL.Text = transfer.FromGL;

                // To Budget
                Guid ToBudgetTypeGuid = transfer.ToBudgetType;
                var ToBudgetType = db.BudgetTypes
                    .Where(x => x.Id == ToBudgetTypeGuid)
                    .Select(x => x.Name)
                    .FirstOrDefault();
                lblToBudgetType.Text = ToBudgetType ?? "Unknown";
                lblToBA.Text = transfer.ToBA;
                lblToBudget.Text = (transfer.ToBudget ?? 0).ToString("F2");
                lblToBalance.Text = (transfer.ToBalance ?? 0).ToString("F2");
                lblToTransfer.Text = (transfer.ToTransfer ?? 0).ToString("F2");
                lblToAfter.Text = (transfer.ToAfter ?? 0).ToString("F2");
                lblToGL.Text = transfer.ToGL;

                // Justification
                litJustification.Text = Server.HtmlEncode(transfer.Justification)?.Replace("\n", "<br/>");
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

                gvHistory.DataSource = transfers;
                gvHistory.DataBind();
            }
        }
    }
}