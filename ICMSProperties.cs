using System;
using System.Collections.Generic;

namespace cms.database
{
    public interface ICMSProperties
    {
        public void DeleteProperty(string propertyName);

        public string GetProperty(string propertyName, string defaultValue);
        
        public bool GetProperty(string propertyName, bool defaultValue);
        
        public int GetProperty(string propertyName, int defaultValue);

        /// <summary>
        /// Return a list of strings for a comma or semi-colon  separated property value
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public List<string> GetPropertyValueList(string propertyName, string defaultValue);
        
        public DateTime GetPropertyTimestamp(string propertyName);
        
        public void SaveProperty(string propertyName, string value);
    }
}
