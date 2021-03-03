using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using MySql.Data.MySqlClient;
using NLog;
using Polly;

#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities

namespace winlink.cms.data
{
    public class CMSDatabase : ICMSDatabase
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly string _connectionString;

        // This policy will attempt the action up to 7 times (~4 minutes) with
        // increasing delays between each attempt. Each failure is logged as a
        // 'warning'. After 7 failures an 'error' is reported.
        private readonly Policy _retryPolicy = Policy
            //Don't retry for syntax errors - they're not going away
            .Handle<MySqlException>(ex => ex.Message.IndexOf("syntax error", StringComparison.OrdinalIgnoreCase) == -1)
            .Or<TimeoutException>()
            .Or<IOException>()
            .WaitAndRetry(
                7,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, context) =>
                {
                    Log.Warn(
                        $"Received exception: '{exception.Message}'. Delaying for {timeSpan.TotalSeconds} seconds. \r\nSQL Command: {context["SQL"]}.");
                }
            );

        public CMSDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool ExistsQuery(string sql)
        {
            Debug.Assert(!string.IsNullOrEmpty(sql));
            Log.Trace($"Query: {sql}");
            bool blnResult = false;
            try
            {
                _retryPolicy.Execute(ctx =>
                {
                    using MySqlConnection connection = new MySqlConnection(_connectionString);
                    connection.Open();
                    using DataSet ds = new DataSet();
                    using MySqlCommand cmd = new MySqlCommand(sql, connection);
                    using MySqlDataAdapter adpMySql = new MySqlDataAdapter { SelectCommand = cmd };
                    adpMySql.Fill(ds);
                    blnResult = ds.Tables[0].Rows.Count > 0;
                }, new Dictionary<string, object> { { "SQL", sql } });
            }
            catch (Exception ex)
            {
                Log.Error(ex, sql);
            }

            return blnResult;
        }

        public DataSet FillDataSet(string sql, string tableName = "Records")
        {
            Debug.Assert(!string.IsNullOrEmpty(sql));
            Log.Trace($"Query: {sql}");
            DataSet ds = new DataSet();
            try
            {
                _retryPolicy.Execute(ctx =>
                {
                    using MySqlConnection connection = new MySqlConnection(_connectionString);
                    connection.Open();
                    using MySqlCommand cmd = new MySqlCommand(sql, connection);
                    using MySqlDataAdapter adpMySql = new MySqlDataAdapter { SelectCommand = cmd };
                    adpMySql.Fill(ds, tableName);
                }, new Dictionary<string, object> { { "SQL", sql } });
            }
            catch (Exception ex)
            {
                Log.Error(ex, sql);
            }

            return ds;
        }

        public string FillSingleValue(string sql)
        {
            Debug.Assert(!string.IsNullOrEmpty(sql));
            Log.Trace($"Query: {sql}");
            string result = "";
            try
            {
                _retryPolicy.Execute(ctx =>
                {
                    using MySqlConnection connection = new MySqlConnection(_connectionString);
                    connection.Open();
                    using DataSet ds = new DataSet();
                    using MySqlCommand cmd = new MySqlCommand(sql, connection);
                    using MySqlDataAdapter adpMySql = new MySqlDataAdapter { SelectCommand = cmd };
                    adpMySql.Fill(ds, "Records");
                    if (ds.Tables[0].Rows.Count == 1)
                    {
                        result = Convert.ToString(ds.Tables[0].Rows[0][0]);
                        if (string.IsNullOrWhiteSpace(result)) result = "";
                    }
                    else if (ds.Tables[0].Rows.Count == 0) throw new ZeroRecordsException();
                    else if (ds.Tables[0].Rows.Count > 0) throw new MultipleRecordsException();
                }, new Dictionary<string, object> { { "SQL", sql } });
            }
            catch (Exception ex)
            {
                Log.Error(ex, sql);
            }

            return result;
        }

        public bool FillSingleValueBoolean(string sql)
        {
            try
            {
                var s = FillSingleValue(sql);
                if (string.IsNullOrWhiteSpace(s)) return false;
                if (s == "1") return true;
                if (s == "0") return false;
                return Convert.ToBoolean(s);
            }
            catch (Exception ex)
            {
                Log.Error(ex, sql);
                return false;
            }
        }

        public DateTime FillSingleValueDateTime(string sql)
        {
            try
            {
                var s = FillSingleValue(sql);
                return DateTime.Parse(s, null, System.Globalization.DateTimeStyles.RoundtripKind);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return DateTime.MinValue;
            }
        }

        public int FillSingleValueInteger(string sql)
        {
            try
            {
                var s = FillSingleValue(sql);
                if (int.TryParse(s, out var result)) return result;
                return -1;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return -1;
            }
        }

        public int NonQuery(string sql)
        {
            int result = -1;
            try
            {
                _retryPolicy.Execute(ctx =>
                {
                    using MySqlConnection connection = new MySqlConnection(_connectionString);
                    connection.Open();
                    using MySqlCommand cmdMySqlCommand = new MySqlCommand(sql, connection);
                    result = cmdMySqlCommand.ExecuteNonQuery();
                }, new Dictionary<string, object> { { "SQL", sql } });
            }
            catch (Exception ex)
            {
                Log.Error(ex, sql);
            }
            Log.Trace($"Records Impacted: {result}, Query: {sql}");
            return result;
        }
    }
}

#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities

