using Prodata.WebForm.Models;
using Prodata.WebForm.Models.Auth;
using Prodata.WebForm.Models.ModelAWO;
using System;
using System.Linq;
using System.Text;

namespace Prodata.WebForm.Class
{
    public static class AWOEmailTemplateBuilder
    {
        public static string BuildAWOEmailBody(Models.ModelAWO.AssetWriteOff awo, string actionName, string fullUrl, User user)
        {
            var rows = new StringBuilder();

            // Format Data
            string appDate = awo.Date.ToString("dd/MM/yyyy");
            string nbv = awo.NetBookValue.ToString("N2");

            // Get Total Assets Count
            int totalAssets = 0;
            using (var db = new AppDbContext())
            {
                totalAssets = db.AssetWriteOffDetails.Count(d => d.WriteOffId == awo.Id);
            }

            // Build Rows
            rows.Append(BuildRow("Biz Area Code", awo.BACode));
            rows.Append(BuildRow("Reference No.", awo.RequestNo));
            rows.Append(BuildRow("Application Date", appDate));
            rows.Append(BuildRow("Total Assets", totalAssets.ToString()));

            // Highlight the Net Book Value as it drives the approval matrix
            rows.Append(BuildRow("Highest Net Book Value", "RM " + nbv, true));

            rows.Append(BuildRow("Project Name", awo.Project));
            rows.Append(BuildRow("Justification", awo.Justification, false, true)); // Setting isLast = true removes the bottom border

            return BuildBaseHtml(user, rows.ToString(), actionName, fullUrl);
        }

        #region Private Helper Methods (Base HTML & Rows)

        private static string BuildBaseHtml(User user, string tableRows, string actionName, string fullUrl)
        {
            // Modern Corporate Color Palette
            string bodyBg = "#F4F7F6";
            string containerBg = "#FFFFFF";
            string headerBg = "#0F2E4A"; // Deep Navy Blue
            string primaryBtn = "#0056B3"; // Bright Action Blue
            string textDark = "#2C3E50";
            string textMuted = "#7F8C8D";
            string borderSubtle = "#EAECEE";

            return $@"
            <!DOCTYPE html>
            <html lang='en'>
            <head>
                <meta charset='utf-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>CCMS Notification</title>
                <style>
                    /* Reset styles for email clients */
                    body, table, td, a {{ -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; }}
                    table, td {{ mso-table-lspace: 0pt; mso-table-rspace: 0pt; border-collapse: collapse !important; }}
                    img {{ -ms-interpolation-mode: bicubic; border: 0; height: auto; line-height: 100%; outline: none; text-decoration: none; }}
                    body {{ margin: 0 !important; padding: 0 !important; width: 100% !important; font-family: 'Segoe UI', Helvetica, Arial, sans-serif; background-color: {bodyBg}; }}
                    
                    /* Hover effects (where supported) */
                    .btn-action:hover {{ background-color: #004494 !important; }}
                </style>
            </head>
            <body style='background-color: {bodyBg}; margin: 0; padding: 40px 20px;'>
                
                <table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color: {bodyBg};'>
                    <tr>
                        <td align='center'>
                            
                            <table border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width: 640px; background-color: {containerBg}; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
                                
                                <tr>
                                    <td height='6' style='background-color: {primaryBtn}; line-height: 6px; font-size: 6px;'>&nbsp;</td>
                                </tr>

                                <tr>
                                    <td align='center' style='background-color: {headerBg}; padding: 35px 20px;'>
                                        <h1 style='color: #FFFFFF; font-size: 22px; font-weight: 600; margin: 0; letter-spacing: 0.5px;'>
                                            Asset Write-Off Notification
                                        </h1>
                                        <p style='color: #AAB7B8; font-size: 14px; margin: 8px 0 0 0;'>
                                            CCMS Financial Control System
                                        </p>
                                    </td>
                                </tr>

                                <tr>
                                    <td style='padding: 40px 40px 30px 40px;'>
                                        
                                        <p style='margin: 0 0 10px 0; color: {textDark}; font-size: 16px; font-weight: 600;'>
                                            Dear {user.Name},
                                        </p>
                                        <p style='margin: 0 0 30px 0; color: {textMuted}; font-size: 15px; line-height: 1.6;'>
                                            An Asset Write-Off application requires your attention. You are receiving this notification because you are assigned as <strong style='color: {textDark};'>{user.CCMSRoleCode}</strong> in the CCMS matrix. Please review the details below.
                                        </p>

                                        <table border='0' cellpadding='0' cellspacing='0' width='100%' style='border: 1px solid {borderSubtle}; border-radius: 8px; overflow: hidden;'>
                                            {tableRows}
                                        </table>

                                        <table border='0' cellpadding='0' cellspacing='0' width='100%' style='margin-top: 35px;'>
                                            <tr>
                                                <td align='center'>
                                                    <table border='0' cellpadding='0' cellspacing='0'>
                                                        <tr>
                                                            <td align='center' bgcolor='{primaryBtn}' style='border-radius: 6px;'>
                                                                <a href='{fullUrl}' target='_blank' class='btn-action' style='display: inline-block; padding: 14px 35px; font-family: Helvetica, Arial, sans-serif; font-size: 16px; color: #ffffff; text-decoration: none; border-radius: 6px; font-weight: bold;'>
                                                                    {actionName}
                                                                </a>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                        
                                        <p style='margin: 30px 0 0 0; text-align: center; font-size: 12px; color: {textMuted}; line-height: 1.5;'>
                                            If the button above does not work, copy and paste this link into your browser:<br>
                                            <a href='{fullUrl}' style='color: {primaryBtn}; text-decoration: none; word-break: break-all;'>{fullUrl}</a>
                                        </p>
                                    </td>
                                </tr>

                                <tr>
                                    <td align='center' style='background-color: #F8F9FA; padding: 25px 40px; border-top: 1px solid {borderSubtle};'>
                                        <p style='margin: 0; color: #95A5A6; font-size: 12px; line-height: 1.6;'>
                                            &copy; {DateTime.Now.Year} FGV Holdings Berhad. All rights reserved.<br>
                                            This is an automated message generated by the CCMS System. Please do not reply to this email.
                                        </p>
                                    </td>
                                </tr>

                            </table>
                            </td>
                    </tr>
                </table>
            </body>
            </html>";
        }

        private static string BuildRow(string label, string value, bool highlight = false, bool isLast = false)
        {
            string displayValue = string.IsNullOrWhiteSpace(value) ? "-" : value;
            string borderStyle = isLast ? "" : "border-bottom: 1px solid #EAECEE;";

            if (highlight)
            {
                // Highlighted Row (e.g., Highest NBV)
                return $@"
                <tr>
                    <td width='35%' style='padding: 16px 20px; font-size: 14px; font-weight: 600; color: #B9770E; background-color: #FCF3CF; {borderStyle} border-right: 1px solid #F5DA81; vertical-align: top;'>
                        {label}
                    </td>
                    <td width='65%' style='padding: 16px 20px; font-size: 15px; font-weight: bold; color: #9C640C; background-color: #FEF9E7; {borderStyle} vertical-align: top;'>
                        {displayValue}
                    </td>
                </tr>";
            }
            else
            {
                // Standard Row
                return $@"
                <tr>
                    <td width='35%' style='padding: 16px 20px; font-size: 14px; font-weight: 600; color: #566573; background-color: #F8F9FA; {borderStyle} border-right: 1px solid #EAECEE; vertical-align: top;'>
                        {label}
                    </td>
                    <td width='65%' style='padding: 16px 20px; font-size: 14px; color: #2C3E50; background-color: #FFFFFF; {borderStyle} vertical-align: top; white-space: pre-wrap; line-height: 1.5;'>
                        {displayValue}
                    </td>
                </tr>";
            }
        }
        #endregion
    }
}