using MySql.Data.MySqlClient;

namespace cms.database
{
    /// <summary>
    /// Configuration settings - populate an instance of this class and
    /// pass it to the CMSDatabase constructor.
    /// </summary>
    public class CMSDatabaseConfiguration
    {
        /// <summary>
        /// The database connection string
        /// </summary>
        public MySqlConnectionStringBuilder Connection { get; set; } 
    }
}
