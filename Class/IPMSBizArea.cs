﻿using System;
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
        public static string GetZoneCodeByBizAreaCode(string bizAreaCode)
        {
            string zoneCode = null;

            string connectionString = ConfigurationManager.ConnectionStrings["iPMSConnection"].ConnectionString;

            string query = @"
        SELECT TOP 1 W.zone_code
        FROM iPMS.dbo.BizArea B
        JOIN iPMS.dbo.Wilayah W ON B.kod_wilayah = W.kod_wilayah
        WHERE B.code = @BizAreaCode
          AND B.type = 'MILL'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@BizAreaCode", bizAreaCode);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        zoneCode = reader["zone_code"].ToString();
                    }
                }
            }

            return zoneCode;
        }

    }
}