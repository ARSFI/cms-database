using System;
using System.Data;

namespace cms.database
{
    public interface ICMSDatabase
    {
        /// <summary>
        /// Returns true if the query returns at least one row
        /// </summary>
        public bool ExistsQuery(string sql);
        /// <summary>
        /// Fills a dataset based on the provided sql query
        /// </summary>
        public DataSet FillDataSet(string sql, string tableName);
        public string FillSingleValue(string sql);
        public DateTime FillSingleValueDateTime(string sql);
        public int NonQuery(string sql);
    }
}
