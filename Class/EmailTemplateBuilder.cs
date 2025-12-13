using Prodata.WebForm.Models;
using Prodata.WebForm.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prodata.WebForm.Class
{
    public static class EmailTemplateBuilder
    {
        public static string BuildAdditionalEmailBody(AdditionalBudgetRequests abr, string actionName, string fullUrl, User user)
        {
            return $@"
                <p>Dear {user.Name},</p>
                <p>As a CCMS user with the role <strong>{user.CCMSRoleCode}</strong>, please view the details below:</p>

                <table style='border-collapse: collapse; width: 100%; font-family: Arial, sans-serif; font-size: 14px;'>
                    <tr style='background-color: #f2f2f2;'>
                        <th style='border: 1px solid #ddd; padding: 8px;'>Field</th>
                        <th style='border: 1px solid #ddd; padding: 8px;'>Details</th>
                    </tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Biz Area</td><td style='border: 1px solid #ddd; padding: 8px;'>{abr.BA}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Ref No</td><td style='border: 1px solid #ddd; padding: 8px;'>{abr.RefNo}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Project</td><td style='border: 1px solid #ddd; padding: 8px;'>{abr.Project}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Application Date</td><td style='border: 1px solid #ddd; padding: 8px;'>{abr.ApplicationDate.ToString("dd/MM/yyyy")}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Budget Type</td><td style='border: 1px solid #ddd; padding: 8px;'>{abr.BudgetType}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Check Type</td><td style='border: 1px solid #ddd; padding: 8px;'>{abr.CheckType}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Estimated Cost</td><td style='border: 1px solid #ddd; padding: 8px;'>{abr.EstimatedCost}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Additional Budget</td><td style='border: 1px solid #ddd; padding: 8px;'>{abr.AdditionalBudget}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px; vertical-align: top;'>Request Details</td><td style='border: 1px solid #ddd; padding: 12px; white-space: pre-wrap;'>{abr.RequestDetails}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px; vertical-align: top;'>Reason</td><td style='border: 1px solid #ddd; padding: 12px; white-space: pre-wrap;'>{abr.Reason}</td></tr>
                </table>

                <p style='margin-top: 20px;'>Please view the <strong>{actionName}</strong> by clicking the link below:</p>
                <p><a href='{fullUrl}' target='_blank'>{actionName} (CCMS)</a></p>
                <p>Thank you.</p>";
        }

        public static string BuildTransferEmailBody(TransfersTransaction tt, string actionName, string fullUrl, User user)
        {
            return $@"
                <p>Dear {user.Name},</p>
                <p>As a CCMS user with the role <strong>{user.CCMSRoleCode}</strong>, please view the details below:</p>

                <table style='border-collapse: collapse; width: 100%; font-family: Arial, sans-serif; font-size: 14px;'>
                    <tr style='background-color: #f2f2f2;'>
                        <th style='border: 1px solid #ddd; padding: 8px;'>Field</th>
                        <th style='border: 1px solid #ddd; padding: 8px;'>Details</th>
                    </tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Biz Area</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.BA}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Ref No</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.RefNo}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Project</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.Project}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Application Date</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.Date.ToString("dd/MM/yyyy")}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Budget Type</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.BudgetType}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Estimated Cost</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.EstimatedCost}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Transfer Budget</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.FromTransfer}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px; vertical-align: top;'>Request Justification</td><td style='border: 1px solid #ddd; padding: 12px; white-space: pre-wrap;'>{tt.Justification}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px; vertical-align: top;'>Work Details</td><td style='border: 1px solid #ddd; padding: 12px; white-space: pre-wrap;'>{tt.WorkDetails}</td></tr>
                </table>

                <p style='margin-top: 20px;'>Please view the <strong>{actionName}</strong> by clicking the link below:</p>
                <p><a href='{fullUrl}' target='_blank'>{actionName} (CCMS)</a></p>
                <p>Thank you.</p>";
        }

        public static string BuildT1CEmailBody(Models.Form tt, string actionName, string fullUrl, User user)
        {
            // Build Actual Amount row conditionally
            string actualAmountRow = string.Empty;
            if (tt.ActualAmount.HasValue)
            {
                actualAmountRow = $@"
            <tr style='background-color: #fff3cd;'>
                <td style='border: 1px solid #ddd; padding: 8px; font-weight: bold; color: #856404;'>Actual Amount (Please Note)</td>
                <td style='border: 1px solid #ddd; padding: 8px; font-weight: bold; color: #856404;'>{tt.ActualAmount.Value.ToString("N2")}</td>
            </tr>";
            }

            return $@"
                <p>Dear {user.Name},</p>
                <p>As a CCMS user with the role <strong>{user.CCMSRoleCode}</strong>, please view the details below:</p>

                <table style='border-collapse: collapse; width: 100%; font-family: Arial, sans-serif; font-size: 14px;'>
                    <tr style='background-color: #f2f2f2;'>
                        <th style='border: 1px solid #ddd; padding: 8px;'>Field</th>
                        <th style='border: 1px solid #ddd; padding: 8px;'>Details</th>
                    </tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Biz Area Code</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.BizAreaCode}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Biz Area Name</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.BizAreaName}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Reference</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.Ref}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Application Date</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.Date?.ToString("dd/MM/yyyy")}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Estimated Amount</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.Amount?.ToString("N2")}</td></tr>
                    {actualAmountRow}
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Procurement Type</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.ProcurementType}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px; vertical-align: top;'>Justification of Need</td><td style='border: 1px solid #ddd; padding: 12px; white-space: pre-wrap;'>{tt.JustificationOfNeed}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px; vertical-align: top;'>Remarks</td><td style='border: 1px solid #ddd; padding: 12px; white-space: pre-wrap;'>{tt.Remarks}</td></tr>
                </table>

                <p style='margin-top: 20px;'>Please view the <strong>{actionName}</strong> by clicking the link below:</p>
                <p><a href='{fullUrl}' target='_blank'>{actionName} (CCMS)</a></p>
                <p>Thank you.</p>";
        }

        public static string BuildT2EmailBody(Models.FormsProcurement tt, string actionName, string fullUrl, string ProcurementType, User user)
        {
            // Build Actual Amount row conditionally
            string actualAmountRow = string.Empty;
            if (tt.ActualAmount.HasValue)
            {
                actualAmountRow = $@"
            <tr style='background-color: #fff3cd;'>
                <td style='border: 1px solid #ddd; padding: 8px; font-weight: bold; color: #856404;'>Actual Amount (Please Note)</td>
                <td style='border: 1px solid #ddd; padding: 8px; font-weight: bold; color: #856404;'>{tt.ActualAmount.Value.ToString("N2")}</td>
            </tr>";
            }

            return $@"
                <p>Dear {user.Name},</p>
                <p>As a CCMS user with the role <strong>{user.CCMSRoleCode}</strong>, please view the details below:</p>

                <table style='border-collapse: collapse; width: 100%; font-family: Arial, sans-serif; font-size: 14px;'>
                    <tr style='background-color: #f2f2f2;'>
                        <th style='border: 1px solid #ddd; padding: 8px;'>Field</th>
                        <th style='border: 1px solid #ddd; padding: 8px;'>Details</th>
                    </tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Biz Area Code</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.BizAreaCode}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Biz Area Name</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.BizAreaName}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Reference</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.Ref}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Application Date</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.Date?.ToString("dd/MM/yyyy")}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Amount</td><td style='border: 1px solid #ddd; padding: 8px;'>{tt.Amount?.ToString("N2")}</td></tr>
                    {actualAmountRow}
                    <tr><td style='border: 1px solid #ddd; padding: 8px;'>Procurement Type</td><td style='border: 1px solid #ddd; padding: 8px;'>{ProcurementType}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px; vertical-align: top;'>Justification of Need</td><td style='border: 1px solid #ddd; padding: 12px; white-space: pre-wrap;'>{tt.JustificationOfNeed}</td></tr>
                    <tr><td style='border: 1px solid #ddd; padding: 8px; vertical-align: top;'>Remarks</td><td style='border: 1px solid #ddd; padding: 12px; white-space: pre-wrap;'>{tt.Remarks}</td></tr>
                </table>

                <p style='margin-top: 20px;'>Please view the <strong>{actionName}</strong> by clicking the link below:</p>
                <p><a href='{fullUrl}' target='_blank'>{actionName} (CCMS)</a></p>
                <p>Thank you.</p>";
        }
    }
}
