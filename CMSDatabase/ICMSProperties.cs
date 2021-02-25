using System;
using System.Collections.Generic;

namespace cms.database
{
    /// <summary>
    /// This class provides methods to save and retrieve properties from the CMS Properties table.
    /// Property names should start with the module name followed by a dash and the name of the
    /// property (eg. 'CMS Telnet Server - Telnet Port'). All property values are stored as
    /// strings; to be converted upon retrieval. Methods for several common types are included.
    /// </summary>
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
        /// Returns a list of strings for a comma or semicolon separated property value.
        /// (eg, 'A,1,Two,3,4,Dog, Cat' would be returned as a string list containing
        /// those items)
        /// </summary>
        public List<string> GetPropertyValueList(string propertyName, string defaultValue);

        /// <summary>
        /// Returns the timestamp of the specified property.
        /// </summary>
        public DateTime GetPropertyTimestamp(string propertyName);

        /// <summary>
        /// Saves the property using the name and value provided.
        /// </summary>
        public void SaveProperty(string propertyName, string value);
    }
}
