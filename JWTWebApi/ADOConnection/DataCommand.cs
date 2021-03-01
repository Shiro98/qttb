using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace JWTWebApi.ADOConnection
{
    public class DataCommand
    {
        private readonly IDatabase _database;
        private readonly IDictionary<string, object> _parameters = new Dictionary<string, object>();
        private readonly string _text;
        private readonly bool _isSp = false;

        public bool IsStoredProcedure
        {
            get { return _isSp; }
        }

        public IDictionary<string, object> Parameters
        {
            get { return _parameters; }
        }

        public string Text
        {
            get { return _text; }
        }

        public DataCommand(IDatabase database, string command) :
            this(database, command, false)
        {
        }

        public DataCommand(IDatabase database, string command, bool isSP)
        {
            _database = database;
            _text = command;
            _isSp = isSP;
        }

        public IList<T> ExecuteList<T>()
        {
            return _database.ExecuteList<T>(this);
        }

        public T Execute<T>()
        {
            return _database.ExecuteObject<T>(this);
        }

        public DataCommand Parameter(string name, object value)
        {
            _parameters[name] = value;
            return this;
        }

        public void ExecuteNonQuery()
        {
            _database.ExecuteNonQuery(this);
        }

        public string ExecuteNonQuery(SqlParameter parameterOutput)
        {
            return _database.ExecuteNonQuery(this, parameterOutput);
        }
        public string ExecuteNonQueryNoOpenConnect(SqlConnection SqlConnection, SqlParameter parameterOutput)
        {
            return _database.ExecuteNonQueryNoOpenConnect(this, SqlConnection, parameterOutput);
        }
        public T ExecuteScalar<T>()
        {
            return _database.ExecuteScalar<T>(this);
        }

        public DataSet ExecuteDataset()
        {
            return _database.ExecuteDataset(this);
        }
    }
}
