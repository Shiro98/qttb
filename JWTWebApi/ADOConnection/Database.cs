using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace JWTWebApi.ADOConnection
{
    public class Database : IDatabase, IDisposable
    {
        #region Implementation of IDisposable

        public void Dispose()
        {
        }

        #endregion

        private string _connectionString;

        public SqlConnection SqlConn;

        public Database()
        {
        }

        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    _connectionString = System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"];
                }
                return _connectionString;
            }
        }

        #region Implementation of IDatabase

        public DataCommand NewCommand(string command)
        {
            return new DataCommand(this, command);
        }

        public DataCommand NewSpCommand(string command)
        {
            return new DataCommand(this, command, true);
        }

        public T ExecuteObject<T>(DataCommand command)
        {
            using (var connection = GetSqlConnection())
            {
                var sqlCommand = CreateSqlCommand(command, connection);
                connection.Open();

                using (var reader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (reader != null && reader.Read())
                    {
                        return ReadObject<T>(reader);
                    }
                    return default(T);
                }
            }
        }

        private static SqlCommand CreateSqlCommand(DataCommand command, SqlConnection connection)
        {
            var sqlCommand = new SqlCommand(command.Text, connection);
            if (command.IsStoredProcedure) sqlCommand.CommandType = CommandType.StoredProcedure;

            foreach (var pair in command.Parameters)
            {
                sqlCommand.Parameters.Add(pair.Key, pair.Value);
            }
            return sqlCommand;
        }

        public IList<T> ExecuteList<T>(DataCommand command)
        {
            using (var connection = GetSqlConnection())
            {
                var sqlCommand = CreateSqlCommand(command, connection);
                var result = new List<T>();
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        var item = ReadObject<T>(reader);
                        result.Add(item);
                    }
                }

                return result;
            }
        }

        public IList<T> ExecuteListOutput<T>(SqlCommand sqlCommand)
        {
            using (var connection = GetSqlConnection())
            {
                var result = new List<T>();
                connection.Open();
                sqlCommand.Connection = connection;
                using (var reader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        var item = ReadObjectNew<T>(reader);
                        result.Add(item);
                    }
                }
                return result;
            }
        } 
        /// <summary>
        /// Many list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        public IList<T> ExecuteListOutputManyList<T>(SqlCommand sqlCommand)
        {
            using (var connection = GetSqlConnection())
            {
                var result = new List<T>();
                connection.Open();
                sqlCommand.Connection = connection;
                using (var reader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    do
                    {
                        var item = ReadObjectNew<T>(reader);
                        result.Add(item);
                    }
                    while (reader.NextResult());

                }
                return result;
            }
        }
        private static T ReadObjectNew<T>(IDataRecord reader)
        {
            //var result = (T)FormatterServices.GetUninitializedObject(typeof(T));
            int fCount = reader.FieldCount;
            Type m_Type = typeof(T);
            PropertyInfo[] l_Property = m_Type.GetProperties();
            object obj;

            string pName;

            obj = Activator.CreateInstance(m_Type);
            for (int i = 0; i < fCount; i++)
            {
                pName = reader.GetName(i);
                if (reader[i] != DBNull.Value &&
                    l_Property.Where(a => a.Name == pName).Select(a => a.Name).Count() > 0)
                {
                    m_Type.GetProperty(pName).SetValue(obj, reader[i], null);
                }
            }
            return (T)obj;
        }

        public T ExecuteScalar<T>(DataCommand command)
        {
            using (var connection = GetSqlConnection())
            {
                var sqlCommand = CreateSqlCommand(command, connection);
                connection.Open();

                using (var reader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (reader != null && reader.Read())
                    {
                        return (T)Convert.ChangeType(reader[0], typeof(T));
                    }
                    return default(T);
                }
            }
        }

        public DataSet ExecuteDataset(DataCommand command)
        {
            var ds = new DataSet();
            var adapter = new SqlDataAdapter();
            using (var connection = GetSqlConnection())
            {
                var sqlCommand = CreateSqlCommand(command, connection);
                connection.Open();
                adapter.SelectCommand = sqlCommand;
                adapter.Fill(ds);
            }
            return ds;
        }


        public int ExecuteNonQueryOutput(SqlCommand command)
        {
            using (var connection = GetSqlConnection())
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                command.Connection = connection;
                command.CommandTimeout = 30;
                command.Transaction = trans;
                try
                {
                    var rsl = command.ExecuteNonQuery();
                    trans.Commit();
                    return rsl;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return -99;
                }
            }
        }
        public string ExecuteNonQueryNoOpenConnect(DataCommand command, SqlConnection SqlConnection, SqlParameter parameterOutput)
        {
            string strReturn = "";
            try
            {
                var sqlCommand = CreateSqlCommand(command, SqlConnection);
                if (command.IsStoredProcedure) sqlCommand.CommandType = CommandType.StoredProcedure;
                var sqlPara = parameterOutput;
                sqlPara.Direction = ParameterDirection.Output;
                sqlCommand.Parameters.Add(sqlPara);
                SqlTransaction trans = SqlConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                sqlCommand.Connection = SqlConnection;
                sqlCommand.Transaction = trans;
                try
                {
                    sqlCommand.ExecuteNonQuery();
                    if (sqlPara.Value.Equals("-1") || sqlPara.Value.Equals("0"))
                        strReturn = "";
                    strReturn = sqlPara.Value.ToString();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
            catch (Exception exp)
            {
                return "ExecuteNonQuery error" + exp.Message;
            }
            return strReturn;
        }

        public string ExecuteNonQueryNoOpenConnect(DataCommand command, SqlConnection SqlConnection, SqlParameter parameterOutput, SqlParameter parameterOutput1, out string messages)
        {
            string strReturn = "";

            messages = "";
            try
            {
                var sqlCommand = CreateSqlCommand(command, SqlConnection);
                if (command.IsStoredProcedure) sqlCommand.CommandType = CommandType.StoredProcedure;
                var sqlPara = parameterOutput;
                sqlPara.Direction = ParameterDirection.Output;
                sqlCommand.Parameters.Add(sqlPara);
                var sqlPara1 = parameterOutput1;
                sqlPara1.Direction = ParameterDirection.Output;
                sqlCommand.Parameters.Add(sqlPara1);
                SqlTransaction trans = SqlConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                sqlCommand.Connection = SqlConnection;
                sqlCommand.Transaction = trans;
                try
                {
                    sqlCommand.ExecuteNonQuery();
                    if (sqlPara.Value.Equals("-1") || sqlPara.Value.Equals("0"))
                        strReturn = "";
                    if (sqlPara1.Value.Equals("-1") || sqlPara1.Value.Equals("0"))
                        messages = "";
                    strReturn = sqlPara.Value.ToString();
                    messages = sqlPara1.Value.ToString();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
            catch (Exception exp)
            {
                return "ExecuteNonQuery error" + exp.Message;
            }
            return strReturn;
        }
        public string ExecuteNonQuery(DataCommand command, SqlParameter parameterOutput)
        {
            string strReturn = "";

            try
            {
                using (var connection = GetSqlConnection())
                {
                    var sqlCommand = CreateSqlCommand(command, connection);
                    if (command.IsStoredProcedure) sqlCommand.CommandType = CommandType.StoredProcedure;

                    var sqlPara = parameterOutput;
                    sqlPara.Direction = ParameterDirection.Output;
                    sqlCommand.Parameters.Add(sqlPara);
                    connection.Open();
                    SqlTransaction trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    sqlCommand.Connection = connection;
                    sqlCommand.Transaction = trans;
                    try
                    {
                        sqlCommand.ExecuteNonQuery();
                        if (sqlPara.Value.Equals("-1") || sqlPara.Value.Equals("0"))
                            strReturn = "";
                        strReturn = sqlPara.Value.ToString();
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {

                        trans.Rollback();
                    }
                }
            }
            catch (Exception exp)
            {

                return "ExecuteNonQuery error" + exp.Message;
            }
            return strReturn;

        }

        public void ExecuteNonQuery(DataCommand command)
        {
            using (var connection = GetSqlConnection())
            {

                var sqlCommand = CreateSqlCommand(command, connection);
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                sqlCommand.Connection = connection;
                sqlCommand.Transaction = trans;
                try
                {
                    sqlCommand.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                }
            }
        }



        private static string GenerateInsertCommand(Type type, IList<PropertyInfo> tmp)
        {
            var sb = new StringBuilder();

            sb.Append("insert into ");
            sb.Append(GetTableName(type));

            // field
            BuildNameValuesPart(sb, tmp);

            sb.Append("select SCOPE_IDENTITY()");

            return sb.ToString();
        }

        private static void BuildNameValuesPart(StringBuilder sb, IList<PropertyInfo> tmp)
        {
            sb.Append(" (");

            for (var i = 0; i < tmp.Count; i++)
            {
                var p = tmp[i];
                sb.AppendFormat("[{0}]", p.Name);

                if (i < tmp.Count - 1) sb.Append(",");
            }
            sb.Append(")");

            // values
            sb.Append(" values (");

            for (var i = 0; i < tmp.Count; i++)
            {
                var p = tmp[i];
                sb.AppendFormat("@{0}", p.Name);

                if (i < tmp.Count - 1) sb.Append(",");
            }
            sb.Append(")\n");
        }

        private static string GetTableName(Type type)
        {
            //var name = type.Name;
            //if (name.ToLower().Equals("money")) return name;
            //return name.EndsWith("y") ? name.Substring(0, name.Length - 1) + "ies" : name + "s";
            return type.Name;
        }

        private string GenerateUpdateCommand(Type type, IList<PropertyInfo> tmp)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("update [{0}] set", GetTableName(type));

            for (var i = 0; i < tmp.Count; i++)
            {
                var p = tmp[i];
                sb.AppendFormat("[{0}] = @{0}", p.Name);
                if (i < tmp.Count - 1) sb.Append(",");
            }

            sb.AppendFormat(" where Id = @Id");

            return sb.ToString();
        }

        #endregion

        public SqlConnection GetSqlConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public void OpenConnect()
        {
            SqlConn = GetSqlConnection();
            SqlConn.Open();
        }

        public void CloseConnect()
        {
            if (SqlConn == null) return;
            if (SqlConn.State == ConnectionState.Open)
                SqlConn.Close();
        }

        private static T ReadObject<T>(IDataRecord reader)
        {
            var result = (T)FormatterServices.GetUninitializedObject(typeof(T));

            var ps = typeof(T).GetProperties();
            foreach (var p in ps)
            {
                try
                {
                    var obj = reader[p.Name];

                    if (p.PropertyType.BaseType != typeof(System.Enum))
                    {
                        obj = Convert.ChangeType(obj, p.PropertyType);
                    }

                    p.SetValue(result, obj, null);
                }
                catch (Exception ex)
                {
                }
            }
            return result;
        }




        public DataCommand NewSqlCommand(string command)
        {
            throw new NotImplementedException();
        }

        void IDatabase.ExecuteNonQueryOutput(SqlCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
