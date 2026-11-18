using System.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;

namespace BTL_LapTrinhWeb
{
    public class DBHelper
    {
        public static SqlConnection GetConnection()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            return new SqlConnection(connStr);
        }

        public static void AddText(SqlCommand command, string name, string value)
        {
            string paramName = name.StartsWith("@") ? name : "@" + name;
            var param = command.Parameters.Add(paramName, SqlDbType.NVarChar);
            param.Value = (object)value ?? DBNull.Value;
        }
    }
}
