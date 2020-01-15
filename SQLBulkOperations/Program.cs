using SQLBulkOperations.DataAttributes;
using SQLBulkOperations.Entities;
using SQLBulkOperations.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
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
                var result1 = bookRepository.Create(new Entities.Book() { Name = "First Book", Price = 1 });
                var result2 = bookRepository.Create(new Entities.Book() { Name = "First Book2", Price = 1 });

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
                M m = new M();
                m.Process(connectionString);
            }

            Console.Read();
        }

















    }


    public class M
    {
        public void Process(string connectionString)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = connectionString;
            SqlCommand cmd = new SqlCommand("SELECT Id, Name, Price FROM Books", sqlConnection);
            // Fill the DataObject list.
            try
            {
                List<Book> myList = FillDataObjectList();
                List<String> columnList = new List<string>();
                columnList.Add("Id");
                columnList.Add("Name");
                columnList.Add("Price");
                // Execute the bulk insert
                BulkInsertDataObjectList<Book>(myList, columnList);
            }
            catch (Exception ex)
            {
                Console.Write(String.Format("Error Occurred:  {0}", ex.Message));
            }
        }
        public static List<Book> FillDataObjectList()
        {
            List<Book> myList = new List<Book>();
            for (int i = 10; i < 20; i++)
            {
                Book book = new Book();
                //book.Id = i;
                book.Name = $"name{i}";
                book.Price = Convert.ToDecimal(i);

                myList.Add(book);
            }

            return myList;
        }

        public Boolean BulkInsertDataObjectList<T>(List<T> myDataObjectList, List<String> propertyList)
        {
            System.Data.DataTable dt = new System.Data.DataTable(GetTableNameFromObject<T>());
            // Create the datatable from the PandaPriceListEntry object.  This allows changes
            // to be made in the data object and flow to this controller.
            PropertyInfo[] props = typeof(T).GetProperties();
            //for (Int32 i = 0; i == propertyList.Count - 1; i++)
            //{
            //    DataColumnAttribute col = GetColumnNameFromProperty<T>(propertyList[i]);
            //    dt.Columns.Add(col.ColumnName, col.ColumnType);
            //}

            // Convert the list to a datatable.
            //foreach (T rec in myDataObjectList)
            //{
            //    DataRow row = dt.NewRow();
            //    for (int x = 0; x == propertyList.Count - 1; x++)
            //    {
            //        row[x] = typeof(T).GetProperty(propertyList[x]).GetValue(rec);
            //    }
            //    dt.Rows.Add(row);
            //}

            var connectionString = @"Data Source=localhost;Initial Catalog=BulkOperationsDB;Integrated Security=True";
            // Bulk Insert the datatable to the database.
            using (SqlConnection connection =
            new SqlConnection(connectionString))
            {
                // make sure to enable triggers
                // more on triggers in next post
                SqlBulkCopy bulkCopy =
                    new SqlBulkCopy(connection);

                // Set up the bulk copy mappings
                //for (Int32 z = 0; z == propertyList.Count - 1; z++)
                //{
                //    bulkCopy.ColumnMappings.Add(z, z + 1);
                //}

                //Set the destination table name
                //bulkCopy.DestinationTableName = GetTableNameFromObject<Book>();
                bulkCopy.DestinationTableName = "Books";
                connection.Open();

                //write the data in the dataTable
                bulkCopy.BatchSize = 2000;
                bulkCopy.NotifyAfter = 50000;
                bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(bulkCopy_SqlRowsCopied);

                System.Data.DataTable dt2 = new System.Data.DataTable("Books");
                var t = new DataColumn("Id", typeof(Int32));
                t.AutoIncrement = true;
                t.AutoIncrementSeed = 1;
                t.AutoIncrementStep = 1;
                dt2.Columns.Add(t);
                dt2.Columns.Add(new DataColumn("Name", typeof(string)));
                dt2.Columns.Add(new DataColumn("Price", typeof(decimal)));

                DataRow row = dt2.NewRow();
                row["Id"] = Convert.ToInt32(0);
                row["Name"] = "N";
                row["Price"] = Convert.ToDecimal(5.0);
                dt2.Rows.Add(row);
                DataRow row2 = dt2.NewRow();
                row2["Id"] = Convert.ToInt32(0);
                row2["Name"] = "N2";
                row2["Price"] = Convert.ToDecimal(55.0);
                dt2.Rows.Add(row2);

                bulkCopy.ColumnMappings.Add("Id", "Id");
                bulkCopy.ColumnMappings.Add("Name", "Name");
                bulkCopy.ColumnMappings.Add("Price", "Price");

                bulkCopy.WriteToServer(dt2);
                connection.Close();
            }

            return true;
        }

        public static String GetTableNameFromObject<T>()
        {
            Type attribType = typeof(DataTableAttribute);
            object[] arrTableAttrib = null;
            arrTableAttrib = typeof(T).GetCustomAttributes(typeof(DataTableAttribute), true);
            return ((DataTableAttribute)arrTableAttrib[0]).TableName;
        }

        public static DataColumnAttribute GetColumnNameFromProperty<T>(String propertyName)
        {
            DataColumnAttribute propAttr = null;
            object[] arrColumnAttributes = null;

            foreach (PropertyInfo pi in typeof(T).GetProperties())
            {
                if (pi.Name == propertyName)
                {
                    arrColumnAttributes = pi.GetCustomAttributes(typeof(DataColumnAttribute), true);
                    if (arrColumnAttributes.Length > 0)
                    {
                        propAttr = (DataColumnAttribute)arrColumnAttributes[0];
                        break;
                    }
                }
            }

            return propAttr;
        }

        public event EventHandler<SqlRowsCopiedEventArgs> RowsInserted;

        /// <summary>
		/// Handles the SqlRowsCopied event of the bulkCopy control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Data.SqlClient.SqlRowsCopiedEventArgs"/> instance containing the event data.</param>
		void bulkCopy_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            EventHandler<SqlRowsCopiedEventArgs> temp = RowsInserted;
            if (temp != null)
            {
                temp(this, e);
                Console.Write(String.Format("{0} - Rows inserted.", e.RowsCopied.ToString()));
            }
        }
    }
}
