using SQLBulkOperations.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;

namespace SQLBulkOperations.Repositories
{
    public class BookRepository
    {
        public string ConnectionString { get; set; }
        public BookRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public List<Book> GetUsers()
        {
            List<Book> users = new List<Book>();
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                users = connection.Query<Book>("SELECT * FROM Books").ToList();
            }
            return users;
        }

        public Book Create(Book book)
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                var sqlQuery = "INSERT INTO Books(Name,Price) VALUES(@Name,@Price);SELECT CAST(SCOPE_IDENTITY() as int)";
                int? bookId = connection.Query<int>(sqlQuery, book).FirstOrDefault();
                book.Id = bookId ?? 0;
            }

            return book;
        }

        public void BulkInsert()
        {

        }

        public void BulkRemove(List<int> ids)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(int));

            foreach (var id in ids)
                dataTable.Rows.Add(Convert.ToInt32(id));

            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Execute("BulkIntRemove", new { tableName="Books",ids= dataTable }, commandType: CommandType.StoredProcedure);
            }
        }

        public void RemoveAllRcords()
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                db.Query("DELETE FROM Books;");
            }
        }
    }
}
