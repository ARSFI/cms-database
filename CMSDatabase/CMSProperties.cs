using System;
using System.Collections.Generic;
using NLog;

namespace cms.database
{
    public class CMSProperties : ICMSProperties
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly ICMSDatabase _database;

        public CMSProperties(ICMSDatabase database)
        {
            _database = database;
        }

        public void DeleteProperty(string propertyName)
        {
            var sql = $"DELETE IGNORE FROM Properties WHERE Property='{propertyName}'";
            _database.NonQuery(sql);
        }

        public string GetProperty(string propertyName, string defaultValue)
        {
            try
            {
                var sql = $"SELECT Value FROM Properties WHERE Property='{propertyName}'";
                string result = _database.FillSingleValue(sql);
                //  Return result if no default specified
                if (string.IsNullOrWhiteSpace(defaultValue))
                {
                    return result;
                }

                //  Return result if a value was found 
                if (string.IsNullOrWhiteSpace(result) == false)
                {
                    return result;
                }

                // add the default value (if property not found)
                SaveProperty(propertyName, defaultValue);
                return defaultValue;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return defaultValue;
            }
        }

        public bool GetProperty(string propertyName, bool defaultValue)
        {
            var s = GetProperty(propertyName, defaultValue.ToString());
            if (s == "1" || s.ToLower() == "yes") return true;
            if (s == "0" || s.ToLower() == "no") return false;
            return Convert.ToBoolean(s);
        }

        public int GetProperty(string propertyName, int defaultValue)
        {
            return Convert.ToInt32(GetProperty(propertyName, defaultValue.ToString()));
        }

        public DateTime GetProperty(string propertyName, DateTime defaultValue)
        {
            var s = GetProperty(propertyName, ""); //get value as a string
            return DateTime.Parse(s, null, System.Globalization.DateTimeStyles.RoundtripKind);
        }

        public List<string> GetPropertyValueList(string propertyName, string defaultValue)
        {
            var list = new List<string>();
            try
            {
                var sql = $"SELECT Value FROM Properties WHERE Property='{propertyName}'";
                string result = _database.FillSingleValue(sql);

                //return empty list if no result and no default specified
                if (string.IsNullOrWhiteSpace(result) && string.IsNullOrWhiteSpace(defaultValue))
                {
                    return list;
                }

                //use the default value if the property was not found)
                if (string.IsNullOrWhiteSpace(result))
                {
                    result = defaultValue;
                    //save the default value 
                    SaveProperty(propertyName, defaultValue);
                }

                //convert to list and remove empty elements
                list = result.DelimitedStringToList(",;");
                return list;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return list;
            }
        }

        public DateTime GetPropertyTimestamp(string propertyName)
        {
            try
            {
                var sql = $"SELECT Timestamp FROM Properties WHERE Property='{propertyName}'";
                string result = _database.FillSingleValue(sql);
                if (string.IsNullOrWhiteSpace(result)) return DateTime.MinValue;
                return Convert.ToDateTime(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return DateTime.MinValue;
            }
        }

        public bool PropertyExists(string propertyName)
        {
            try
            {
                return _database.ExistsQuery($"SELECT Property FROM Properties WHERE Property = '{propertyName}'");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        public void SaveProperty(string propertyName, string value)
        {
            var ts = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm");
            var sql = $"REPLACE INTO Properties (Timestamp,Property,Value) VALUES ('{ts}','{propertyName}','{value}')";
            _database.NonQuery(sql);
        }

        public void SaveProperty(string propertyName, bool value)
        {
            SaveProperty(propertyName, value.ToString());
        }

        public void SaveProperty(string propertyName, int value)
        {
            SaveProperty(propertyName, value.ToString());
        }

        public void SaveProperty(string propertyName, DateTime value)
        {
            SaveProperty(propertyName, value.ToString("o"));
        }

    }
}
