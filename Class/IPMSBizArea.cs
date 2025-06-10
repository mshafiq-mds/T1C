using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace Prodata.WebForm.Class
{
    public class IPMSBizArea
    {
        public List<Models.ViewModels.IPMSBizAreaListViewModel> GetIPMSBizAreas()
        {
            var dt = new DataTable();
            var bizAreas = new List<Models.ViewModels.IPMSBizAreaListViewModel>();
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
                        sql.AppendLine("WHERE LEN(code) = '4'");
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
                bizAreas.Add(new Models.ViewModels.IPMSBizAreaListViewModel
                {
                    Code = row["code"].ToString(),
                    Name = row["description"].ToString(),
                    DisplayName = $"{row["code"].ToString()} - {row["description"].ToString()}"
                });
            }
            return bizAreas;
        }

        public string GetIPMSBizAreaNameByCode(string code)
        {
            string baName = string.Empty;

            try
            {
                using (var con = new SqlConn().GetSqlConnection("iPMSConnection"))
                {
                    StringBuilder sql = new StringBuilder();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        sql.AppendLine("SELECT description");
                        sql.AppendLine("FROM BizArea");
                        sql.AppendLine("WHERE code = @code");

                        cmd.CommandText = sql.ToString();
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@code", code);

                        con.Open();
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            baName = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return baName;
        }

        public List<string> GetBizAreaCodes(string bizAreaCode)
        {
            var bizAreaCodes = new List<string>();

            string connectionString = ConfigurationManager.ConnectionStrings["iPMSConnection"].ConnectionString;

            string query = @"
        SELECT DISTINCT B.code AS BizAreaCode
        FROM iPMS.dbo.[User] U
        JOIN iPMS.dbo.BizArea B ON B.type = 'MILL'
        JOIN iPMS.dbo.Wilayah W ON B.kod_wilayah = W.kod_wilayah
        WHERE U.BizAreaCode = @BizAreaCode
          AND W.zone_code = U.BizAreaCode";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@BizAreaCode", bizAreaCode);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bizAreaCodes.Add(reader["BizAreaCode"].ToString());
                    }
                }
            }

            return bizAreaCodes;
        }

    }
}