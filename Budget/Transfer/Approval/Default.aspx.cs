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
    public partial class Default : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindTransfers();
            }
        }
        private void BindTransfers()
        {
            string ba = Auth.User().iPMSBizAreaCode;
            string userRole = Auth.User().iPMSRoleCode;

            // Handle null gracefully: null ba = access all, null role = no approval limit filter
            List<string> accessibleBizAreas = !string.IsNullOrEmpty(ba)
                ? new Class.IPMSBizArea().GetBizAreaCodes(ba)
                : new List<string>(); // Empty list means access all later
            //List<string> accessibleBizAreas = Auth.IPMSBizAreaCodes();

            using (var db = new AppDbContext())
            {
                // Get approval limits only if userRole is not null
                var limits = !string.IsNullOrEmpty(userRole)
                    ? db.TransferApprovalLimits
                        .Where(m => m.TransApproverCode == userRole && m.DeletedDate == null)
                        .ToList()
                    : new List<TransferApprovalLimits>();

                // Query base transfers
                var query = db.TransfersTransaction
                              .Where(x => x.DeletedDate == null);

                if (!string.IsNullOrEmpty(ba))
                {
                    // If BA is defined, filter by accessible BAs (or user's BA if none found)
                    if (accessibleBizAreas.Count > 0)
                        query = query.Where(x => accessibleBizAreas.Contains(x.BA));
                    else
                        query = query.Where(x => x.BA == ba);
                }
                // else: ba == null → no filtering, access all

                var transfers = query
                    .OrderByDescending(x => x.Date)
                    .ToList()
                    .Select(x =>
                    {
                        int currentLevelApproval = db.TransferApprovalLog
                            .Where(w => w.BudgetTransferId == x.Id)
                            .OrderByDescending(w => w.CreatedDate)
                            .Select(w => w.StepNumber)
                            .FirstOrDefault();

                        var matchingLimit = limits.FirstOrDefault(l =>
                            l.AmountMin <= x.EstimatedCost &&
                            (l.AmountMax == null || l.AmountMax >= x.EstimatedCost));

                        int userLevelApproval = matchingLimit?.Order ?? 0;

                        bool canEdit = (matchingLimit != null && userLevelApproval == currentLevelApproval + 1);//|| Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "admin-user-edit");

                        return new
                        {
                            x.BA,
                            x.Id,
                            x.RefNo,
                            x.Project,
                            x.Date,
                            x.EstimatedCost,
                            Status = x.status == 0 ? "Resubmit" :
                                        //x.status == 1 ? "Submitted" :
                                        x.status == 2 ? "Under Review" :
                                        x.status == 3 ? "Completed" :
                                        "Submitted",
                            CanEdit = canEdit
                        };
                    })
                    .ToList();

                gvTransfers.DataSource = transfers;
                gvTransfers.DataBind();
            }
        }

        //private void BindTransfers()
        //{
        //    string ba = Auth.User().iPMSBizAreaCode;
        //    string userRole = Auth.User().iPMSRoleCode;

        //    using (var db = new AppDbContext())
        //    {
        //        // Get all approval limits for this user
        //        var limits = db.TransferApprovalLimits
        //                       .Where(m => m.TransApproverCode == userRole && m.DeletedDate == null)
        //                       .ToList();

        //        var transfers = db.TransfersTransaction
        //                          .Where(x => x.DeletedDate == null && (ba == null || x.BA == ba))
        //                          .OrderByDescending(x => x.Date)
        //                          .ToList()
        //                          .Select(x =>
        //                          {
        //                              // Get the current approval step for this transfer
        //                              int currentLevelApproval = db.TransferApprovalLog
        //                                  .Where(w => w.BudgetTransferId == x.Id)
        //                                  .OrderByDescending(w => w.CreatedDate)
        //                                  .Select(w => w.StepNumber)
        //                                  .FirstOrDefault();

        //                              // Find the user's approval limit matching this cost
        //                              var matchingLimit = limits.FirstOrDefault(l =>
        //                                  l.AmountMin <= x.EstimatedCost &&
        //                                  (l.AmountMax == null || l.AmountMax >= x.EstimatedCost));

        //                              int userLevelApproval = matchingLimit?.Order ?? 0;

        //                              bool costWithinLimit = matchingLimit != null;

        //                              bool canEdit = ba == null || (
        //                                  costWithinLimit &&
        //                                  userLevelApproval == currentLevelApproval + 1
        //                              );

        //                              return new
        //                              {
        //                                  x.Id,
        //                                  x.RefNo,
        //                                  x.Project,
        //                                  x.Date,
        //                                  x.BudgetType,
        //                                  x.EstimatedCost,
        //                                  x.Justification,
        //                                  Status = x.status == 0 ? "Decline" :
        //                                           x.status == 2 ? "Processing" :
        //                                           x.status == 3 ? "Completed" :
        //                                           "Pending",
        //                                  CanEdit = canEdit
        //                              };
        //                          })
        //                          .ToList();

        //        gvTransfers.DataSource = transfers;
        //        gvTransfers.DataBind();
        //    }
        //}

        //private void BindTransfers()
        //{
        //    string ba = Auth.User().iPMSBizAreaCode;
        //    string userRole = Auth.User().iPMSRoleCode;

        //    using (var db = new AppDbContext())
        //    {
        //        // Get all approval limits for this user
        //        var limits = db.TransferApprovalLimits
        //                       .Where(m => m.TransApproverCode == userRole && m.DeletedDate == null)
        //                       .ToList();

        //        // Get the user's approval step (Order)
        //        int userLevelApproval = limits
        //                                .Select(l => l.Order.GetValueOrDefault())
        //                                .FirstOrDefault();

        //        var transfers = db.TransfersTransaction
        //                          .Where(x => x.DeletedDate == null && (ba == null || x.BA == ba))
        //                          .OrderByDescending(x => x.Date)
        //                          .ToList()
        //                          .Select(x =>
        //                          {
        //                              // Get the latest approval step for this transfer
        //                              int currentLevelApproval = db.TransferApprovalLog
        //                                  .Where(w => w.BudgetTransferId == x.Id)
        //                                  .OrderByDescending(w => w.CreatedDate)
        //                                  .Select(w => w.StepNumber)
        //                                  .FirstOrDefault(); // default to 0 if none

        //                              // Check if the user's cost limit allows them to approve
        //                              bool costWithinLimit = limits.Any(l =>
        //                                  l.AmountMin <= x.EstimatedCost &&
        //                                  (l.AmountMax == null || l.AmountMax >= x.EstimatedCost));

        //                              // User can edit if:
        //                              // - user is the next approver in line, AND
        //                              // - cost is within their limit, OR
        //                              // - ba == null (admin override)
        //                              bool canEdit = ba == null || (
        //                                  costWithinLimit &&
        //                                  userLevelApproval == currentLevelApproval + 1
        //                              );

        //                              return new
        //                              {
        //                                  x.Id,
        //                                  x.RefNo,
        //                                  x.Project,
        //                                  x.Date,
        //                                  x.BudgetType,
        //                                  x.EstimatedCost,
        //                                  x.Justification,
        //                                  Status = x.status == 0 ? "Decline" :
        //                                           x.status == 2 ? "Processing" :
        //                                           x.status == 3 ? "Completed" :
        //                                           "Pending",
        //                                  CanEdit = canEdit
        //                              };
        //                          })
        //                          .ToList();

        //        gvTransfers.DataSource = transfers;
        //        gvTransfers.DataBind();
        //    }
        //}


        //private void BindTransfers()
        //{
        //    string ba = Auth.User().iPMSBizAreaCode;
        //    string userRole = Auth.User().iPMSRoleCode;

        //    using (var db = new AppDbContext())
        //    {
        //        var limits = db.TransferApprovalLimits
        //                       .Where(m => m.TransApproverCode == userRole)
        //                       .ToList(); 

        //        var transfers = db.TransfersTransaction
        //                          .Where(x => x.DeletedDate == null && (ba == null || x.BA == ba))
        //                          .OrderByDescending(x => x.Date)
        //                          .ToList()
        //                          .Select(x => new
        //                          {
        //                              x.Id,
        //                              x.RefNo,
        //                              x.Project,
        //                              x.Date,
        //                              x.BudgetType,
        //                              x.EstimatedCost,
        //                              x.Justification,
        //                              Status =
        //                                  x.status == 0 ? "Decline" :
        //                                  x.status == 2 ? "Processing" :
        //                                  x.status == 3 ? "Completed" :
        //                                  "Pending",
        //                              CanEdit = ba == null || limits.Any(l =>
        //                                  l.AmountMin <= x.EstimatedCost &&
        //                                  (l.AmountMax == null || l.AmountMax >= x.EstimatedCost))
        //                          })
        //                          .ToList();

        //        gvTransfers.DataSource = transfers;
        //        gvTransfers.DataBind();
        //    }
        //}


        //private bool CanUserApprove(decimal cost, string userRole )
        //{
        //    using (var db = new AppDbContext())
        //    {
        //        // Find matching matrix row
        //        var match = db.ApprovalLimits.FirstOrDefault(m =>
        //            m.ApproverCode == userRole &&                               // Role match
        //            m.AmountMin <= cost &&
        //            (m.AmountMax == null || m.AmountMax >= cost)                // Handle upper limit being null
        //        );

        //        return match != null;
        //    }
        //}

        protected void btnDeleteConfirmed_Click(object sender, EventArgs e)
        {
            try
            {
                int id = int.Parse(hdnDeleteId.Value);

                using (var db = new AppDbContext())
                {
                    var record = db.TransfersTransaction.Find(id);
                    if (record != null)
                    {
                        record.DeletedBy = Auth.Id();
                        record.DeletedDate = DateTime.Now;

                        db.SaveChanges();
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Info, "Record deleted successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Error deleting record: " + ex.Message);
            }

            BindTransfers();
        }
    }
}
