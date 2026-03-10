using CustomGuid.AspNet.Identity;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.ModelAWO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Class
{
    public class AWOHelper
    {
        // For Edit.aspx (Checks DB directly)
        public static bool CheckIfUserCanApprove(Models.ModelAWO.AssetWriteOff request, string currentUserRole)
        {
            if (request == null || string.IsNullOrEmpty(currentUserRole)) return false;

            using (var db = new AppDbContext())
            {
                var applicableRule = db.AssetWriteOffApprovalLimits.FirstOrDefault(rule =>
                    rule.AWOApproverCode == currentUserRole &&
                    rule.Order == request.CurrentApprovalLevel &&
                    request.NetBookValue >= rule.AmountMin &&
                    (rule.AmountMax == null || request.NetBookValue <= rule.AmountMax)
                );

                return applicableRule != null;
            }
        }

        // For Default.aspx Dashboards (Uses in-memory list to prevent lag)
        public static bool CheckIfUserCanApprove(Models.ModelAWO.AssetWriteOff request, List<AssetWriteOffApprovalLimit> cachedUserLimits)
        {
            if (request == null || cachedUserLimits == null || !cachedUserLimits.Any()) return false;

            var applicableRule = cachedUserLimits.FirstOrDefault(rule =>
                rule.Order == request.CurrentApprovalLevel &&
                request.NetBookValue >= rule.AmountMin &&
                (rule.AmountMax == null || request.NetBookValue <= rule.AmountMax)
            );

            return applicableRule != null;
        }

        public static bool IsAOWPendingUserAction(Guid AOWId, Guid? userId = null)
        {
            using (var db = new AppDbContext())
            {
                var user = userId.HasValue ? db.Users.Find(userId.Value) : Auth.User();
                if (user == null) return false;

                var AWO = db.AssetWriteOffs.Find(AOWId);
                if (AWO == null) return false;

                var highestNBV = db.AssetWriteOffDetails.Where(x => x.WriteOffId == AOWId).Select(x => x.NetBookValue).DefaultIfEmpty(0).Max();

                var approvalLimits = db.AssetWriteOffApprovalLimits.ExcludeSoftDeleted()
                    .Where(al => al.AmountMin <= highestNBV && (highestNBV <= al.AmountMax || al.AmountMax == null))
                    .OrderBy(al => al.Order)
                    .ToList();

                if (!approvalLimits.Any()) return false;

                var approvals = db.AssetWriteOffApprovalLogs
                    .Where(a => a.WriteOffId == AOWId)
                    .OrderBy(a => a.CreatedDate)
                    .ToList();

                if (!approvals.Any()) return false;

                // Step 1: Find the last "Submitted" to reset the cycle
                // Everything before the last submission is history and irrelevant for the current status check
                var lastSubmittedIndex = approvals.FindLastIndex(a => a.Status.Equals("Submitted", StringComparison.OrdinalIgnoreCase));
                if (lastSubmittedIndex == -1)
                    return false;

                // Step 2: Get ALL actions that happened AFTER the last submission
                var actionsAfterSubmit = approvals
                    .Skip(lastSubmittedIndex + 1)
                    .ToList();

                // [CRITICAL CHECK] If the LATEST action is "Sent Back", stop the approval flow.
                // The ball is now in the Applicant's court to edit and resubmit.
                // No approver should see "Approve/Reject" buttons while it is Sent Back.
                if (actionsAfterSubmit.Any())
                {
                    var latestAction = actionsAfterSubmit.Last();
                    if (latestAction.ActionType.Equals("Sent Back", StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }

                // [Existing Check] If any action is "Rejected", the approval flow stops permanently.
                if (actionsAfterSubmit.Any(a => a.ActionType.Equals("Rejected", StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }

                // Step 3: Filter for the "Approved" codes to determine who has already acted in this cycle
                var approvedCodes = actionsAfterSubmit
                    .Where(a => a.ActionType.Equals("Approve", StringComparison.OrdinalIgnoreCase))
                    .Select(a => a.RoleName)
                    .ToList();

                // Step 4: Find the next approver in the sequence
                foreach (var limit in approvalLimits)
                {
                    // If this role has NOT approved yet in this cycle
                    if (!approvedCodes.Contains(limit.AWOApproverCode, StringComparer.OrdinalIgnoreCase))
                    {
                        // Then it is this role's turn.
                        // Return TRUE if the current user holds this role.
                        return string.Equals(user.CCMSRoleCode, limit.AWOApproverCode, StringComparison.OrdinalIgnoreCase);
                    }
                }

                return false;
            }
        }
    }
}