using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.ModelAWO;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.AssetWriteOff.Approver
{
    public partial class Edit : ProdataPage
    {
        private string CurrentUserRole => Auth.User().CCMSRoleCode;

        // Variables to calculate Repeater totals
        private decimal _totalOrig = 0;
        private decimal _totalAcc = 0;
        private decimal _totalNet = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["id"] != null && Guid.TryParse(Request.QueryString["id"], out Guid writeOffId))
                {
                    LoadData(writeOffId);
                }
                else
                {
                    Response.Redirect("~/AssetWriteOff/Approver/Default.aspx");
                }
            }
        }

        private void LoadData(Guid writeOffId)
        {
            using (var db = new AppDbContext())
            {
                // 1. Load Master Info
                var master = db.AssetWriteOffs.Find(writeOffId);
                if (master == null) return;

                lblRefNo.Text = master.RequestNo;
                lblGlobalBA.Text = master.BACode;
                lblDate.Text = master.Date.ToString("dd-MMM-yyyy");
                lblHighestNBV.Text = master.NetBookValue.ToString("N2");
                string status = master.Status ?? "Unknown";
                lblStatus.Text = status == "UnderReview" ? "Under Review" : status;
                lblGlobalBA.Text = master.BACode;
                lblPrintBA.Text = master.BACode;

                string statusClass = "view-data font-weight-bold px-3 py-1 rounded text-white d-inline-block ";
                if (status == "Rejected") statusClass += "bg-danger";
                else if (status == "UnderReview") statusClass += "bg-warning text-dark";
                else if (status == "Approved") statusClass += "bg-success";
                else statusClass += "bg-primary";
                lblStatus.CssClass = statusClass;

                // SPLIT Project and Justification variables
                lblProject.Text = string.IsNullOrEmpty(master.Project) ? "-" : master.Project;
                litJustification.Text = string.IsNullOrEmpty(master.Justification) ? "-" : Server.HtmlEncode(master.Justification).Replace("\n", "<br/>");

                // 2. Load Documents
                LoadDocuments(writeOffId);

                // 3. Load Details Grid (Repeater)
                var details = db.AssetWriteOffDetails.Where(d => d.WriteOffId == writeOffId).ToList();
                rptAssets.DataSource = details;
                rptAssets.DataBind();

                // 4. Load History Grid 
                var history = db.AssetWriteOffApprovalLogs
                                .Where(h => h.WriteOffId == writeOffId)
                                .OrderBy(h => h.ActionDate)
                                .ToList();
                gvHistory.DataSource = history;
                gvHistory.DataBind();

                // 5. SECURITY CHECK: Button Visibility Logic
                bool isPending = master.Status == "Pending" || master.Status == "Submitted";
                bool isAuthorizedApprover = false;

                if (isPending)
                {
                    isAuthorizedApprover = Class.AWOHelper.CheckIfUserCanApprove(master, CurrentUserRole);
                }

                if (!isPending || !isAuthorizedApprover)
                {
                    btnApprove.Visible = false;
                    btnReject.Visible = false;
                    btnSendBack.Visible = false;
                }
            }
        }

        protected void rptAssets_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var detail = (AssetWriteOffDetail)e.Item.DataItem;
                _totalOrig += detail.OriginalPrice;
                _totalAcc += detail.AccDepreciation;
                _totalNet += detail.NetBookValue;
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                Label lblOrig = (Label)e.Item.FindControl("lblTotalOrig");
                Label lblAcc = (Label)e.Item.FindControl("lblTotalAcc");
                Label lblNet = (Label)e.Item.FindControl("lblTotalNet");

                if (lblOrig != null) lblOrig.Text = _totalOrig.ToString("N2");
                if (lblAcc != null) lblAcc.Text = _totalAcc.ToString("N2");
                if (lblNet != null) lblNet.Text = _totalNet.ToString("N2");
            }
        }

        private void LoadDocuments(Guid writeOffId)
        {
            using (var db = new AppDbContext())
            {
                var documents = db.AssetWriteOffDocuments
                                  .Where(d => d.WriteOffId == writeOffId)
                                  .OrderByDescending(d => d.UploadedDate)
                                  .ToList();

                if (documents.Any())
                {
                    pnlUploadedDocument.Visible = true;
                    lblNoDocument.Visible = false;
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

        private void ProcessAction(string action)
        {
            if (!Guid.TryParse(Request.QueryString["id"], out Guid writeOffId)) return;

            string remark = hfApproverRemark.Value.Trim();

            using (var db = new AppDbContext())
            {
                var master = db.AssetWriteOffs.Find(writeOffId);
                if (master == null || (master.Status != "Pending" && master.Status != "Submitted")) return;

                if (!Class.AWOHelper.CheckIfUserCanApprove(master, CurrentUserRole))
                {
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "You are not authorized to process this request.");
                    return;
                }

                var approvalLog = new AssetWriteOffApprovalLog
                {
                    Id = Guid.NewGuid(),
                    WriteOffId = master.Id,
                    StepNumber = master.CurrentApprovalLevel,
                    RoleName = CurrentUserRole,
                    UserId = Auth.User().Id,
                    ActionType = action,
                    ActionDate = DateTime.Now,
                    Remarks = string.IsNullOrEmpty(remark) ? "No remarks provided" : remark,
                    CreatedBy = Auth.User().Id,
                    CreatedDate = DateTime.Now
                };

                if (action == "Approve")
                {
                    var nextLevelRule = db.AssetWriteOffApprovalLimits
                        .Where(a =>
                            a.Order > master.CurrentApprovalLevel &&
                            master.NetBookValue >= a.AmountMin &&
                            (a.AmountMax == null || master.NetBookValue <= a.AmountMax)
                        )
                        .OrderBy(a => a.Order)
                        .FirstOrDefault();

                    if (nextLevelRule != null)
                    {
                        approvalLog.Status = $"Approved-{nextLevelRule.Order}";
                        master.CurrentApprovalLevel = nextLevelRule.Order ?? 0;
                        master.Status = "Pending";
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, $"Approved. Routed to Level {nextLevelRule.Order} ({nextLevelRule.AWOApproverCode}).");
                    }
                    else
                    {
                        approvalLog.Status = "Approved";
                        master.Status = "Approved";
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Asset Write-Off Fully Approved.");
                    }
                }
                else if (action == "Reject")
                {
                    approvalLog.Status = "Rejected";
                    master.Status = "Rejected";
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Request has been completely rejected.");
                }
                else if (action == "Send Back")
                {
                    approvalLog.Status = "Sent Back";
                    master.Status = "SentBack";
                    master.CurrentApprovalLevel = 0;
                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Request sent back to creator for clarification.");
                }

                master.UpdatedBy = Auth.User().Id;
                master.UpdatedDate = DateTime.Now;

                db.AssetWriteOffApprovalLogs.Add(approvalLog);
                db.SaveChanges();

                Response.Redirect("~/AssetWriteOff/Approver/Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            ProcessAction("Approve");
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            ProcessAction("Reject");
        }

        protected void btnSendBack_Click(object sender, EventArgs e)
        {
            ProcessAction("Send Back");
        }
    }
}