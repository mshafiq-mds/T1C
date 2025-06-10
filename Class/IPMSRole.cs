using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace Prodata.WebForm.Class
{
    public class IPMSRole
    {
        public List<Models.ViewModels.IPMSRoleListViewModel> GetIPMSRoles()
        {
            var dt = new DataTable();
            var roles = new List<Models.ViewModels.IPMSRoleListViewModel>();

            try
            {
                using (var con = new SqlConn().GetSqlConnection("iPMSConnection"))
                {
                    StringBuilder sql = new StringBuilder();
                    using (SqlCommand cmd = new SqlCommand())
                    using (SqlDataAdapter da = new SqlDataAdapter())
                    {
                        sql.AppendLine("SELECT code, name");
                        sql.AppendLine("FROM Role");
                        sql.AppendLine("ORDER BY name");

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
                roles.Add(new Models.ViewModels.IPMSRoleListViewModel
                {
                    Code = row["code"].ToString(),
                    Name = row["name"].ToString(),
                    DisplayName = $"{row["code"].ToString()} - {row["name"].ToString()}"
                });
            }

            return roles;
        }

        public string GetIPMSRoleName(string code)
        {
            var dt = new DataTable();
            var name = string.Empty;
            try
            {
                using (var con = new SqlConn().GetSqlConnection("iPMSConnection"))
                {
                    StringBuilder sql = new StringBuilder();
                    using (SqlCommand cmd = new SqlCommand())
                    using (SqlDataAdapter da = new SqlDataAdapter())
                    {
                        sql.AppendLine("SELECT name");
                        sql.AppendLine("FROM Role");
                        sql.AppendLine("WHERE code = @code");
                        cmd.Parameters.AddWithValue("@code", code);
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
            if (dt.Rows.Count > 0)
                name = dt.Rows[0]["name"].ToString();
            return name;
        }
    }
}