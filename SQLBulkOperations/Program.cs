using SQLBulkOperations.Entities;
using SQLBulkOperations.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SQLBulkOperations
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = @"Data Source=localhost;Initial Catalog=BulkOperationsDB;Integrated Security=True";
            DbUp.DbUp.Init(connectionString);

            try
            {
                DBContext.DBContext.SetUp(connectionString);
                BookRepository bookRepository = new BookRepository(connectionString);
                bookRepository.RemoveAllRcords();
                var result1 = bookRepository.Create(new Entities.Book() { Name = "First Book", Price = 1 });
                var result2 = bookRepository.Create(new Entities.Book() { Name = "First Book2", Price = 1 });

                var listToDelete = new List<int>();
                listToDelete.Add(result1.Id);
                listToDelete.Add(result2.Id);

                bookRepository.BulkRemove(listToDelete);

                var books = FillDataList();
                bookRepository.BulkInsert(books);

            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
               
                Console.WriteLine("Конец...");
            }

            Console.Read();
        }


        public static List<Book> FillDataList()
        {
            List<Book> objects = new List<Book>();
            for (int i = 10; i < 20; i++)
            {
                Book book = new Book();
                book.Name = $"name{i}";
                book.Price = Convert.ToDecimal(i);

                objects.Add(book);
            }

            return objects;
        }
    }
}
