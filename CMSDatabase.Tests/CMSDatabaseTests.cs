using System;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;

namespace cms.database.Tests
{
    [TestClass()]
    public class CMSDatabaseTests
    {
        static IConfiguration Configuration { get; set; }
        private static ICMSDatabase _db;

        const string Test01Name = "Test-01";
        const string Test01Value = "Test-01-value";

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
        public void ExistsQueryTest()
        {
            var p = new CMSProperties(_db);
            p.SaveProperty(Test01Name, Test01Value);
            var b = _db.ExistsQuery("SELECT Property FROM Properties LIMIT 1");
            Assert.AreEqual(b, true);
            b = _db.ExistsQuery($"SELECT Property FROM Properties WHERE Value='{Guid.NewGuid()}' LIMIT 1");
            Assert.AreEqual(b, false);
        }

        [TestMethod()]
        public void FillDataSetTest()
        {
            var p = new CMSProperties(_db);
            p.SaveProperty(Test01Name, Test01Value);
            var ds = _db.FillDataSet("SELECT * FROM Properties");
            Assert.IsTrue(ds.Tables[0].Rows.Count > 0);
        }

        [TestMethod()]
        public void GetBooleanTest()
        {
            var p = new CMSProperties(_db);
            p.SaveProperty(Test01Name, true);
            var b = _db.GetBoolean($"SELECT Value FROM Properties WHERE Property='{Test01Name}'");
            Assert.IsTrue(b);
        }

        [TestMethod()]
        public void GetDateTimeTest()
        {
            var p = new CMSProperties(_db);
            var d1 = DateTime.UtcNow;
            p.SaveProperty(Test01Name, d1);
            var d2 = _db.GetDateTime($"SELECT Value FROM Properties WHERE Property='{Test01Name}'");
            Assert.AreEqual(d1, d2);
        }

        [TestMethod()]
        public void GetIntegerTest()
        {
            var p = new CMSProperties(_db);
            p.SaveProperty(Test01Name, 42);
            var i = _db.GetInteger($"SELECT Value FROM Properties WHERE Property='{Test01Name}'");
            Assert.AreEqual(i, 42);
        }

        [TestMethod()]
        public void GetStringTest()
        {
            var p = new CMSProperties(_db);
            p.SaveProperty(Test01Name, "String Value");
            var s = _db.GetString($"SELECT Value FROM Properties WHERE Property='{Test01Name}'");
            Assert.AreEqual(s, "String Value");
        }

        [TestMethod()]
        public void NonQueryTest()
        {
            _db.NonQuery($"DELETE IGNORE FROM Properties WHERE Property='{Test01Name}'");
            var b = _db.ExistsQuery($"SELECT * FROM Properties WHERE Property='{Test01Name}'");
            Assert.IsFalse(b);
        }
    }
}