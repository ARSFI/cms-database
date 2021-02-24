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
        public DataSet FillDataSet(string sql, string tableName = "Records");
        /// <summary>
        /// Returns a single 'String' value for the provided sql query
        /// </summary>
        public string GetString(string sql);
        /// <summary>
        /// Returns a single 'Bool' value for the provided sql query
        /// </summary>
        public bool GetBoolean(string sql);
        /// <summary>
        /// Returns a single 'DateTime' value for the provided sql query
        /// </summary>
        public DateTime GetDateTime(string sql);
        /// <summary>
        /// Returns a single 'Int' value for the provided sql query
        /// </summary>
        public int GetInteger(string sql);
        /// <summary>
        /// Execute the sql query returning the number of records impacted
        /// </summary>
        public int NonQuery(string sql);
    }
}
