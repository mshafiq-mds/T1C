using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Prodata.WebForm
{
    /// <summary>
    /// Summary description for DocumentHandler
    /// </summary>
    public class DocumentHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string idStr = context.Request.QueryString["id"];
                string tablename = context.Request.QueryString["module"];

                if (!Guid.TryParse(idStr, out Guid fileId))
                {
                    context.Response.StatusCode = 400; // Bad Request
                    context.Response.Write("Invalid file ID.");
                    return;
                }

                string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string sql = "SELECT FileName, ContentType, FileData FROM " + tablename + " WHERE Id = @Id"; 


                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", fileId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string fileName = reader["FileName"].ToString();
                                string contentType = reader["ContentType"].ToString();
                                byte[] fileData = (byte[])reader["FileData"];

                                context.Response.Clear();
                                context.Response.ContentType = contentType;
                                context.Response.AddHeader("Content-Disposition", "inline; filename=" + fileName);
                                context.Response.OutputStream.Write(fileData, 0, fileData.Length);
                                context.Response.Flush();
                            }
                            else
                            {
                                context.Response.StatusCode = 404;
                                context.Response.Write("File not found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.Write("Server error: " + ex.Message);
            }
        }

        public bool IsReusable => false;
    }
}