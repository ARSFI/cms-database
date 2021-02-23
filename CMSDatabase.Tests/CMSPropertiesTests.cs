using cms.database;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;

namespace cms.database.Tests
{
    [TestClass()]
    public class CMSPropertiesTests
    {
        static IConfiguration Configuration { get; set; }
        private static ICMSDatabase _db;

        const string TestProp01Name = "Test-Prop-01";
        const string TestProp03Name = "Test-Prop-03";
        const string TestProp01Value = "Test-Prop-01-value";
        const string BoolProp01Name = "Bool-Prop-01";
        private const string TestProp02Name = "Date-Prop-01";
        private const string IntProp01Name = "Int-Prop-01";

        [TestInitialize]
        public void TestInitialize()
        {
            //setup access to secrets.json file
            var builder = new ConfigurationBuilder().AddUserSecrets<CMSPropertiesTests>();
            Configuration = builder.Build();

            //create the database object
            var csb = new MySqlConnectionStringBuilder
            {
                Server = Configuration["MySqlServer"],
                Port = Convert.ToUInt32(Configuration["MySqlPort"]),
                UserID = Configuration["MySqlUser"],
                Password = Configuration["MySqlPassword"]
            };
            _db = new CMSDatabase(csb.ToString());

            //create database
            _db.NonQuery("CREATE DATABASE IF NOT EXISTS TestDatabase");
            _db.NonQuery("USE TestDatabase");

            //create properties table
            _db.NonQuery("CREATE TABLE IF NOT EXISTS Properties (Timestamp DATETIME NOT NULL, Property CHAR(150) NOT NULL, Value TEXT NOT NULL, PRIMARY KEY (Property));");
        }

        [TestMethod()]
        public void SavePropertyTest()
        {
            var p = new CMSProperties(_db);
            p.SaveProperty(TestProp01Name, TestProp01Value);
            var s = p.GetProperty(TestProp01Name, "");
            Assert.AreEqual(s, TestProp01Value);
        }

        [TestMethod()]
        public void SavePropertyDateTimeTest()
        {
            var p = new CMSProperties(_db);
            var d1 = DateTime.UtcNow;
            p.SaveProperty(TestProp02Name, d1);
            var d2 = p.GetProperty(TestProp02Name, DateTime.MinValue);
            Assert.AreEqual(d1, d2);
        }

        [TestMethod()]
        public void GetPropertyBoolTest()
        {
            var p = new CMSProperties(_db);
            p.SaveProperty(BoolProp01Name, true);
            var b = p.GetProperty(BoolProp01Name, false);
            Assert.AreEqual(b, true);
        }

        [TestMethod()]
        public void GetPropertyBoolTestUsingInt1()
        {
            var p = new CMSProperties(_db);
            p.SaveProperty(BoolProp01Name, 1);
            var b = p.GetProperty(BoolProp01Name, false);
            Assert.AreEqual(b, true);
        }

        [TestMethod()]
        public void GetPropertyBoolTestUsingInt0()
        {
            var p = new CMSProperties(_db);
            p.SaveProperty(BoolProp01Name, 0);
            var b = p.GetProperty(BoolProp01Name, true);
            Assert.AreEqual(b, false);
        }

        [TestMethod()]
        public void GetPropertyBoolTestUsingYes()
        {
            var p = new CMSProperties(_db);
            p.SaveProperty(BoolProp01Name, "yes");
            var b = p.GetProperty(BoolProp01Name, false);
            Assert.AreEqual(b, true);
        }

        [TestMethod()]
        public void GetPropertyBoolTestUsingNo()
        {
            var p = new CMSProperties(_db);
            p.SaveProperty(BoolProp01Name, "no");
            var b = p.GetProperty(BoolProp01Name, true);
            Assert.AreEqual(b, false);
        }

        [TestMethod()]
        public void GetPropertyIntTest()
        {
            var p = new CMSProperties(_db);
            p.SaveProperty(IntProp01Name, "42");
            var i = p.GetProperty(IntProp01Name, 0);
            Assert.AreEqual(i, 42);
        }

        [TestMethod()]
        public void GetPropertyStringTest()
        {
            var p = new CMSProperties(_db);
            p.SaveProperty(TestProp01Name, TestProp01Value);
            var s = p.GetProperty(TestProp01Name, "");
            Assert.AreEqual(s, TestProp01Value);
        }

        [TestMethod()]
        public void GetPropertyStringDefaultTest()
        {
            var p = new CMSProperties(_db);
            p.DeleteProperty(TestProp01Name);
            var s = p.GetProperty(TestProp01Name, TestProp01Value);
            Assert.AreEqual(s, TestProp01Value);
        }

        [TestMethod()]
        public void GetPropertyTimestampTest()
        {
            var p = new CMSProperties(_db);
            p.SaveProperty(TestProp01Name, TestProp01Value);
            var d = p.GetPropertyTimestamp(TestProp01Name);
            Assert.AreNotEqual(d, DateTime.MinValue);
        }

        [TestMethod()]
        public void GetPropertyValueListTest()
        {
            var p = new CMSProperties(_db);
            p.SaveProperty(TestProp03Name, "abc,123,xyz,789");
            var l = p.GetPropertyValueList(TestProp03Name, "1,2");
            Assert.AreEqual(l.Count, 4);
        }

        [TestMethod()]
        public void DeletePropertyTest()
        {
            var p = new CMSProperties(_db);
            p.SaveProperty(TestProp01Name, TestProp01Value);
            Assert.AreEqual(p.PropertyExists(TestProp01Name), true);

            p.DeleteProperty(TestProp01Name);
            var b = p.PropertyExists(TestProp01Name);
            Assert.AreEqual(b, false);
        }

        [TestMethod()]
        public void PropertyExistsTest()
        {
            var p = new CMSProperties(_db);
            p.SaveProperty(TestProp01Name, TestProp01Value);
            var b1 = p.PropertyExists(TestProp01Name);
            Assert.AreEqual(b1, true);
        }

        [TestMethod()]
        public void PropertyNotExistsTest()
        {
            var p = new CMSProperties(_db);
            p.DeleteProperty(TestProp01Name);
            var b2 = p.PropertyExists(TestProp01Name);
            Assert.AreEqual(b2, false);
        }

    }
}