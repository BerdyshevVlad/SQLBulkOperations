using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLBulkOperations.DBContext
{
    public static class DBContext
    {
        public static void SetUp(string connectionString)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Query("IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Books' and xtype='U')CREATE TABLE Books(Id INT IDENTITY(1,1) PRIMARY KEY, Name nvarchar(255) NOT NULL UNIQUE,Price DECIMAL NOT NULL)");
            }
        }
    }
}
