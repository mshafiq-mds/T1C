using Prodata.WebForm.Models;
using Prodata.WebForm.Models.Auth;
using Prodata.WebForm.Models.ModelAWO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Prodata.WebForm.Class
{
    public class AWOEmails
    {
        private static string GetSystemUrl()
        {
            string baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string appPath = HttpContext.Current.Request.ApplicationPath.TrimEnd('/');
            return baseUrl + appPath;
        }

        // ========================================================
        // SAVE NEXT APPROVER INTO DATABASE
        // ========================================================
        public static void SaveNextApprover(Guid writeOffId, List<string> nextApproverCodes, AppDbContext db)
        {
            string nextApproverString = string.Join(",", nextApproverCodes);

            // Retrieve the specific AWO form
            var currentForm = db.AssetWriteOffs.FirstOrDefault(f => f.Id == writeOffId);

            if (currentForm != null)
            {
                // Note: Ensure your AssetWriteOff C# Model has the 'NextApprover' string property!
                currentForm.NextApprover = nextApproverString;

                // Save changes to the database
                db.SaveChanges();
            }
        }

        // ========================================================
        // 1. TRIGGER: Send Email to the Next Approver (New or Ongoing)
        // ========================================================
        public static void SendToNextApprover(Guid writeOffId)
        {
            string baseUrl = GetSystemUrl();
            Task.Run(() => ProcessEmailToNextApprover(writeOffId, baseUrl));
        }

        private static void ProcessEmailToNextApprover(Guid writeOffId, string baseUrl)
        {
            using (var db = new AppDbContext())
            {
                var awo = db.AssetWriteOffs.Find(writeOffId);
                if (awo == null) return;

                // 1. Determine the exact RoleCode that needs to approve this right now
                var activeRule = db.AssetWriteOffApprovalLimits
                    .FirstOrDefault(r =>
                        r.Order == awo.CurrentApprovalLevel &&
                        awo.NetBookValue >= r.AmountMin &&
                        (r.AmountMax == null || awo.NetBookValue <= r.AmountMax));

                if (activeRule == null) return;

                string targetRoleCode = activeRule.AWOApproverCode;

                // NEW: Save the Next Approver directly to the Master Table
                SaveNextApprover(writeOffId, new List<string> { targetRoleCode }, db);

                // 2. Fetch Users who match the Role AND the Business Area / Zone / Wilayah
                List<User> targetUsers = GetEligibleUsers(db, targetRoleCode, awo.BACode);
                if (!targetUsers.Any()) return;

                // 3. Build & Send Email
                string actionName = "Pending AWO Approval";
                string targetUrl = $"{baseUrl}/AssetWriteOff/Approver/Edit.aspx?id={writeOffId}";

                foreach (var user in targetUsers)
                {
                    string body = AWOEmailTemplateBuilder.BuildAWOEmailBody(awo, actionName, targetUrl, user);
                    Emails.SendFunctionEmail(user.Email, actionName, body);
                }
            }
        }

        // ========================================================
        // 2. TRIGGER: Send Email Back to the Creator (Sent Back / Clarification)
        // ========================================================
        public static void SendBackToCreator(Guid writeOffId)
        {
            string baseUrl = GetSystemUrl();
            Task.Run(() => ProcessEmailToCreator(writeOffId, baseUrl, "Sent Back for Clarification"));
        }

        // ========================================================
        // 3. TRIGGER: Send Email to Creator upon Final Approval/Rejection
        // ========================================================
        public static void SendFinalResultToCreator(Guid writeOffId, bool isApproved)
        {
            string baseUrl = GetSystemUrl();
            string actionName = isApproved ? "AWO Fully Approved" : "AWO Rejected";
            Task.Run(() => ProcessEmailToCreator(writeOffId, baseUrl, actionName));
        }

        private static void ProcessEmailToCreator(Guid writeOffId, string baseUrl, string actionName)
        {
            using (var db = new AppDbContext())
            {
                var awo = db.AssetWriteOffs.Find(writeOffId);
                if (awo == null) return;

                // When returning to creator or finalizing, clear the NextApprover field
                SaveNextApprover(writeOffId, new List<string> { "-" }, db);

                // Find the Creator User
                var creator = db.Users.FirstOrDefault(u => u.Id == awo.CreatedBy);
                if (creator == null || string.IsNullOrEmpty(creator.Email)) return;

                // For creator, redirect them to the View page (or Edit if sent back)
                string targetUrl = awo.Status == "Clarification" || awo.Status == "SentBack"
                    ? $"{baseUrl}/AssetWriteOff/Edit.aspx?id={writeOffId}"
                    : $"{baseUrl}/AssetWriteOff/View.aspx?id={writeOffId}";

                string body = AWOEmailTemplateBuilder.BuildAWOEmailBody(awo, actionName, targetUrl, creator);
                Emails.SendFunctionEmail(creator.Email, actionName, body);
            }
        }


        // ========================================================
        // Helper: Logic to Find Users based on Role & BA Hierarchy
        // ========================================================
        private static List<User> GetEligibleUsers(AppDbContext db, string roleCode, string formBizAreaCode)
        {
            // Note: Reusing the same logic from your other modules
            string zoneCode = Class.IPMSBizArea.GetZoneCodeByBizAreaCode(formBizAreaCode);
            string wilayahCode = Class.IPMSBizArea.GetWilayahCodeByBizAreaCode(formBizAreaCode);

            var allRoleUsers = db.Users.Where(x => x.CCMSRoleCode == roleCode).ToList();

            // 1. Try Zone
            if (!string.IsNullOrEmpty(zoneCode))
            {
                var cleanCode = zoneCode.Trim();
                var usersByZone = allRoleUsers.Where(x => x.CCMSBizAreaCode == cleanCode || x.CCMSBizAreaCode == "").ToList();
                if (usersByZone.Any()) return usersByZone;
            }

            // 2. Try Wilayah
            if (!string.IsNullOrEmpty(wilayahCode))
            {
                var cleanCode = wilayahCode.Trim();
                var usersByWilayah = allRoleUsers.Where(x => x.CCMSBizAreaCode == cleanCode || x.CCMSBizAreaCode == "").ToList();
                if (usersByWilayah.Any()) return usersByWilayah;
            }

            // 3. Exact Match or Global (Empty BA)
            return allRoleUsers
                .Where(x => x.CCMSBizAreaCode == formBizAreaCode || string.IsNullOrEmpty(x.CCMSBizAreaCode))
                .ToList();
        }
    }
}