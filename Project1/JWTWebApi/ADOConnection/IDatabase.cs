using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient; 

namespace JWTWebApi.ADOConnection
{
    public interface IDatabase
    {
        DataCommand NewCommand(string command);

        DataCommand NewSqlCommand(string command);

        T ExecuteObject<T>(DataCommand command);

        IList<T> ExecuteList<T>(DataCommand command);

        IList<T> ExecuteListOutput<T>(SqlCommand sqlCommand);

        T ExecuteScalar<T>(DataCommand command);

        DataSet ExecuteDataset(DataCommand command);

        void ExecuteNonQuery(DataCommand command);
        void ExecuteNonQueryOutput(SqlCommand command);

        string ExecuteNonQuery(DataCommand command, SqlParameter parameterOutput);
        string ExecuteNonQueryNoOpenConnect(DataCommand command, SqlConnection SqlConnection, SqlParameter parameterOutput);

    }
}
