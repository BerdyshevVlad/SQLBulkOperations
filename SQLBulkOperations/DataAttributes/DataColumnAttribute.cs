using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLBulkOperations.DataAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DataColumnAttribute : Attribute
    {
        private string _columnName;
        private Type _dbType;
       

        public string ColumnName
        {
            get { return _columnName; }
            set { _columnName = value; }
        }
        public Type ColumnType
        {
            get { return _dbType; }
            set { _dbType = value; }
        }

        private DataColumnAttribute()
        {
            _columnName = string.Empty;
            _dbType = null;
        }

        /// <summary>
        /// Map a property to a database column or XML element
        /// </summary>
        /// <param name="name">Name of the column or element to map</param>
        /// <param name="type">Underlying DbType of the column</param>
        public DataColumnAttribute(string name, Type type)
            : this()
        {
            _columnName = name;
            _dbType = type;
        }
    }
}
