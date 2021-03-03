using System;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using winlink.cms.data;

namespace cms.database.Tests
{
    [TestClass()]
    public class CMSPropertiesTests
    {
        static IConfiguration Configuration { get; set; }
        private static ICMSDatabase _database;

        const string TestPropName = "Test-Prop-01";

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
            _database = new CMSDatabase(csb.ToString());

            //create database
            _database.NonQuery("CREATE DATABASE IF NOT EXISTS TestDatabase");
            _database.NonQuery("USE TestDatabase");

            //create properties table
            _database.NonQuery("CREATE TABLE IF NOT EXISTS Properties (Timestamp DATETIME NOT NULL, Property CHAR(150) NOT NULL, Value TEXT NOT NULL, PRIMARY KEY (Property));");
        }

        [TestMethod()]
        public void SavePropertyTest()
        {
            var p = new CMSProperties(_database);
            var testValue = Guid.NewGuid().ToString();
            p.SaveProperty(TestPropName, testValue);
            var s = p.GetProperty(TestPropName, "");
            Assert.AreEqual(s, testValue);
        }

        [TestMethod()]
        public void SavePropertyDateTimeTest()
        {
            var p = new CMSProperties(_database);
            var d1 = DateTime.UtcNow;
            p.SaveProperty(TestPropName, d1);
            var d2 = p.GetProperty(TestPropName, DateTime.MinValue);
            Assert.AreEqual(d1, d2);
        }

        [TestMethod()]
        public void GetPropertyBoolTest()
        {
            var p = new CMSProperties(_database);
            p.SaveProperty(TestPropName, true);
            var b = p.GetProperty(TestPropName, false);
            Assert.AreEqual(b, true);
        }

        [TestMethod()]
        public void GetPropertyBoolTestUsingInt1()
        {
            var p = new CMSProperties(_database);
            p.SaveProperty(TestPropName, 1);
            var b = p.GetProperty(TestPropName, false);
            Assert.AreEqual(b, true);
        }

        [TestMethod()]
        public void GetPropertyBoolTestUsingInt0()
        {
            var p = new CMSProperties(_database);
            p.SaveProperty(TestPropName, 0);
            var b = p.GetProperty(TestPropName, true);
            Assert.AreEqual(b, false);
        }

        [TestMethod()]
        public void GetPropertyBoolTestUsingYes()
        {
            var p = new CMSProperties(_database);
            p.SaveProperty(TestPropName, "yes");
            var b = p.GetProperty(TestPropName, false);
            Assert.AreEqual(b, true);
        }

        [TestMethod()]
        public void GetPropertyBoolTestUsingNo()
        {
            var p = new CMSProperties(_database);
            p.SaveProperty(TestPropName, "no");
            var b = p.GetProperty(TestPropName, true);
            Assert.AreEqual(b, false);
        }

        [TestMethod()]
        public void GetPropertyIntTest()
        {
            var p = new CMSProperties(_database);
            Random rnd = new Random();
            var i1 = rnd.Next(1, 999);
            p.SaveProperty(TestPropName, i1);
            var i2 = p.GetProperty(TestPropName, 0);
            Assert.AreEqual(i1, i2);
        }

        [TestMethod()]
        public void GetPropertyStringTest()
        {
            var p = new CMSProperties(_database);
            var testValue = Guid.NewGuid().ToString();
            p.SaveProperty(TestPropName, testValue);
            var s = p.GetProperty(TestPropName, "");
            Assert.AreEqual(s, testValue);
        }

        [TestMethod()]
        public void GetPropertyStringDefaultTest()
        {
            var p = new CMSProperties(_database);
            p.DeleteProperty(TestPropName);
            var testValue = Guid.NewGuid().ToString();
            var s = p.GetProperty(TestPropName, testValue);
            Assert.AreEqual(s, testValue);
        }

        [TestMethod()]
        public void GetPropertyTimestampTest()
        {
            var p = new CMSProperties(_database);
            p.SaveProperty(TestPropName, "doesn't matter");
            var d = p.GetPropertyTimestamp(TestPropName);
            Assert.AreNotEqual(d, DateTime.MinValue);
        }

        [TestMethod()]
        public void GetPropertyValueListTest()
        {
            var p = new CMSProperties(_database);
            p.SaveProperty(TestPropName, "abc,123,xyz,789");
            var l = p.GetPropertyValueList(TestPropName, "1,2");
            Assert.AreEqual(l.Count, 4);
        }

        [TestMethod()]
        public void DeletePropertyTest()
        {
            var p = new CMSProperties(_database);
            var testValue = Guid.NewGuid().ToString();
            p.SaveProperty(TestPropName, testValue);
            Assert.IsTrue(p.PropertyExists(TestPropName));

            p.DeleteProperty(TestPropName);
            Assert.IsFalse(p.PropertyExists(TestPropName));
        }

        [TestMethod()]
        public void PropertyExistsTest()
        {
            var p = new CMSProperties(_database);
            p.SaveProperty(TestPropName, "doesn't matter");
            Assert.IsTrue(p.PropertyExists(TestPropName));
        }

        [TestMethod()]
        public void PropertyNotExistsTest()
        {
            var p = new CMSProperties(_database);
            p.DeleteProperty(TestPropName);
            Assert.IsFalse(p.PropertyExists(TestPropName));
        }

    }
}