using System;
using System.Collections.Generic;

namespace cms.database
{
    public interface ICMSProperties
    {
        /// <summary>
        /// Deletes the specified property
        /// </summary>
        public void DeleteProperty(string propertyName);

        /// <summary>
        /// Returns the value for the specified property. If not found saves
        /// the default value and returns the default value as a 'String'.
        /// </summary>
        public string GetProperty(string propertyName, string defaultValue);

        /// <summary>
        /// Returns the value for the specified property. If not found saves
        /// the default value and returns the default value as a 'Boolean'.
        /// </summary>
        public bool GetProperty(string propertyName, bool defaultValue);

        /// <summary>
        /// Returns the value for the specified property. If not found saves
        /// the default value and returns the default value as an 'Integer'.
        /// </summary>
        public int GetProperty(string propertyName, int defaultValue);

        /// <summary>
        /// Return a list of strings for a comma or semicolon separated property value
        /// </summary>
        public List<string> GetPropertyValueList(string propertyName, string defaultValue);

        /// <summary>
        /// Returns the value for the specified property. If not found saves
        /// the default value and returns the default value as a 'DateTime'.
        /// </summary>
        public DateTime GetPropertyTimestamp(string propertyName);

        /// <summary>
        /// Saves the provided value withe the specified property name.
        /// </summary>
        public void SaveProperty(string propertyName, string value);
    }
}
