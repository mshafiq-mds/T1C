using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.ModelAWO;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.AssetWriteOff
{
    public partial class View : ProdataPage
    {
        private Guid _writeOffId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Guid.TryParse(Request.QueryString["id"], out _writeOffId))
                {
                    LoadWriteOffData(_writeOffId);
                    LoadDocuments(_writeOffId);
                }
                else
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Invalid Application ID.");
                    Response.Redirect("~/AssetWriteOff/Default.aspx");
                }
            }
        }

        private void LoadWriteOffData(Guid id)
        {
            using (var db = new AppDbContext())
            {
                // 1. Fetch Master Data
                var master = db.AssetWriteOffs.FirstOrDefault(x => x.Id == id);
                if (master == null)
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Application record not found.");
                    Response.Redirect("~/AssetWriteOff/Default.aspx");
                    return;
                }

                // 2. Bind Header Data
                lblRefNo.Text = master.RequestNo;
                lblProject.Text = master.Project;
                lblDate.Text = master.Date.ToString("dd-MMM-yyyy");

                // Get Business Area Name
                var ipmsBA = new Class.IPMSBizArea();
                string baName = ipmsBA.GetNameByCode(master.BACode) ?? "Unknown BA";
                lblGlobalBA.Text = $"{master.BACode} - {baName}";
                lblPrintBA.Text = lblGlobalBA.Text; // Applied for printed version

                // Format Status Label
                string status = master.Status ?? "Unknown";
                lblStatus.Text = status == "UnderReview" ? "Under Review" : status;

                string statusClass = "view-data font-weight-bold px-3 py-1 rounded text-white d-inline-block ";
                if (status == "Rejected") statusClass += "bg-danger";
                else if (status == "UnderReview") statusClass += "bg-warning text-dark";
                else if (status == "Approved") statusClass += "bg-success";
                else statusClass += "bg-primary";
                lblStatus.CssClass = statusClass;

                litJustification.Text = Server.HtmlEncode(master.Justification)?.Replace("\n", "<br/>");

                // 3. Fetch Asset Details
                var details = db.AssetWriteOffDetails.Where(x => x.WriteOffId == id).OrderBy(x => x.AssetCode).ToList();

                if (details.Any())
                {
                    // Bind Grid
                    rptAssets.DataSource = details;
                    rptAssets.DataBind();

                    // Calculate Totals manually
                    decimal sumOrig = details.Sum(x => x.OriginalPrice);
                    decimal sumAcc = details.Sum(x => x.AccDepreciation);
                    decimal sumNet = details.Sum(x => x.NetBookValue);

                    lblTotalOrig.Text = sumOrig.ToString("N2");
                    lblTotalAcc.Text = sumAcc.ToString("N2");
                    lblTotalNet.Text = sumNet.ToString("N2");
                }

                // 4. Fetch Audit Trail / Workflow History
                var history = db.AssetWriteOffApprovalLogs
                                .Where(h => h.WriteOffId == id)
                                .OrderBy(h => h.ActionDate)
                                .ToList();
                gvHistory.DataSource = history;
                gvHistory.DataBind();
            }
        }

        private void LoadDocuments(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var documents = db.AssetWriteOffDocuments
                                  .Where(d => d.WriteOffId == id)
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
                            NavigateUrl = $"~/DocumentHandler.ashx?id={doc.Id}&module=AssetWriteOffDocuments",
                            CssClass = "btn btn-sm btn-outline-success mr-2",
                            Target = "_blank",
                            Text = "<i class='fas fa-download'></i> Download"
                        };

                        var fileLabel = new Label
                        {
                            Text = $"<i class='fas fa-file-alt text-secondary mr-2'></i> {doc.FileName}",
                            CssClass = "text-dark font-weight-bold",
                            EnableViewState = false
                        };

                        panel.Controls.Add(link);
                        panel.Controls.Add(fileLabel);
                        phDocumentList.Controls.Add(panel);
                    }
                }
            }
        }
    }
}