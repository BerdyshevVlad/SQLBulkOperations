using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLBulkOperations.DataAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DataTableAttribute : Attribute
    {
        private string _tableName;


        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        private DataTableAttribute()
        {
            _tableName = string.Empty;
        }
        /// <summary>
        /// Maps a class to a Database Table or XML Element
        /// </summary>
        /// <param name="containerName">Name of the Table or XML Parant Element to map</param>
        public DataTableAttribute(string containerName)
            : this()
        {
            _tableName = containerName;
        }
    }
}
