using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace Prodata.WebForm.Class
{
    public class Mill
    {
        public List<Models.ViewModels.MillListViewModel> GetMills()
        {
            var dt = new DataTable();
            var mills = new List<Models.ViewModels.MillListViewModel>();

            try
            {
                using (var con = new SqlConn().GetSqlConnection("iPMSConnection"))
                {
                    StringBuilder sql = new StringBuilder();
                    using (SqlCommand cmd = new SqlCommand())
                    using (SqlDataAdapter da = new SqlDataAdapter())
                    {
                        sql.AppendLine("SELECT code, description");
                        sql.AppendLine("FROM BizArea");
                        sql.AppendLine("WHERE type = 'MILL'");
                        sql.AppendLine("ORDER BY code");

                        cmd.CommandText = sql.ToString();
                        cmd.Connection = con;

                        da.SelectCommand = cmd;

                        con.Open();
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            foreach (DataRow row in dt.Rows)
            {
                mills.Add(new Models.ViewModels.MillListViewModel
                {
                    Code = row["code"].ToString(),
                    Name = row["description"].ToString(),
                    DisplayName = $"{row["code"].ToString()} - {row["description"].ToString()}"
                });
            }

            return mills;
        }

        public string GetMillNameByCode(string code)
        {
            string millName = null;

            try
            {
                using (var con = new SqlConn().GetSqlConnection("iPMSConnection"))
                {
                    StringBuilder sql = new StringBuilder();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        sql.AppendLine("SELECT description");
                        sql.AppendLine("FROM BizArea");
                        sql.AppendLine("WHERE type = 'MILL' AND code = @code");

                        cmd.CommandText = sql.ToString();
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@code", code);

                        con.Open();
                        var result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            millName = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return millName;
        }
    }
}