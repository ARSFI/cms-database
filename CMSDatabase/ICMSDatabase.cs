using System;
using System.Data;

namespace winlink.cms.data
{
    /// <summary>
    /// This class provides an interface to the MySQL database used by most all of the CMS modules.
    /// The class implements an automatic retry with increasing delays between attempts for
    /// TimeoutException and IOException errors.
    /// </summary>
    public interface ICMSDatabase
    {
        /// <summary>
        /// Returns 'true' if the sql query returns at least one row, otherwise 'false'.
        /// </summary>
        public bool ExistsQuery(string sql);

        /// <summary>
        /// Fills a 'Dataset' based on the provided sql query.
        /// </summary>
        public DataSet FillDataSet(string sql, string tableName = "Records");

        /// <summary>
        /// Returns a single 'String' value for the provided sql query.
        /// If the query returns zero records a ZeroRecordsException is returned.
        /// If the query results in more than one record a MultipleRecordsException is returned.
        /// </summary>
        public string FillSingleValue(string sql);

        /// <summary>
        /// Returns a single 'Bool' value for the provided sql query.
        /// If the query returns zero records a ZeroRecordsException is returned.
        /// If the query results in more than one record a MultipleRecordsException is returned.
        /// </summary>
        public bool FillSingleValueBoolean(string sql);

        /// <summary>
        /// Returns a single 'DateTime' value for the provided sql query.
        /// If the query returns zero records a ZeroRecordsException is returned.
        /// If the query results in more than one record a MultipleRecordsException is returned.
        /// </summary>
        public DateTime FillSingleValueDateTime(string sql);

        /// <summary>
        /// Returns a single 'Int' value for the provided sql query.
        /// If the query returns zero records a ZeroRecordsException is returned.
        /// If the query results in more than one record a MultipleRecordsException is returned.
        /// </summary>
        public int FillSingleValueInteger(string sql);

        /// <summary>
        /// Execute the sql query returning the number of records impacted.
        /// </summary>
        public int NonQuery(string sql);
    }
}
