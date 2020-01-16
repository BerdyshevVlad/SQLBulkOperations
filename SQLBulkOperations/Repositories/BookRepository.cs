using SQLBulkOperations.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using System.Data;

namespace SQLBulkOperations.Repositories
{
    public class BookRepository : BaseRepository
    {
        public BookRepository(string connectionString) : base(connectionString)
        {

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

        public void BulkInsert(List<Book> books)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    using (SqlCommand oCommand = connection.CreateCommand())
                    {
                        oCommand.Transaction = transaction;
                        oCommand.CommandType = CommandType.Text;
                        oCommand.CommandText = "INSERT INTO [Books] ([Name], [Price]) VALUES (@name, @price);";
                        oCommand.Parameters.Add(new SqlParameter("@name", SqlDbType.NChar));
                        oCommand.Parameters.Add(new SqlParameter("@price", SqlDbType.Decimal));
                        try
                        {
                            foreach (var book in books)
                            {
                                oCommand.Parameters[0].Value = book.Name;
                                oCommand.Parameters[1].Value = book.Price;
                                if (oCommand.ExecuteNonQuery() != 1)
                                {
                                    throw new InvalidProgramException();
                                }
                            }
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
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
