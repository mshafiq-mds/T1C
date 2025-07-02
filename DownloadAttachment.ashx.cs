using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prodata.WebForm
{
    /// <summary>
    /// Summary description for DownloadAttachment
    /// </summary>
    public class DownloadAttachment : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string idParam = context.Request.QueryString["id"];
            if (!Guid.TryParse(idParam, out Guid id))
            {
                context.Response.StatusCode = 400;
                context.Response.Write("Invalid ID");
                return;
            }

            using (var db = new AppDbContext())
            {
                var attachment = db.Attachments.FirstOrDefault(a => a.Id == id);

                if (attachment == null || attachment.Content == null)
                {
                    context.Response.StatusCode = 404;
                    context.Response.Write("Attachment not found");
                    return;
                }

                context.Response.Clear();
                context.Response.ContentType = attachment.ContentType;

                // Define content types that should open inline
                var inlineTypes = new[]
                {
                    "application/pdf",
                    "image/jpeg",
                    "image/png",
                    "image/gif",
                    "image/webp",
                    "image/bmp",
                    "image/svg+xml"
                };

                bool isInline = inlineTypes.Contains(attachment.ContentType);
                string disposition = isInline ? "inline" : "attachment";

                string fileName = attachment.FileName ?? "file";
                context.Response.AddHeader("Content-Disposition", $"{disposition}; filename=\"{fileName}\"");
                context.Response.OutputStream.Write(attachment.Content, 0, attachment.Content.Length);
                context.Response.End();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}