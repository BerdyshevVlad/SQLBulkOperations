using SQLBulkOperations.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLBulkOperations
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = @"Data Source=localhost;Initial Catalog=BulkOperationsDB;Integrated Security=True";
            DbUp.DbUp.Init(connectionString);

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                DBContext.DBContext.SetUp(connectionString);
                BookRepository bookRepository = new BookRepository(connectionString);
                bookRepository.RemoveAllRcords();
                var result1 = bookRepository.Create(new Entities.Book() {Name="First Book",Price=1 });
                var result2 = bookRepository.Create(new Entities.Book() {Name="First Book2",Price=1 });

                var listToDelete = new List<int>();
                listToDelete.Add(result1.Id);
                listToDelete.Add(result2.Id);

                bookRepository.BulkRemove(listToDelete);
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // закрываем подключение
                connection.Close();
                Console.WriteLine("Подключение закрыто...");
            }

            Console.Read();
        }
    }
}
