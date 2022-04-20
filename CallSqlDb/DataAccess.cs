using System;
using System.Collections.Generic;
using System.Text;

namespace CallSqlDb
{
    public class DataAccess
    {
        private string _connection;

        public DataAccess(string connection)
        {
            _connection = connection;
        }

        public List<T> LoadData<T, U> (string sql, U parameters, string connectionString)
        {

        }


    }
}
