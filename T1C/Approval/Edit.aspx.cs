using FGV.Prodata.App;
using FGV.Prodata.Web.UI;
using Org.BouncyCastle.Asn1.Ocsp;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.T1C.Approval
{
    public partial class Edit : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["Id"] != null)
                {
                    string id = Request.QueryString["Id"].ToString();
                    Guid guid;
                    if (Guid.TryParse(id, out guid))
                    {
                        hdnFormId.Value = id;
                        BindData(id);
                    }
                }
                else
                {
                    Response.Redirect("~/T1C/Approval");
                }
            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            string formId = hdnFormId.Value;
            Guid parsedFormId = Guid.Parse(formId);

            try
            {
                using (var db = new AppDbContext())
                {
                    var formClass = new Class.Form();
                    var form = db.Forms.Find(parsedFormId);
                    var lastApproval = db.Approvals
                        .Where(a => a.ObjectId == form.Id && a.ObjectType == "Form")
                        .OrderByDescending(a => a.CreatedDate)
                        .FirstOrDefault();
                    int lastApprovalOrder = lastApproval != null && lastApproval.Order.HasValue ? lastApproval.Order.Value : 0;

                    db.Approvals.Add(new Models.Approval
                    {
                        ObjectId = form.Id,
                        ObjectType = "Form",
                        ActionById = Auth.User().Id,
                        ActionByType = "User",
                        ActionByCode = Auth.User().iPMSRoleCode,
                        ActionByName = Auth.User().Name,
                        Action = "Approved",
                        Order = lastApprovalOrder + 1,
                    });
                    db.SaveChanges();

                    if (!formClass.IsFormHasNextApprover(form.Id))
                    {
                        form.Status = "Approved";
                        db.Entry(form).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
                Response.Redirect(Request.Url.GetCurrentUrl(true));
            }

            SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Approved.");
            Response.Redirect("~/T1C/Approval");
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            string formId = hdnFormId.Value;
            Guid parsedFormId = Guid.Parse(formId);

            try
            {
                using (var db = new AppDbContext())
                {
                    var form = db.Forms.Find(parsedFormId);
                    var lastApproval = db.Approvals
                        .Where(a => a.ObjectId == form.Id && a.ObjectType == "Form")
                        .OrderByDescending(a => a.CreatedDate)
                        .FirstOrDefault();
                    int lastApprovalOrder = lastApproval != null && lastApproval.Order.HasValue ? lastApproval.Order.Value : 0;

                    db.Approvals.Add(new Models.Approval
                    {
                        ObjectId = form.Id,
                        ObjectType = "Form",
                        ActionById = Auth.User().Id,
                        ActionByType = "User",
                        ActionByCode = Auth.User().iPMSRoleCode,
                        ActionByName = Auth.User().Name,
                        Action = "Rejected",
                        Order = lastApprovalOrder + 1,
                    });

                    form.Status = "Rejected";
                    db.Entry(form).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
                Response.Redirect(Request.Url.GetCurrentUrl(true));
            }

            SweetAlert.SetAlert(SweetAlert.SweetAlertType.Info, "Rejected.");
            Response.Redirect("~/T1C/Approval");
        }

        private void BindData(string id = null)
        {
            if (!string.IsNullOrEmpty(id))
            {
                using (var db = new AppDbContext())
                {
                    var form = db.Forms.Find(Guid.Parse(id));
                    if (form != null)
                    {
                        lblBA.Text = form.BizAreaName;
                        lblRefNo.Text = form.Ref;
                        lblDate.Text = form.Date.HasValue ? form.Date.Value.ToString("dd/MM/yyyy") : "-";
                        lblDetails.Text = form.Details;
                        lblJustificationOfNeed.Text = form.JustificationOfNeed;
                        lblAmount.Text = form.Amount.HasValue ? "RM" + form.Amount.Value.ToString("#,##0.00") : "-";

                        #region Allocation
                        var budgets = db.FormBudgets
                            .Where(fb => fb.FormId == form.Id)
                            .Select(fb => new
                            {
                                fb.Budget.Ref,
                                fb.Budget.Details,
                                fb.Amount
                            })
                            .ToList();

                        if (budgets.Any())
                        {
                            string html = "<ol style='padding-left: 15px;'>";  // 👈 added style here

                            foreach (var budget in budgets)
                            {
                                string amount = budget.Amount.HasValue ? "RM" + budget.Amount.Value.ToString("#,##0.00") : "-";
                                string line = $"{budget.Ref} - {budget.Details} [{amount}]";
                                html += $"<li>{Server.HtmlEncode(line)}</li>";
                            }

                            html += "</ol>";

                            lblAllocation.Text = html;
                        }
                        else
                        {
                            lblAllocation.Text = "<em>No Budgets Assigned</em>";
                        }
                        #endregion

                        #region Vendor
                        var vendorNames = db.FormVendors
                            .Where(fv => fv.FormId == form.Id)
                            .Select(fv => fv.Vendor.Name)
                            .ToList();

                        if (vendorNames.Any())
                        {
                            string html = "<ol style='padding-left: 15px;'>";
                            foreach (var name in vendorNames)
                            {
                                html += $"<li>{Server.HtmlEncode(name)}</li>";
                            }
                            html += "</ol>";
                            lblVendor.Text = html;
                        }
                        else
                        {
                            lblVendor.Text = "<em>No Vendors Assigned</em>";
                        }
                        #endregion

                        lblJustificationDirectAward.Visible = false;
                        string procurementType = string.Empty;
                        if (form.ProcurementType.Equals("quotation", StringComparison.OrdinalIgnoreCase))
                        {
                            procurementType = "Kaedah Sebutharga";
                        }
                        else if (form.ProcurementType.Equals("selective_tender", StringComparison.OrdinalIgnoreCase))
                        {
                            procurementType = "Tender Selektif";
                        }
                        else if (form.ProcurementType.Equals("open_tender", StringComparison.OrdinalIgnoreCase))
                        {
                            procurementType = "Tender Terbuka";
                        }
                        else if (form.ProcurementType.Equals("direct_award", StringComparison.OrdinalIgnoreCase))
                        {
                            procurementType = "Rundingan Terus";
                            lblJustificationDirectAward.Visible = true;
                        }

                        lblProcurementType.Text = procurementType;
                        lblJustificationDirectAward.Text = form.Justification;

                        lblCurrentYearActualYTD.Text = form.CurrentYearActualYTD.HasValue ? form.CurrentYearActualYTD.Value.ToString("#,##0.00") : "-";
                        lblCurrentYearBudget.Text = form.CurrentYearBudget.HasValue ? form.CurrentYearBudget.Value.ToString("#,##0.00") : "-";
                        lblPreviousYearActualYTD.Text = form.PreviousYearActualYTD.HasValue ? form.PreviousYearActualYTD.Value.ToString("#,##0.00") : "-";
                        lblPreviousYearActual.Text = form.PreviousYearActual.HasValue ? form.PreviousYearActual.Value.ToString("#,##0.00") : "-";
                        lblPreviousYearBudget.Text = form.PreviousYearBudget.HasValue ? form.PreviousYearBudget.Value.ToString("#,##0.00") : "-";
                        lblA.Text = form.A.HasValue ? form.A.Value.ToString("#,##0.00") : "-";
                        lblB.Text = !string.IsNullOrEmpty(form.B) ? form.B : "-";
                        lblC.Text = form.C.HasValue ? form.C.Value.ToString("#,##0.00") : "-";
                        lblD.Text = form.D.HasValue ? form.D.Value.ToString("#,##0.00") : "-";
                    }
                }
            }
        }
    }
}