using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Prodata.WebForm
{
	public class SqlConn
	{
		public SqlConnection GetSqlConnection(string name = "DefaultConnection")
		{
			return new SqlConnection(ConfigurationManager.ConnectionStrings[name].ToString());
		}
	}
}