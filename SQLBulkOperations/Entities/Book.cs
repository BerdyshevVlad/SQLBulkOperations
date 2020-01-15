using SQLBulkOperations.DataAttributes;
using System;

namespace SQLBulkOperations.Entities
{
    [DataTable("Books")]
    public class Book
    {
        [DataColumn("Id", typeof(Int32))]
        public int Id { get; set; }
        [DataColumn("Name", typeof(string))]
        public string Name { get; set; }
        [DataColumn("Price", typeof(decimal))]
        public decimal Price { get; set; }
    }
}
