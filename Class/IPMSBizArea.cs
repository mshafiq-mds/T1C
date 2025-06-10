using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace Prodata.WebForm.Class
{
    public class IPMSBizArea
    {
        public List<Models.ViewModels.IPMSBizAreaListViewModel> GetIPMSBizAreas(string type = null, string zoneCode = null)
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
                        sql.AppendLine("SELECT BA.code, BA.description");
                        sql.AppendLine("FROM BizArea BA");
                        sql.AppendLine("LEFT JOIN Wilayah W ON BA.kod_wilayah = W.kod_wilayah");

                        var conditions = new List<string>();

                        if (!string.IsNullOrEmpty(type))
                        {
                            conditions.Add("BA.type = @type");
                            cmd.Parameters.AddWithValue("@type", type);
                        }

                        if (!string.IsNullOrEmpty(zoneCode))
                        {
                            conditions.Add("W.zone_code = @zoneCode");
                            cmd.Parameters.AddWithValue("@zoneCode", zoneCode);
                        }

                        if (conditions.Any())
                        {
                            sql.AppendLine("WHERE " + string.Join(" AND ", conditions));
                        }

                        sql.AppendLine("ORDER BY BA.code");

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
                    DisplayName = $"{row["code"]} - {row["description"]}"
                });
            }

            return bizAreas;
        }


        public string GetNameByCode(string code)
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

        public string GetTypeByCode(string code)
        {
            string baType = string.Empty;
            try
            {
                using (var con = new SqlConn().GetSqlConnection("iPMSConnection"))
                {
                    StringBuilder sql = new StringBuilder();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        sql.AppendLine("SELECT type");
                        sql.AppendLine("FROM BizArea");
                        sql.AppendLine("WHERE code = @code");
                        cmd.CommandText = sql.ToString();
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@code", code);
                        con.Open();
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            baType = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return baType;
        }

        public bool IsBizAreaInWilayah(string code)
        {
            bool isInWilayah = false;
            try
            {
                using (var con = new SqlConn().GetSqlConnection("iPMSConnection"))
                {
                    StringBuilder sql = new StringBuilder();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        sql.AppendLine("SELECT COUNT(*) FROM BizArea");
                        sql.AppendLine("WHERE code = @code");
                        sql.AppendLine("AND kod_wilayah IS NOT NULL AND kod_wilayah <> ''");
                        cmd.CommandText = sql.ToString();
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@code", code);
                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        isInWilayah = count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isInWilayah;
        }

        public string GetKodWilayahByCode(string code)
        {
            string kodWilayah = string.Empty;
            try
            {
                using (var con = new SqlConn().GetSqlConnection("iPMSConnection"))
                {
                    StringBuilder sql = new StringBuilder();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        sql.AppendLine("SELECT kod_wilayah");
                        sql.AppendLine("FROM BizArea");
                        sql.AppendLine("WHERE code = @code");
                        cmd.CommandText = sql.ToString();
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@code", code);
                        con.Open();
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            kodWilayah = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return kodWilayah;
        }

        public string GetZoneCodeByKodWilayah(string kodWilayah)
        {
            string zoneCode = string.Empty;
            try
            {
                using (var con = new SqlConn().GetSqlConnection("iPMSConnection"))
                {
                    StringBuilder sql = new StringBuilder();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        sql.AppendLine("SELECT zone_code");
                        sql.AppendLine("FROM Wilayah");
                        sql.AppendLine("WHERE kod_wilayah = @kodWilayah");
                        cmd.CommandText = sql.ToString();
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@kodWilayah", kodWilayah);
                        con.Open();
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            zoneCode = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return zoneCode;
        }
    }
}