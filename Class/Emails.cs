using NPOI.SS.Formula.Functions;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using static System.Net.WebRequestMethods;

namespace Prodata.WebForm.Class
{
    public class Emails
    {
        #region Public call email Function EmailsTransferBudgetForResubmit
        // Public methods to trigger email notifications for different scenarios

        public static string urlsystem()
        {
            string baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string appPath = HttpContext.Current.Request.ApplicationPath.TrimEnd('/');
            return baseUrl + appPath;
        }
        // ===========================================
        // ✅ For new requests  
        // ===========================================

        // ✅ For request T2
        public static void EmailsT2ForNewRequest(Guid id, Models.FormsProcurement FoP, string roleCode)
        {
            string baseUrl = urlsystem();
            Task.Run(() => EmailsT2RequestModified(id, FoP, roleCode, baseUrl));
        }

        // ✅ For request T1C
        public static void EmailsT1CForNewRequest(Guid id, Models.Form Fo, string roleCode)
        {
            string baseUrl = urlsystem();
            Task.Run(() => EmailsT1CRequestModified(id, Fo, roleCode, baseUrl));
        }

        // ✅ For request Additional
        public static void EmailsAdditionalBudgetForNewRequest(Guid id, AdditionalBudgetRequests ABR, string roleCode)
        {
            string baseUrl = urlsystem();
            Task.Run(() => EmailsAdditionalBudgetForNewRequestModified(id, ABR, roleCode, baseUrl));
        }


        // ✅ For request Transfer
        public static void EmailsReqTransferBudgetForNewRequest(Guid id, TransfersTransaction TT, string roleCode)
        {
            string baseUrl = urlsystem();
            Task.Run(() => EmailsTransferBudgetForNewRequestModified(id, TT, roleCode, baseUrl));
        }


        // ===========================================
        // ✅ For submissions next approver              //resubmit or complete (roleCode = "")
        // =========================================== 

        // ✅ For request T2
        public static void EmailsT2ForApprover(Guid id, Models.FormsProcurement FoP)
        {
            string baseUrl = urlsystem();
            Task.Run(() => EmailsReqT2ForApproverModified(id, FoP, baseUrl));
        }

        // ✅ For request T1C
        public static void EmailsT1CForApprover(Guid id, Models.Form Fo, string roleCode = "")
        {
            string baseUrl = urlsystem();
            Task.Run(() => EmailsReqT1CForApproverModified(id, Fo, roleCode, baseUrl));
        }

        // ✅ For request Additional
        public static void EmailsAdditionalBudgetForApprover(Guid id, AdditionalBudgetRequests ABR, string roleCode = "")
        {
            string baseUrl = urlsystem();
            Task.Run(() => EmailsReqAdditionalBudgetForApproverModified(id, ABR, roleCode, baseUrl));
        }


        // ✅ For request Transfer 
        public static void EmailsReqTransferBudgetForApprover(Guid id, TransfersTransaction TT, string roleCode = "")
        {
            string baseUrl = urlsystem();
            Task.Run(() => EmailsReqTransferBudgetModifiedForApprover(id, TT, roleCode, baseUrl));
        }


        // ===========================================
        // ✅ For resubmissions to approver
        // ===========================================
         
        // ✅ For request T1C
        public static void EmailsT1CForResubmit(Guid id, Models.Form Fo, string roleCode)
        {
            string baseUrl = urlsystem();
            Task.Run(() => EmailsReqT1CForResubmitModified(id, Fo, roleCode, baseUrl));
        }

        // ✅ For request Additional
        public static void EmailsAdditionalBudgetForResubmit(Guid id, AdditionalBudgetRequests ABR, string roleCode)
        {
            string baseUrl = urlsystem();
            Task.Run(() => EmailsReqAdditionalBudgetForResubmitModified(id, ABR, roleCode, baseUrl));
        }


        // ✅ For request Transfer
        public static void EmailsTransferBudgetForResubmit(Guid id, TransfersTransaction ABR, string roleCode)
        {
            string baseUrl = urlsystem();
            Task.Run(() => EmailsReqTransferBudgetForResubmitModified(id, ABR, roleCode, baseUrl));
        }

        #endregion







        #region T1C Logic
        public static void EmailsT1CRequestModified(Guid id, Models.Form Fo, string roleCode, string baseUrl)
        {
            decimal amount = Fo.Amount ?? 0;

            List<string> nextApproverCodes = GetNextApproverCodesForTransT1CNewRequest(amount, roleCode, out var db);
            if (nextApproverCodes == null || !nextApproverCodes.Any()) return;

            // First try direct match on BizArea
            List<User> userRole = db.Users
                .Where(x => nextApproverCodes.Contains(x.iPMSRoleCode) && x.iPMSBizAreaCode == Fo.BizAreaCode)
                .ToList();

            // Fallback: Try zone-level using helper
            if (!userRole.Any())
            {
                foreach (var code in nextApproverCodes)
                {
                    var fallbackUsers = GetUsersDetails(db, code, Fo.BizAreaCode);
                    if (fallbackUsers.Any())
                    {
                        userRole.AddRange(fallbackUsers);
                    }
                }

                // Optional: remove duplicates by Email if multiple fallback hits
                userRole = userRole.GroupBy(u => u.Email).Select(g => g.First()).ToList();
            }

            if (!userRole.Any()) return; // Still no users, exit

            string actionName = "New T1C Request";


            foreach (var user in userRole)
            {
                string fullUrl = $"{baseUrl}/T1C/Approval/Default"; 
                string body = EmailTemplateBuilder.BuildT1CEmailBody(Fo, actionName, fullUrl, user);
                SendFunctionEmail(user.Email, actionName, body);
            }
        }
        private static List<string> GetNextApproverCodesForTransT1CNewRequest(decimal amount, string roleCode, out AppDbContext db)
        {
            db = new AppDbContext();

            return db.ApprovalLimits
                .Where(x => x.DeletedDate == null &&
                            x.AmountMin <= amount &&
                            (x.AmountMax == null || amount <= x.AmountMax) &&
                            x.Order == 1)
                .Select(x => x.ApproverCode)
                .Distinct()
                .ToList();

        }
        #endregion

        #region T2 Logic
        public static void EmailsT2RequestModified(Guid id, Models.FormsProcurement FoP, string roleCode, string baseUrl)
        {
            using (var db = new AppDbContext())
            {
                    // First try direct match on BizArea
                List<User> userRole = db.Users
                    .Where(x => (x.iPMSRoleCode == "MM" || x.iPMSRoleCode == "AM" || x.iPMSRoleCode == "AMM")
                                && x.iPMSBizAreaCode == FoP.BizAreaCode)
                    .ToList();

                if (!userRole.Any()) return; // Still no users, exit

                string actionName = "New Budget Transaction";

                var budgettype = db.BudgetTypes.Where(x => x.Id == FoP.TypeId).Select(x => x.Name).FirstOrDefault();

                foreach (var user in userRole)
                {
                    string fullUrl = $"{baseUrl}/T1C/PoolBudget/Approval/Default";
                    string body = EmailTemplateBuilder.BuildT2EmailBody(FoP, actionName, fullUrl, budgettype, user);
                    SendFunctionEmail(user.Email, actionName, body);
                }
            }
        }

        #endregion

        #region Transfer budget Logic
        public static void EmailsReqTransferBudgetModifiedForApprover(Guid id, TransfersTransaction TT, string roleCode, string baseUrl)
        {
            string nextApproverCode = GetNextTransferApproverCode(TT.FromTransfer ?? 0, roleCode, out var db, id);
            if (string.IsNullOrEmpty(nextApproverCode)) return;

            List<User> userRole = GetUsersDetails(db, nextApproverCode, TT.BA); 
            string actionName = "";
            string fullUrl = "";

            foreach (var user in userRole)
            {

                if (TT.status == 3) // completed transfer budget
                {
                    fullUrl = $"{baseUrl}/Budget/Transfer/View?Id={id}";
                    actionName = "Completed Transfer Budget";
                }
                else if (roleCode == "") // resubmit approver to creator
                {
                    fullUrl = $"{baseUrl}/Budget/Transfer/Default";
                    actionName = "Transfer Budget Request Revision";
                }
                else // verify approver to next approver
                {
                    fullUrl = $"{baseUrl}/Budget/Transfer/Approval/Default";
                    actionName = "Transfer Budget Request";
                }

                string body = EmailTemplateBuilder.BuildTransferEmailBody(TT, actionName, fullUrl, user);
                SendFunctionEmail(user.Email, actionName, body);
            }
        }
        private static string GetNextTransferApproverCode(decimal amount, string roleCode, out AppDbContext db, Guid? id = null)
        {
            if (roleCode == "") // resubmit approver to creator
            {
                db = new AppDbContext();
                Guid? eligibleID = Guid.Empty;

                if (id.HasValue)
                {
                    eligibleID = db.TransfersTransaction
                        .Where(x => x.Id == id.Value)
                        .Select(x => x.CreatedBy)
                        .FirstOrDefault();
                }

                var user = db.Users
                    .Where(x => x.Id == eligibleID)
                    .FirstOrDefault();

                return user?.iPMSRoleCode;
            }
            else // verify approver to next approver
            {
                db = new AppDbContext();
                var eligible = db.TransferApprovalLimits
                    .Where(x => x.DeletedDate == null && x.AmountMin <= amount && (x.AmountMax == null || amount <= x.AmountMax))
                    .OrderBy(x => x.Order)
                    .ToList();

                var current = eligible.FirstOrDefault(x => x.TransApproverCode == roleCode);
                var next = eligible.FirstOrDefault(x => current != null && x.Order > current.Order);

                return next?.TransApproverCode;
            }

        }
        #endregion

        #region  Transfer budget For Requestor Logic
        public static void EmailsTransferBudgetForNewRequestModified(Guid id, TransfersTransaction TT, string roleCode, string baseUrl)
        {
            decimal amount = TT.FromTransfer ?? 0;

            List<string> nextApproverCodes = GetNextApproverCodesForTransferBudgetForNewRequest(amount, roleCode, out var db);
            if (nextApproverCodes == null || !nextApproverCodes.Any()) return;

            // First try direct match on BizArea
            List<User> userRole = db.Users
                .Where(x => nextApproverCodes.Contains(x.iPMSRoleCode) && x.iPMSBizAreaCode == TT.BA)
                .ToList();

            // Fallback: Try zone-level using helper
            if (!userRole.Any())
            {
                foreach (var code in nextApproverCodes)
                {
                    var fallbackUsers = GetUsersDetails(db, code, TT.BA);
                    if (fallbackUsers.Any())
                    {
                        userRole.AddRange(fallbackUsers);
                    }
                }

                // Optional: remove duplicates by Email if multiple fallback hits
                userRole = userRole.GroupBy(u => u.Email).Select(g => g.First()).ToList();
            }

            if (!userRole.Any()) return; // Still no users, exit
             
            string actionName = "Transfer Budget Request";


            foreach (var user in userRole)
            {
                string fullUrl = $"{baseUrl}/Budget/Transfer/Approval/Default";
                string body = EmailTemplateBuilder.BuildTransferEmailBody(TT, actionName, fullUrl, user);
                SendFunctionEmail(user.Email, actionName, body);
            }
        }



        private static List<string> GetNextApproverCodesForTransferBudgetForNewRequest(decimal amount, string roleCode, out AppDbContext db)
        {
            db = new AppDbContext();
             
                return db.TransferApprovalLimits
                    .Where(x => x.DeletedDate == null &&
                                x.AmountMin <= amount &&
                                (x.AmountMax == null || amount <= x.AmountMax) &&
                                x.Order == 1)
                    .Select(x => x.TransApproverCode)
                    .Distinct()
                    .ToList();
             
        }

        #endregion

        #region  Additional budget For Requestor Logic
        public static void EmailsAdditionalBudgetForNewRequestModified(Guid id, AdditionalBudgetRequests ABR, string roleCode, string baseUrl)
        {
            string type = ABR.CheckType == "FINANCE" ? "Finance" : "COGS";
            decimal amount = ABR.AdditionalBudget ?? 0;

            List<string> nextApproverCodes = GetNextApproverCodesForAdditionalBudgetForNewRequest(amount, roleCode, type, out var db);
            if (nextApproverCodes == null || !nextApproverCodes.Any()) return;

            // First try direct match on BizArea
            List<User> userRole = db.Users
                .Where(x => nextApproverCodes.Contains(x.iPMSRoleCode) && x.iPMSBizAreaCode == ABR.BA)
                .ToList();

            // Fallback: Try zone-level using helper
            if (!userRole.Any())
            {
                foreach (var code in nextApproverCodes)
                {
                    var fallbackUsers = GetUsersDetails(db, code, ABR.BA);
                    if (fallbackUsers.Any())
                    {
                        userRole.AddRange(fallbackUsers);
                    }
                }

                // Optional: remove duplicates by Email if multiple fallback hits
                userRole = userRole.GroupBy(u => u.Email).Select(g => g.First()).ToList();
            }

            if (!userRole.Any()) return; // Still no users, exit
             
            string actionName = "Additional Budget Request";


            foreach (var user in userRole)
            {
                string fullUrl = $"{baseUrl}/Budget/Additional/Approval/{type}/Default";
                string body = EmailTemplateBuilder.BuildAdditionalEmailBody(ABR, actionName, fullUrl, user);
                SendFunctionEmail(user.Email, actionName, body);
            }
        }



        private static List<string> GetNextApproverCodesForAdditionalBudgetForNewRequest(decimal amount, string roleCode, string type, out AppDbContext db)
        {
            db = new AppDbContext();
            if (type == "Finance")
            {
                return db.AdditionalLoaFinanceLimits
                    .Where(x => x.DeletedDate == null &&
                                x.AmountMin <= amount &&
                                (x.AmountMax == null || amount <= x.AmountMax) &&
                                x.Order == 1)
                    .Select(x => x.FinanceApproverCode)
                    .Distinct()
                    .ToList();
            }
            else
            {
                return db.AdditionalLoaCogsLimits
                    .Where(x => x.DeletedDate == null &&
                                x.AmountMin <= amount &&
                                (x.AmountMax == null || amount <= x.AmountMax) &&
                                x.Order == 1)
                    .Select(x => x.CogsApproverCode)
                    .Distinct()
                    .ToList();
            }
        }

        #endregion

        #region  Additional budget For Resubmit Logic
        public static void EmailsReqAdditionalBudgetForResubmitModified(Guid id, AdditionalBudgetRequests ABR, string roleCode, string baseUrl)
        {
            string type = ABR.CheckType == "FINANCE" ? "Finance" : "COGS";
            decimal amount = ABR.AdditionalBudget ?? 0;

            var db = new AppDbContext();
            List<User> userRole = GetUsersDetails(db, roleCode, ABR.BA); 
            string actionName = "Additional Budget Request";

            foreach (var user in userRole)
            {
                string fullUrl = $"{baseUrl}/Budget/Additional/Approval/{type}/Default";
                string body = EmailTemplateBuilder.BuildAdditionalEmailBody(ABR, actionName, fullUrl, user);
                SendFunctionEmail(user.Email, actionName, body);
            }
        }
        #endregion 

        #region  T1C For Resubmit Logic
        public static void EmailsReqT1CForResubmitModified(Guid id, Models.Form Fo, string roleCode, string baseUrl)
        {
            decimal amount = Fo.Amount ?? 0;

            var db = new AppDbContext();
            List<User> userRole = GetUsersDetails(db, roleCode, Fo.BizAreaCode);
            string actionName = "Transfer Budget Request";

            foreach (var user in userRole)
            {
                string fullUrl = $"{baseUrl}/Budget/Transfer/Approval/Default";
                string body = EmailTemplateBuilder.BuildT1CEmailBody(Fo, actionName, fullUrl, user);
                SendFunctionEmail(user.Email, actionName, body);
            }
        }
        #endregion

        #region  Transfer budget For Resubmit Logic
        public static void EmailsReqTransferBudgetForResubmitModified(Guid id, TransfersTransaction ABR, string roleCode, string baseUrl)
        {
            decimal amount = ABR.FromTransfer ?? 0;

            var db = new AppDbContext();
            List<User> userRole = GetUsersDetails(db, roleCode, ABR.BA); 
            string actionName = "Transfer Budget Request";

            foreach (var user in userRole)
            {
                string fullUrl = $"{baseUrl}/Budget/Transfer/Approval/Default";
                string body = EmailTemplateBuilder.BuildTransferEmailBody(ABR, actionName, fullUrl, user);
                SendFunctionEmail(user.Email, actionName, body);
            }
        }
        #endregion

        #region  T2 Approver Logic
        public static void EmailsReqT2ForApproverModified(Guid id, Models.FormsProcurement Fo, string baseUrl)
        {
            AppDbContext db;
            string nextApproverCode = GetT2OwnerCode(out db, id); 

            List<User> userRole = GetUsersDetails(db, nextApproverCode, Fo.BizAreaCode);
            string actionName = "";
            string fullUrl = "";
            var budgettype = db.BudgetTypes.Where(x => x.Id == Fo.TypeId).Select(x => x.Name).FirstOrDefault();

            foreach (var user in userRole)
            {

                if (Fo.Status == "Approved") // completed Transaction budget
                {
                    fullUrl = $"{baseUrl}/T1C/PoolBudget/View?Id={id}";
                    actionName = "Completed T1C Budget";
                } 

                string body = EmailTemplateBuilder.BuildT2EmailBody(Fo, actionName, fullUrl, budgettype, user);
                SendFunctionEmail(user.Email, actionName, body);
            }
        }
        private static string GetT2OwnerCode( out AppDbContext db, Guid? id = null)
        { 
            db = new AppDbContext();
            Guid? eligibleID = Guid.Empty;

            if (id.HasValue)
            {
                eligibleID = db.FormsProcurement
                    .Where(x => x.Id == id.Value)
                    .Select(x => x.CreatedBy)
                    .FirstOrDefault();
            }

            var user = db.Users
                .Where(x => x.Id == eligibleID)
                .FirstOrDefault();

            return user?.iPMSRoleCode; 
        }
        #endregion


        #region  T1C Approver Logic
        public static void EmailsReqT1CForApproverModified(Guid id, Models.Form Fo, string roleCode, string baseUrl)
        {
            AppDbContext db;
            string nextApproverCode = GetNextT1CApproverCode(Fo.Amount ?? 0, roleCode, out db, id);
            if (string.IsNullOrEmpty(nextApproverCode))
            {
                nextApproverCode = GetNextT1CApproverCode(Fo.Amount ?? 0, "", out db, id);
                if (string.IsNullOrEmpty(nextApproverCode)) return;
            }

            List<User> userRole = GetUsersDetails(db, nextApproverCode, Fo.BizAreaCode); 
            string actionName = "";
            string fullUrl = "";

            foreach (var user in userRole)
            {

                if (Fo.Status == "Approved") // completed Transaction budget
                {
                    fullUrl = $"{baseUrl}/T1C/View?Id={id}";
                    actionName = "Completed T1C Budget";
                }
                else if (Fo.Status == "SentBack" && roleCode == "") // resubmit approver to creator
                {
                    fullUrl = $"{baseUrl}/T1C/Default";
                    actionName = "T1C Budget Request Revision";
                }
                else // verify approver to next approver
                {
                    fullUrl = $"{baseUrl}/T1C/Approval/Default";
                    actionName = "T1C Budget Request";
                }

                string body = EmailTemplateBuilder.BuildT1CEmailBody(Fo, actionName, fullUrl, user);
                SendFunctionEmail(user.Email, actionName, body);
            }
        }
        private static string GetNextT1CApproverCode(decimal amount, string roleCode, out AppDbContext db, Guid? id = null)
        {
            if (roleCode == "") // resubmit approver to creator
            {
                db = new AppDbContext();
                Guid? eligibleID = Guid.Empty;

                if (id.HasValue)
                {
                    eligibleID = db.Forms
                        .Where(x => x.Id == id.Value)
                        .Select(x => x.CreatedBy)
                        .FirstOrDefault();
                }

                var user = db.Users
                    .Where(x => x.Id == eligibleID)
                    .FirstOrDefault();

                return user?.iPMSRoleCode;
            }
            else // verify approver to next approver
            {
                db = new AppDbContext();
                var eligible = db.ApprovalLimits
                    .Where(x => x.DeletedDate == null && x.AmountMin <= amount && (x.AmountMax == null || amount <= x.AmountMax))
                    .OrderBy(x => x.Order)
                    .ToList();

                var current = eligible.FirstOrDefault(x => x.ApproverCode == roleCode);
                var next = eligible.FirstOrDefault(x => current != null && x.Order > current.Order);

                return next?.ApproverCode;
            }

        }
        #endregion

        #region  Additional budget For Approver Logic

        public static void EmailsReqAdditionalBudgetForApproverModified(Guid id, AdditionalBudgetRequests ABR, string roleCode, string baseUrl)
        {
            string type = ABR.CheckType == "FINANCE" ? "Finance" : "COGS";
            decimal amount = ABR.AdditionalBudget ?? 0;

            string nextApproverCode = GetNextApproverCodeForAdditionalBudget(amount, roleCode, type, out var db, id);
            if (string.IsNullOrEmpty(nextApproverCode)) return;

            List<User> userRole = GetUsersDetails(db, nextApproverCode, ABR.BA); 
            string actionName = "Additional Budget Request";


            foreach (var user in userRole)
            {
                string fullUrl;
                if (ABR.Status == 3) // completed additional budget
                {
                    fullUrl = $"{baseUrl}/Budget/Additional/View?Id={id}";
                    actionName = "Completed Additional Budget";
                }
                else if (roleCode == "") // resubmit approver to creator
                {
                    fullUrl = $"{baseUrl}/Budget/Additional/Default";
                    actionName = "Additional Budget Request Revision";
                }
                else // verify approver to next approver
                {
                    fullUrl = $"{baseUrl}/Budget/Additional/Approval/{type}/Default";
                    actionName = "Additional Budget Request";
                } 
                string body = EmailTemplateBuilder.BuildAdditionalEmailBody(ABR, actionName, fullUrl, user);
                SendFunctionEmail(user.Email, actionName, body);
            }
        }

        private static string GetNextApproverCodeForAdditionalBudget(decimal amount, string roleCode, string type, out AppDbContext db, Guid? id = null)
        {
            db = new AppDbContext();
            if (roleCode == "") // resubmit approver to creator
            {
                db = new AppDbContext();
                Guid? eligibleID = Guid.Empty;

                if (id.HasValue)
                {
                    eligibleID = db.AdditionalBudgetRequests
                        .Where(x => x.Id == id.Value)
                        .Select(x => x.CreatedBy)
                        .FirstOrDefault();
                }

                var user = db.Users
                    .Where(x => x.Id == eligibleID)
                    .FirstOrDefault();

                return user?.iPMSRoleCode;
            }
            else
            {
                if (type == "Finance")
                {
                    var eligible = db.AdditionalLoaFinanceLimits
                        .Where(x => x.DeletedDate == null && x.AmountMin <= amount && (x.AmountMax == null || amount <= x.AmountMax))
                        .OrderBy(x => x.Order)
                        .ToList();

                    var current = eligible.FirstOrDefault(x => x.FinanceApproverCode == roleCode);
                    var next = eligible.FirstOrDefault(x => current != null && x.Order > current.Order);

                    return next?.FinanceApproverCode;
                }
                else
                {
                    var eligible = db.AdditionalLoaCogsLimits
                        .Where(x => x.DeletedDate == null && x.AmountMin <= amount && (x.AmountMax == null || amount <= x.AmountMax))
                        .OrderBy(x => x.Order)
                        .ToList();

                    var current = eligible.FirstOrDefault(x => x.CogsApproverCode == roleCode);
                    var next = eligible.FirstOrDefault(x => current != null && x.Order > current.Order);

                    return next?.CogsApproverCode;
                }
            }
        }
        #endregion

        #region Function 

        private static List<User> GetUsersDetails(AppDbContext db, string ApproverCode, string bizAreaCode)
        {
            string zoneCode = Class.IPMSBizArea.GetZoneCodeByBizAreaCode(bizAreaCode);
            var users = db.Users.Where(x => x.iPMSRoleCode == ApproverCode).ToList();

            var filtered = !string.IsNullOrEmpty(zoneCode)
                ? users.Where(x => x.iPMSBizAreaCode == zoneCode).ToList()
                : new List<User>();

            return filtered.Any()
                ? filtered
                : users.Where(x => x.iPMSBizAreaCode == bizAreaCode).ToList();
        }

        //public static void SendFunctionEmail(string email, string subject, string body)
        //{
        //    try
        //    {
        //        var smtpClient = new SmtpClient("sandbox.smtp.mailtrap.io", 25)
        //        {
        //            Credentials = new NetworkCredential("cdc48d9f523501", "28fdc344fda835"),    //ijat
        //            //Credentials = new NetworkCredential("b79f1fe30948da", "b0fbba86f8d7da"), // capek
        //            EnableSsl = true
        //        };

        //        MailMessage mailMessage = new MailMessage
        //        {
        //            From = new MailAddress("siserver.fps@fgvholdings.com"),
        //            Subject = "(CCMS) " + subject,
        //            Body = body,
        //            IsBodyHtml = true
        //        };

        //        mailMessage.To.Add(email);
        //        smtpClient.Send(mailMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        HttpContext.Current.Response.Write("<p>Email Error: " + ex.Message + "</p>");
        //    }
        //}
        public static void SendFunctionEmail(string email, string subject, string body)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient("mx.felda.net.my")
                {
                    Port = 25,
                    UseDefaultCredentials = false
                };

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("siserver.fps@fgvholdings.com"),
                    Subject = "(CCMS) " + subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write("<p>Email Error: " + ex.Message + "</p>");
            }
        }
        #endregion
    }
}

#region backup code
//using Prodata.WebForm.Models;
//using Prodata.WebForm.Models.Auth;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.IO;
//using System.Linq;
//using System.Net.Mail;
//using System.Web;

//namespace Prodata.WebForm.Class
//{
//    public class Emails
//    {
//        public static void EmailsReqAdditionalBudget(Guid id, AdditionalBudgetRequests ABR, string roleCode)
//        { EmailsReqAdditionalBudgetModified(id, ABR, roleCode); }
//        public static void EmailsReqTransferBudget(Guid id, TransfersTransaction TT, string roleCode)
//        { EmailsReqTransferBudgetModified(id, TT, roleCode); }


//        public static void EmailsReqTransferBudgetModified(Guid id, TransfersTransaction TT, string roleCode)
//        {
//            {
//                string nextApproverCode = "";
//                List<User> userRole = new List<User>();

//                using (var db = new AppDbContext())
//                {
//                    decimal amount = TT.FromTransfer ?? 0;

//                    // Step 1: Get all eligible approvers based on amount
//                    var eligible = db.TransferApprovalLimits
//                        .Where(x => x.DeletedDate == null
//                                 && x.AmountMin <= amount
//                                 && (x.AmountMax == null || amount <= x.AmountMax))
//                        .ToList();

//                    // Step 2: Get the current approver by roleCode
//                    var current = eligible
//                        .Where(x => x.TransApproverCode == roleCode)
//                        .OrderBy(x => x.Order)
//                        .FirstOrDefault();

//                    if (current != null)
//                    {
//                        // Step 3: Get the next higher order approver from the same eligible list
//                        var nextApprover = eligible
//                            .Where(x => x.Order > current.Order)
//                            .OrderBy(x => x.Order)
//                            .FirstOrDefault();

//                        if (nextApprover != null)
//                        {
//                            // You can now use nextApprover to send an email or log something
//                            nextApproverCode = nextApprover.TransApproverCode;
//                        }
//                        else
//                        {
//                            return;
//                        }
//                    }
//                    else
//                    {
//                        return;
//                    }
//                    string accessibleBizAreas = Class.IPMSBizArea.GetZoneCodeByBizAreaCode(TT.BA);

//                    userRole = db.Users
//                        .Where(x => x.iPMSRoleCode == nextApproverCode)
//                        .ToList();

//                    if (!string.IsNullOrEmpty(accessibleBizAreas))
//                    {
//                        var filtered = userRole
//                            .Where(x => x.iPMSBizAreaCode == accessibleBizAreas)
//                            .ToList();

//                        // Fallback if result is empty
//                        userRole = filtered.Any()
//                            ? filtered
//                            : userRole.Where(x => x.iPMSBizAreaCode == TT.BA).ToList();
//                    }
//                    else
//                    {
//                        userRole = userRole
//                            .Where(x => x.iPMSBizAreaCode == TT.BA)
//                            .ToList();
//                    }
//                }

//                string baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

//                string fullUrl = $"{baseUrl}/Budget/Transfer/Approval/Approval?Id={id}";
//                string ActionName = "Transfer Budget Request";

//                string body = $@"
//                            <p>Dear CCMS User,</p>
//                            <p>Please review the details below:</p>

//                            <table style='border-collapse: collapse; width: 100%; font-family: Arial, sans-serif; font-size: 14px;'>
//                                <tr style='background-color: #f2f2f2;'>
//                                    <th style='border: 1px solid #ddd; padding: 8px;'>Field</th>
//                                    <th style='border: 1px solid #ddd; padding: 8px;'>Value</th>
//                                </tr>
//                                <tr>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>Biz Area</td>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>{TT.BA}</td>
//                                </tr>
//                                <tr>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>Ref No</td>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>{TT.RefNo}</td>
//                                </tr>
//                                <tr>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>Project</td>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>{TT.Project}</td>
//                                </tr>
//                                <tr>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>Application Date</td>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>{TT.Date}</td>
//                                </tr>
//                                <tr>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>Budget Type</td>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>{TT.BudgetType}</td>
//                                </tr> 
//                                <tr>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>Estimated Cost</td>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>{TT.EstimatedCost}</td>
//                                </tr>
//                                <tr>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>Transfer Budget</td>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>{TT.FromTransfer}</td>
//                                </tr>
//                                <tr>
//                                    <td style='border: 1px solid #ddd; padding: 8px; vertical-align: top;'>Request Justification</td>
//                                    <td style='border: 1px solid #ddd; padding: 12px; white-space: pre-wrap;'>{TT.Justification}</td>
//                                </tr>
//                                <tr>
//                                    <td style='border: 1px solid #ddd; padding: 8px; vertical-align: top;'>Work Details</td>
//                                    <td style='border: 1px solid #ddd; padding: 12px; white-space: pre-wrap;'>{TT.WorkDetails}</td>
//                                </tr>
//                            </table>

//                            <p style='margin-top: 20px;'>Please review the <strong>{ActionName}</strong> by clicking the link below:</p>
//                            <p><a href='{fullUrl}' target='_blank'>{ActionName} (CCMS)</a></p>
//                            <p>Thank you.</p>";

//                foreach (var user in userRole)
//                {
//                    SendFunctionEmail(user.Email, ActionName, body);
//                }
//            }
//        }

//        public static void EmailsReqAdditionalBudgetModified(Guid id, AdditionalBudgetRequests ABR, string roleCode)
//        {
//            string nextApproverCode = "";
//            List<User> userRole = new List<User>();
//            string type = "";

//            if (ABR.CheckType == "FINANCE")
//            {
//                type = "Finance";
//            }
//            else
//            {
//                type = "COGS";

//            }
//            using (var db = new AppDbContext())
//            {
//                decimal amount = ABR.AdditionalBudget ?? 0;

//                if (type == "Finance")
//                {
//                    // Step 1: Get all eligible approvers based on amount
//                    var eligible = db.AdditionalLoaFinanceLimits
//                        .Where(x => x.DeletedDate == null
//                                 && x.AmountMin <= amount
//                                 && (x.AmountMax == null || amount <= x.AmountMax))
//                        .ToList();

//                    // Step 2: Get the current approver by roleCode
//                    var current = eligible
//                        .Where(x => x.FinanceApproverCode == roleCode)
//                        .OrderBy(x => x.Order)
//                        .FirstOrDefault();

//                    if (current != null)
//                    {
//                        // Step 3: Get the next higher order approver from the same eligible list
//                        var nextApprover = eligible
//                            .Where(x => x.Order > current.Order)
//                            .OrderBy(x => x.Order)
//                            .FirstOrDefault();

//                        if (nextApprover != null)
//                        {
//                            // You can now use nextApprover to send an email or log something
//                            nextApproverCode = nextApprover.FinanceApproverCode;
//                        }
//                        else
//                        {
//                            return;
//                        }
//                    }
//                    else
//                    {
//                        return;
//                    }
//                }
//                else
//                {
//                    // Step 1: Get all eligible approvers based on amount
//                    var eligible = db.AdditionalLoaCogsLimits
//                        .Where(x => x.DeletedDate == null
//                                 && x.AmountMin <= amount
//                                 && (x.AmountMax == null || amount <= x.AmountMax))
//                        .ToList();

//                    // Step 2: Get the current approver by roleCode
//                    var current = eligible
//                        .Where(x => x.CogsApproverCode == roleCode)
//                        .OrderBy(x => x.Order)
//                        .FirstOrDefault();

//                    if (current != null)
//                    {
//                        // Step 3: Get the next higher order approver from the same eligible list
//                        var nextApprover = eligible
//                            .Where(x => x.Order > current.Order)
//                            .OrderBy(x => x.Order)
//                            .FirstOrDefault();

//                        if (nextApprover != null)
//                        {
//                            // You can now use nextApprover to send an email or log something
//                            nextApproverCode = nextApprover.CogsApproverCode;
//                        }
//                        else
//                        {
//                            return;
//                        }
//                    }
//                    else
//                    {
//                        return;
//                    }
//                }
//                string accessibleBizAreas = Class.IPMSBizArea.GetZoneCodeByBizAreaCode(ABR.BA);

//                userRole = db.Users
//                    .Where(x => x.iPMSRoleCode == nextApproverCode)
//                    .ToList();

//                if (!string.IsNullOrEmpty(accessibleBizAreas))
//                {
//                    var filtered = userRole
//                        .Where(x => x.iPMSBizAreaCode == accessibleBizAreas)
//                        .ToList();

//                    // Fallback if result is empty
//                    userRole = filtered.Any()
//                        ? filtered
//                        : userRole.Where(x => x.iPMSBizAreaCode == ABR.BA).ToList();
//                }
//                else
//                {
//                    userRole = userRole
//                        .Where(x => x.iPMSBizAreaCode == ABR.BA)
//                        .ToList();
//                }
//            }

//            string baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

//            string fullUrl = $"{baseUrl}/Budget/Additional/Approval/{type}/Approval?Id={id}";
//            string ActionName = "Additional Budget Request";

//            string body = $@"
//                            <p>Dear CCMS User,</p>
//                            <p>Please review the details below:</p>

//                            <table style='border-collapse: collapse; width: 100%; font-family: Arial, sans-serif; font-size: 14px;'>
//                                <tr style='background-color: #f2f2f2;'>
//                                    <th style='border: 1px solid #ddd; padding: 8px;'>Field</th>
//                                    <th style='border: 1px solid #ddd; padding: 8px;'>Value</th>
//                                </tr>
//                                <tr>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>Biz Area</td>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>{ABR.BA}</td>
//                                </tr>
//                                <tr>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>Ref No</td>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>{ABR.RefNo}</td>
//                                </tr>
//                                <tr>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>Project</td>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>{ABR.Project}</td>
//                                </tr>
//                                <tr>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>Application Date</td>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>{ABR.ApplicationDate}</td>
//                                </tr>
//                                <tr>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>Budget Type</td>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>{ABR.BudgetType}</td>
//                                </tr>
//                                <tr>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>Check Type</td>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>{ABR.CheckType}</td>
//                                </tr>
//                                <tr>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>Estimated Cost</td>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>{ABR.EstimatedCost}</td>
//                                </tr>
//                                <tr>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>Additional Budget</td>
//                                    <td style='border: 1px solid #ddd; padding: 8px;'>{ABR.AdditionalBudget}</td>
//                                </tr>
//                                <tr>
//                                    <td style='border: 1px solid #ddd; padding: 8px; vertical-align: top;'>Request Details</td>
//                                    <td style='border: 1px solid #ddd; padding: 12px; white-space: pre-wrap;'>{ABR.RequestDetails}</td>
//                                </tr>
//                                <tr>
//                                    <td style='border: 1px solid #ddd; padding: 8px; vertical-align: top;'>Reason</td>
//                                    <td style='border: 1px solid #ddd; padding: 12px; white-space: pre-wrap;'>{ABR.Reason}</td>
//                                </tr>
//                            </table>

//                            <p style='margin-top: 20px;'>Please review the <strong>{ActionName}</strong> by clicking the link below:</p>
//                            <p><a href='{fullUrl}' target='_blank'>{ActionName} (CCMS)</a></p>
//                            <p>Thank you.</p>";

//            foreach (var user in userRole)
//            {
//                SendFunctionEmail(user.Email, ActionName, body);
//            }
//        } 

//        public static void SendFunctionEmail( string email, string subject, string body)
//        {
//            try
//            {
//                // Set up the SMTP client and credentials
//                SmtpClient smtpClient = new SmtpClient("mx.felda.net.my"); // Replace with your SMTP server
//                smtpClient.Port = 25; // Specify the SMTP server port
//                smtpClient.UseDefaultCredentials = false;
//                //smtpClient.Credentials = new NetworkCredential("aimanamir.m@fgvholdings.com", "@!man*FGV23"); // Replace with your email credentials
//                //smtpClient.EnableSsl = true; // Enable SSL/TLS for secure communication

//                // Create the email message
//                MailMessage mailMessage = new MailMessage();
//                mailMessage.From = new MailAddress("siserver.fps@fgvholdings.com"); // Replace with your email
//                mailMessage.To.Add(email);
//                mailMessage.Subject = "(CCMS) " + subject;
//                mailMessage.Body = body;
//                mailMessage.IsBodyHtml = true; // Set to true if the body contains HTML

//                // Send the email
//                smtpClient.Send(mailMessage);
//                //lblMessage.Text = "Email sent successfully!";
//                //lblMessage.Visible = true;
//            }
//            catch (Exception ex)
//            {
//                HttpContext.Current.Response.Write("<p>Email Error: " + ex.Message + "</p>");

//                //lblError.Text = "Error sending emails: " + ex.Message;
//                //lblError.Visible = true;
//            }
//        }
//    }
//}
#endregion