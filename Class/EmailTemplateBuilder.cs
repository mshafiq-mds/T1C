using Prodata.WebForm.Models;
using Prodata.WebForm.Models.Auth;
using System;
using System.Text;
using System.Web;

namespace Prodata.WebForm.Class
{
    public static class EmailTemplateBuilder
    {
        #region Public Methods

        public static string BuildAdditionalEmailBody(AdditionalBudgetRequests abr, string actionName, string fullUrl, User user)
        {
            var rows = new StringBuilder();
            rows.Append(BuildRow("Biz Area", abr.BA));
            rows.Append(BuildRow("Ref No", abr.RefNo));
            rows.Append(BuildRow("Project", abr.Project));
            rows.Append(BuildRow("Application Date", abr.ApplicationDate.ToString("dd/MM/yyyy")));
            rows.Append(BuildRow("Budget Type", abr.BudgetType));
            rows.Append(BuildRow("Check Type", abr.CheckType));
            rows.Append(BuildRow("Estimated Cost", abr.EstimatedCost.ToString()));
            rows.Append(BuildRow("Additional Budget", abr.AdditionalBudget.ToString()));
            rows.Append(BuildRow("Request Details", abr.RequestDetails));
            rows.Append(BuildRow("Reason", abr.Reason));

            return BuildBaseHtml(user, rows.ToString(), actionName, fullUrl);
        }

        public static string BuildTransferEmailBody(TransfersTransaction tt, string actionName, string fullUrl, User user)
        {
            var rows = new StringBuilder();
            rows.Append(BuildRow("Biz Area", tt.BA));
            rows.Append(BuildRow("Ref No", tt.RefNo));
            rows.Append(BuildRow("Project", tt.Project));
            rows.Append(BuildRow("Application Date", tt.Date.ToString("dd/MM/yyyy")));
            rows.Append(BuildRow("Budget Type", tt.BudgetType));
            rows.Append(BuildRow("Estimated Cost", tt.EstimatedCost.ToString()));
            rows.Append(BuildRow("Transfer Budget", tt.FromTransfer.ToString()));
            rows.Append(BuildRow("Request Justification", tt.Justification));
            rows.Append(BuildRow("Work Details", tt.WorkDetails));

            return BuildBaseHtml(user, rows.ToString(), actionName, fullUrl);
        }

        public static string BuildT1CEmailBody(Models.Form tt, string actionName, string fullUrl, User user)
        {
            var rows = new StringBuilder();
            rows.Append(BuildRow("Biz Area Code", tt.BizAreaCode));
            rows.Append(BuildRow("Biz Area Name", tt.BizAreaName));
            rows.Append(BuildRow("Reference", tt.Ref));
            rows.Append(BuildRow("Application Date", tt.Date?.ToString("dd/MM/yyyy")));
            rows.Append(BuildRow("Estimated Amount", tt.Amount?.ToString("N2")));

            // Conditional Row for Actual Amount
            if (tt.ActualAmount.HasValue)
            {
                rows.Append(BuildRow("Actual Amount (Please Note)", tt.ActualAmount.Value.ToString("N2"), true));
            }

            rows.Append(BuildRow("Procurement Type", tt.ProcurementType));
            rows.Append(BuildRow("Justification of Need", tt.JustificationOfNeed));
            rows.Append(BuildRow("Details", tt.Details));

            return BuildBaseHtml(user, rows.ToString(), actionName, fullUrl);
        }

        public static string BuildT2EmailBody(Models.FormsProcurement tt, string actionName, string fullUrl, string procurementType, User user)
        {
            var rows = new StringBuilder();
            rows.Append(BuildRow("Biz Area Code", tt.BizAreaCode));
            rows.Append(BuildRow("Biz Area Name", tt.BizAreaName));
            rows.Append(BuildRow("Reference", tt.Ref));
            rows.Append(BuildRow("Application Date", tt.Date?.ToString("dd/MM/yyyy")));
            rows.Append(BuildRow("Amount", tt.Amount?.ToString("N2")));

            // Conditional Row for Actual Amount
            if (tt.ActualAmount.HasValue)
            {
                rows.Append(BuildRow("Actual Amount (Please Note)", tt.ActualAmount.Value.ToString("N2"), true));
            }

            rows.Append(BuildRow("Procurement Type", procurementType));
            rows.Append(BuildRow("Justification of Need", tt.JustificationOfNeed));
            rows.Append(BuildRow("Remarks", tt.Remarks));

            return BuildBaseHtml(user, rows.ToString(), actionName, fullUrl);
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Generates a unified HTML structure for the email.
        /// </summary>
        private static string BuildBaseHtml(User user, string tableRows, string actionName, string fullUrl)
        {
            // Styles
            string bodyBg = "#f4f6f9";
            string containerBg = "#ffffff";
            string primaryColor = "#0056b3"; // Professional Blue
            string textColor = "#333333";
            string mutedColor = "#6c757d";

            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <style>
                    body {{ margin: 0; padding: 0; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; -webkit-text-size-adjust: 100%; }}
                    table {{ border-collapse: collapse; }}
                    .button-link:hover {{ background-color: #004494 !important; }}
                </style>
            </head>
            <body style='background-color: {bodyBg}; padding: 20px 0;'>
                <table border='0' cellpadding='0' cellspacing='0' width='100%'>
                    <tr>
                        <td align='center'>
                            <!-- Main Container -->
                            <table border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width: 600px; background-color: {containerBg}; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.05);'>
                                
                                <!-- Header -->
                                <tr>
                                    <td bgcolor='{primaryColor}' style='padding: 20px; text-align: center; color: #ffffff; font-size: 20px; font-weight: 600; letter-spacing: 0.5px;'>
                                        CCMS Notification
                                    </td>
                                </tr>

                                <!-- Body Content -->
                                <tr>
                                    <td style='padding: 30px;'>
                                        <p style='margin: 0 0 15px 0; color: {textColor}; font-size: 16px;'>Dear <strong>{user.Name}</strong>,</p>
                                        <p style='margin: 0 0 25px 0; color: {mutedColor}; font-size: 14px; line-height: 1.5;'>
                                            As a CCMS user with the role <strong>{user.CCMSRoleCode}</strong>, please review the details below:
                                        </p>

                                        <!-- Data Table -->
                                        <table border='0' cellpadding='0' cellspacing='0' width='100%' style='border: 1px solid #e9ecef; border-radius: 4px; overflow: hidden;'>
                                            {tableRows}
                                        </table>

                                        <!-- Call to Action -->
                                        <table width='100%' border='0' cellspacing='0' cellpadding='0' style='margin-top: 30px;'>
                                            <tr>
                                                <td align='center'>
                                                    <a href='{fullUrl}' target='_blank' class='button-link' 
                                                       style='display: inline-block; padding: 12px 30px; background-color: {primaryColor}; color: #ffffff; text-decoration: none; border-radius: 4px; font-weight: bold; font-size: 15px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                                                       View {actionName}
                                                    </a>
                                                </td>
                                            </tr>
                                        </table>
                                        
                                        <p style='margin-top: 30px; text-align: center; font-size: 13px; color: {mutedColor};'>
                                            If the button above does not work, please copy and paste the following link into your browser:<br>
                                            <a href='{fullUrl}' style='color: {primaryColor}; text-decoration: underline; word-break: break-all;'>{fullUrl}</a>
                                        </p>
                                    </td>
                                </tr>

                                <!-- Footer -->
                                <tr>
                                    <td bgcolor='#f8f9fa' style='padding: 15px; text-align: center; color: #adb5bd; font-size: 12px; border-top: 1px solid #e9ecef;'>
                                        &copy; {DateTime.Now.Year} FGV Holdings. All rights reserved.<br>
                                        This is an automated system-generated email. Please do not reply.
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>";
        }

        /// <summary>
        /// Generates a standardized table row.
        /// </summary>
        /// <param name="label">The field label.</param>
        /// <param name="value">The field value.</param>
        /// <param name="highlight">If true, applies a warning style (yellow).</param>
        /// <returns>HTML Table Row string</returns>
        private static string BuildRow(string label, string value, bool highlight = false)
        {
            // Null check for value
            string displayValue = string.IsNullOrWhiteSpace(value) ? "-" : value;

            if (highlight)
            {
                // Highlighted Row Style (e.g., for Actual Amount)
                return $@"
                <tr style='background-color: #fff3cd; border-bottom: 1px solid #ffeeba;'>
                    <td style='padding: 12px 15px; width: 35%; font-size: 14px; font-weight: bold; color: #856404; vertical-align: top; border-right: 1px solid #ffeeba;'>{label}</td>
                    <td style='padding: 12px 15px; font-size: 14px; font-weight: bold; color: #856404; vertical-align: top;'>{displayValue}</td>
                </tr>";
            }
            else
            {
                // Standard Row Style
                return $@"
                <tr style='border-bottom: 1px solid #e9ecef;'>
                    <td style='padding: 12px 15px; width: 35%; font-size: 14px; font-weight: bold; color: #495057; background-color: #f8f9fa; vertical-align: top; border-right: 1px solid #e9ecef;'>{label}</td>
                    <td style='padding: 12px 15px; font-size: 14px; color: #212529; vertical-align: top; white-space: pre-wrap;'>{displayValue}</td>
                </tr>";
            }
        }

        #endregion
    }
}

//using Prodata.WebForm.Models;
//using Prodata.WebForm.Models.Auth;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace Prodata.WebForm.Class
//{
//    public static class EmailTemplateBuilder
//    {
//        public static string BuildAdditionalEmailBody(AdditionalBudgetRequests abr, string actionName, string fullUrl, User user)
//        {
//            return $@"
//                <p>Dear {user.Name},</p>
//                <p>As a CCMS user with the role <strong>{user.CCMSRoleCode}</strong>, please view the details below:</p>

//                <table style='border-collapse: collapse; width: 100%; font-family: Arial, sans-serif; font-size: 14px;'>
//                    <tr style='background-color: #f2f2f2;'>
//                        <th style='border: 1px solid #ddd; padding: 8px;'>Field</th>
//                        <th style='border: 1px solid #ddd; padding: 8px;'>Details</th>
//                    </tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Biz Area</td><td style='border: 1px solid #ddd; padding: 8px;'>{abr.BA}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Ref No</td><td style='border: 1px solid #ddd; padding: 8px;'>{abr.RefNo}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Project</td><td style='border: 1px solid #ddd; padding: 8px;'>{abr.Project}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Application Date</td><td style='border: 1px solid #ddd; padding: 8px;'>{abr.ApplicationDate.ToString("dd/MM/yyyy")}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Budget Type</td><td style='border: 1px solid #ddd; padding: 8px;'>{abr.BudgetType}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Check Type</td><td style='border: 1px solid #ddd; padding: 8px;'>{abr.CheckType}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Estimated Cost</td><td style='border: 1px solid #ddd; padding: 8px;'>{abr.EstimatedCost}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Additional Budget</td><td style='border: 1px solid #ddd; padding: 8px;'>{abr.AdditionalBudget}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px; vertical-align: top;'>Request Details</td><td style='border: 1px solid #ddd; padding: 12px; white-space: pre-wrap;'>{abr.RequestDetails}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px; vertical-align: top;'>Reason</td><td style='border: 1px solid #ddd; padding: 12px; white-space: pre-wrap;'>{abr.Reason}</td></tr>
//                </table>

//                <p style='margin-top: 20px;'>Please view the <strong>{actionName}</strong> by clicking the link below:</p>
//                <p><a href='{fullUrl}' target='_blank'>{actionName} (CCMS)</a></p>
//                <p>Thank you.</p>";
//        }

//        public static string BuildTransferEmailBody(TransfersTransaction tt, string actionName, string fullUrl, User user)
//        {
//            return $@"
//                <p>Dear {user.Name},</p>
//                <p>As a CCMS user with the role <strong>{user.CCMSRoleCode}</strong>, please view the details below:</p>

//                <table style='border-collapse: collapse; width: 100%; font-family: Arial, sans-serif; font-size: 14px;'>
//                    <tr style='background-color: #f2f2f2;'>
//                        <th style='border: 1px solid #ddd; padding: 8px;'>Field</th>
//                        <th style='border: 1px solid #ddd; padding: 8px;'>Details</th>
//                    </tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Biz Area</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.BA}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Ref No</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.RefNo}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Project</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.Project}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Application Date</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.Date.ToString("dd/MM/yyyy")}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Budget Type</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.BudgetType}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Estimated Cost</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.EstimatedCost}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Transfer Budget</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.FromTransfer}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px; vertical-align: top;'>Request Justification</td><td style='border: 1px solid #ddd; padding: 12px; white-space: pre-wrap;'>{tt.Justification}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px; vertical-align: top;'>Work Details</td><td style='border: 1px solid #ddd; padding: 12px; white-space: pre-wrap;'>{tt.WorkDetails}</td></tr>
//                </table>

//                <p style='margin-top: 20px;'>Please view the <strong>{actionName}</strong> by clicking the link below:</p>
//                <p><a href='{fullUrl}' target='_blank'>{actionName} (CCMS)</a></p>
//                <p>Thank you.</p>";
//        }

//        public static string BuildT1CEmailBody(Models.Form tt, string actionName, string fullUrl, User user)
//        {
//            // Build Actual Amount row conditionally
//            string actualAmountRow = string.Empty;
//            if (tt.ActualAmount.HasValue)
//            {
//                actualAmountRow = $@"
//            <tr style='background-color: #fff3cd;'>
//                <td style='border: 1px solid #ddd; padding: 8px; font-weight: bold; color: #856404;'>Actual Amount (Please Note)</td>
//                <td style='border: 1px solid #ddd; padding: 8px; font-weight: bold; color: #856404;'>{tt.ActualAmount.Value.ToString("N2")}</td>
//            </tr>";
//            }

//            return $@"
//                <p>Dear {user.Name},</p>
//                <p>As a CCMS user with the role <strong>{user.CCMSRoleCode}</strong>, please view the details below:</p>

//                <table style='border-collapse: collapse; width: 100%; font-family: Arial, sans-serif; font-size: 14px;'>
//                    <tr style='background-color: #f2f2f2;'>
//                        <th style='border: 1px solid #ddd; padding: 8px;'>Field</th>
//                        <th style='border: 1px solid #ddd; padding: 8px;'>Details</th>
//                    </tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Biz Area Code</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.BizAreaCode}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Biz Area Name</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.BizAreaName}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Reference</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.Ref}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Application Date</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.Date?.ToString("dd/MM/yyyy")}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Estimated Amount</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.Amount?.ToString("N2")}</td></tr>
//                    {actualAmountRow}
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Procurement Type</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.ProcurementType}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px; vertical-align: top;'>Justification of Need</td><td style='border: 1px solid #ddd; padding: 12px; white-space: pre-wrap;'>{tt.JustificationOfNeed}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px; vertical-align: top;'>Remarks</td><td style='border: 1px solid #ddd; padding: 12px; white-space: pre-wrap;'>{tt.Remarks}</td></tr>
//                </table>

//                <p style='margin-top: 20px;'>Please view the <strong>{actionName}</strong> by clicking the link below:</p>
//                <p><a href='{fullUrl}' target='_blank'>{actionName} (CCMS)</a></p>
//                <p>Thank you.</p>";
//        }

//        public static string BuildT2EmailBody(Models.FormsProcurement tt, string actionName, string fullUrl, string ProcurementType, User user)
//        {
//            // Build Actual Amount row conditionally
//            string actualAmountRow = string.Empty;
//            if (tt.ActualAmount.HasValue)
//            {
//                actualAmountRow = $@"
//            <tr style='background-color: #fff3cd;'>
//                <td style='border: 1px solid #ddd; padding: 8px; font-weight: bold; color: #856404;'>Actual Amount (Please Note)</td>
//                <td style='border: 1px solid #ddd; padding: 8px; font-weight: bold; color: #856404;'>{tt.ActualAmount.Value.ToString("N2")}</td>
//            </tr>";
//            }

//            return $@"
//                <p>Dear {user.Name},</p>
//                <p>As a CCMS user with the role <strong>{user.CCMSRoleCode}</strong>, please view the details below:</p>

//                <table style='border-collapse: collapse; width: 100%; font-family: Arial, sans-serif; font-size: 14px;'>
//                    <tr style='background-color: #f2f2f2;'>
//                        <th style='border: 1px solid #ddd; padding: 8px;'>Field</th>
//                        <th style='border: 1px solid #ddd; padding: 8px;'>Details</th>
//                    </tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Biz Area Code</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.BizAreaCode}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Biz Area Name</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.BizAreaName}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Reference</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.Ref}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Application Date</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.Date?.ToString("dd/MM/yyyy")}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Amount</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.Amount?.ToString("N2")}</td></tr>
//                    {actualAmountRow}
//                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Procurement Type</td><td style='border: 1px solid #ddd; padding: 8px;'>{ProcurementType}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px; vertical-align: top;'>Justification of Need</td><td style='border: 1px solid #ddd; padding: 12px; white-space: pre-wrap;'>{tt.JustificationOfNeed}</td></tr>
//                    <tr><td style='border: 1px solid #ddd; padding: 8px; vertical-align: top;'>Remarks</td><td style='border: 1px solid #ddd; padding: 12px; white-space: pre-wrap;'>{tt.Remarks}</td></tr>
//                </table>

//                <p style='margin-top: 20px;'>Please view the <strong>{actionName}</strong> by clicking the link below:</p>
//                <p><a href='{fullUrl}' target='_blank'>{actionName} (CCMS)</a></p>
//                <p>Thank you.</p>";
//        }
//    }
//}
