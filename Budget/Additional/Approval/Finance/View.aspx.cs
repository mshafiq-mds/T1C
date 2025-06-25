using FGV.Prodata.Web.UI;
using Org.BouncyCastle.Asn1.Ocsp;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Budget.Additional.Approval.Finance
{
    public partial class View : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string idStr = Request.QueryString["id"];
                if (Guid.TryParse(idStr, out Guid requestId))
                {
                    LoadBudgetRequest(requestId);
                    LoadDocument(requestId);
                    Loadhistory(requestId);
                }
                else
                {
                    Response.Redirect("~/Budget/Additional/Approval/Finance");
                }
            }
        }

        private void LoadBudgetRequest(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var model = db.AdditionalBudgetRequests.FirstOrDefault(x => x.Id == id);
                if (model == null)
                {
                    Response.Redirect("~/Budget/Additional/Approval/Finance");
                    return;
                }

                LblBA.Text = model.BA ?? "-";
                LblBAName.Text = new Class.IPMSBizArea().GetNameByCode(model.BA ?? "") ?? "-";

                lblBudgetType.Text = model.BudgetType ?? "-";
                lblProject.Text = model.Project ?? "-";
                lblRefNo.Text = model.RefNo ?? "-";
                lblDate.Text = model.ApplicationDate.ToString("yyyy-MM-dd");

                lblBudgetEstimate.Text = model.EstimatedCost.ToString("N2");

                lblEVisa.Text = string.IsNullOrWhiteSpace(model.EVisaNo) ? "-" : model.EVisaNo;

                lblRequestDetails.Text = string.IsNullOrWhiteSpace(model.RequestDetails) ? "-" : model.RequestDetails;
                lblReason.Text = string.IsNullOrWhiteSpace(model.Reason) ? "-" : model.Reason;

                lblCostCentre.Text = string.IsNullOrWhiteSpace(model.CostCentre) ? "-" : model.CostCentre;
                lblGL.Text = string.IsNullOrWhiteSpace(model.GLCode) ? "-" : model.GLCode;

                lblApprovedBudget.Text = model.ApprovedBudget.HasValue ? model.ApprovedBudget.Value.ToString("N2") : "-";
                lblNewTotalBudget.Text = model.NewTotalBudget.HasValue ? model.NewTotalBudget.Value.ToString("N2") : "-";
                lblAdditionalBudget.Text = model.AdditionalBudget.HasValue ? model.AdditionalBudget.Value.ToString("N2") : "-";

                lblCheckType.Text = model.CheckType;

            }
        }


        private void LoadDocument(Guid transferId)
        {
            using (var db = new AppDbContext())
            {
                var documents = db.AdditionalBudgetDocuments
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
                            NavigateUrl = "~/DocumentHandler.ashx?id=" + doc.Id + "&module=AdditionalBudgetDocuments",
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
        private void Loadhistory(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var query = db.AdditionalBudgetLog
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