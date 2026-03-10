using FGV.Prodata.Web.UI;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Administration
{
    public partial class EmailLogViewer : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Optional: Add authorization checks here to ensure 
            // only Admins can view this page.
        }

        [WebMethod]
        public static object GetEmailList()
        {
            using (var db = new AppDbContext())
            {
                // Fetch the list, grabbing only what we need to display the left pane
                // Ordered by newest first
                var logs = db.EmailLog
                             .OrderByDescending(e => e.CreatedDate)
                             .Select(e => new
                             {
                                 e.LogID,
                                 e.Subject,
                                 e.RecipientEmail,
                                 e.CreatedDate
                             })
                             .ToList();

                // Format the dates and return
                return logs.Select(e => new
                {
                    e.LogID,
                    e.Subject,
                    e.RecipientEmail,
                    // Calculates "X hours ago", "X days ago", etc.
                    FormattedDate = GetTimeAgo(e.CreatedDate)
                }).ToList();
            }
        }

        [WebMethod]
        public static object GetEmailDetail(int logId)
        {
            using (var db = new AppDbContext())
            {
                var log = db.EmailLog.FirstOrDefault(e => e.LogID == logId);
                if (log == null) return null;

                return new
                {
                    log.LogID,
                    log.RecipientEmail,
                    log.Subject,
                    log.Body,
                    log.Status,
                    log.ErrorMessage,
                    ExactDate = log.CreatedDate.ToString("yyyy-MM-dd HH:mm")
                };
            }
        }

        // Helper method to format dates exactly like the Mailtrap screenshot
        private static string GetTimeAgo(DateTime date)
        {
            TimeSpan timeSince = DateTime.Now.Subtract(date);

            if (timeSince.TotalMinutes < 1) return "just now";
            if (timeSince.TotalMinutes < 60) return $"{(int)timeSince.TotalMinutes} mins ago";
            if (timeSince.TotalHours < 24) return $"{(int)timeSince.TotalHours} hours ago";
            if (timeSince.TotalDays < 30)
            {
                int days = (int)timeSince.TotalDays;
                return days == 1 ? "a day ago" : $"{days} days ago";
            }
            if (timeSince.TotalDays < 365)
            {
                int months = (int)(timeSince.TotalDays / 30);
                return months == 1 ? "a month ago" : $"{months} months ago";
            }

            int years = (int)(timeSince.TotalDays / 365);
            return years == 1 ? "a year ago" : $"{years} years ago";
        }
    }
}