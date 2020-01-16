using SQLBulkOperations.DataAttributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace SQLBulkOperations.Repositories
{
    public class BaseRepository
    {

        public event EventHandler<SqlRowsCopiedEventArgs> RowsInserted;
        protected string ConnectionString  { get; set; }

        public BaseRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }
}
