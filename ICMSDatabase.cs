using System;
using System.Data;

namespace cms.database
{
    public interface ICMSDatabase
    {
        /// <summary>
        /// Returns 'true' if the sql query returns at least one row, otherwise 'false'
        /// </summary>
        public bool ExistsQuery(string sql);
        /// <summary>
        /// Fills a 'Dataset' based on the provided sql query
        /// </summary>
        public DataSet FillDataSet(string sql, string tableName);
        /// <summary>
        /// Returns a single 'String' value for the provided sql query
        /// </summary>
        public string FillSingleValue(string sql);
        /// <summary>
        /// Returns a single 'DateTime' value for the provided sql query
        /// </summary>
        public DateTime FillSingleValueDateTime(string sql);
        /// <summary>
        /// Execute the sql query returning the number of records impacted
        /// </summary>
        public int NonQuery(string sql);
    }
}
