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

        public bool BulkInsertDataList<T>(List<T> dataObjects, List<string> sqlColumnList)
        {
            DataTable dataTable = new System.Data.DataTable(GetTableNameFromObject<T>());
            PropertyInfo[] props = typeof(T).GetProperties();
            for (int i = 0; i < sqlColumnList.Count; i++)
            {
                DataColumnAttribute column = GetColumnNameFromSqlColumns<T>(sqlColumnList[i]);
                if (column.ColumnName == "Id")
                {
                    var idColumn = new DataColumn("Id", typeof(int));
                    idColumn.AutoIncrement = true;
                    idColumn.AutoIncrementSeed = 1;
                    idColumn.AutoIncrementStep = 1;
                    dataTable.Columns.Add(idColumn);
                }
                else
                {
                    dataTable.Columns.Add(column.ColumnName, column.ColumnType);
                }
            }

            foreach (T dataObject in dataObjects)
            {
                DataRow row = dataTable.NewRow();
                for (int x = 0; x < sqlColumnList.Count; x++)
                {
                    row[x] = typeof(T).GetProperty(sqlColumnList[x]).GetValue(dataObject);
                }
                dataTable.Rows.Add(row);
            }

            using (SqlConnection connection =
            new SqlConnection(ConnectionString))
            {
                SqlBulkCopy bulkCopy =
                    new SqlBulkCopy(connection);

                for (int z = 0; z < sqlColumnList.Count; z++)
                {
                    bulkCopy.ColumnMappings.Add(sqlColumnList[z], sqlColumnList[z]);
                }

                bulkCopy.DestinationTableName = GetTableNameFromObject<T>();
                connection.Open();

                bulkCopy.BatchSize = 2000;
                bulkCopy.NotifyAfter = 50000;
                bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(bulkCopy_SqlRowsCopied);

                bulkCopy.WriteToServer(dataTable);
                connection.Close();
            }

            return true;
        }


        #region Reflection Methods
        public string GetTableNameFromObject<T>()
        {
            Type attribType = typeof(DataTableAttribute);
            object[] arrTableAttrib = null;
            arrTableAttrib = typeof(T).GetCustomAttributes(typeof(DataTableAttribute), true);
            return ((DataTableAttribute)arrTableAttrib[0]).TableName;
        }

        public DataColumnAttribute GetColumnNameFromSqlColumns<T>(string propertyName)
        {
            DataColumnAttribute propAttr = null;
            object[] arrColumnAttributes = null;

            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                if (propertyInfo.Name == propertyName)
                {
                    arrColumnAttributes = propertyInfo.GetCustomAttributes(typeof(DataColumnAttribute), true);
                    if (arrColumnAttributes.Length > 0)
                    {
                        propAttr = (DataColumnAttribute)arrColumnAttributes[0];
                        break;
                    }
                }
            }

            return propAttr;
        }

        #endregion Reflection Methods


        #region Events
        /// <summary>
        /// Handles the SqlRowsCopied event of the bulkCopy control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Data.SqlClient.SqlRowsCopiedEventArgs"/> instance containing the event data.</param>
        void bulkCopy_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            EventHandler<SqlRowsCopiedEventArgs> teventHendleremp = RowsInserted;
            if (teventHendleremp != null)
            {
                teventHendleremp(this, e);
                Console.Write(String.Format("{0} - Rows inserted.", e.RowsCopied.ToString()));
            }
        }

        #endregion Events

    }
}
