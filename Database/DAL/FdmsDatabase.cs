using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Net;

namespace FDMS.DAL
{
    /// <summary>
    /// Communicates with an FDMS database. Allows for connecting,
    /// disconnecting, inserting and selecting records.
    /// </summary>
    public class FdmsDatabase : IFdmsDatabase
    {
        /// <summary>
        /// Indicates whether the object is connected to an FDMS database as of
        /// its last operation
        /// </summary>
        public bool Connected {
            get => connection == null ? false : connection.State == System.Data.ConnectionState.Open;
        }

        /// <summary>
        /// Actual connection for FDMS database
        /// </summary>
        private SqlConnection connection = null;

        /// <summary>
        /// Connects to an FDMS database using the provided network address and
        /// user credentials
        /// </summary>
        /// <param name="ip">
        /// IP of an SQL server running an FDMS database
        /// </param>
        /// <param name="port">
        /// Port of an SQL server running an FDMS database
        /// </param>
        /// <param name="username">
        /// Username to login to database under
        /// </param>
        /// <param name="password">
        /// Password for username
        /// </param>
        /// <returns>
        /// Indicates if the connection was successful, otherwise contains an
        /// error message indicating what problem occurred
        /// </returns>
        public DALResult Connect(IPAddress ip, ushort port, string username, string password)
        {
            return Connect(
                $"Data Source={ip},{port};" +
                $"Database=FDMS_Server;" +
                $"User ID={username};" +
                $"Password={password};"
            );
        }
        
        /// <summary>
        /// Connects to an FDMS database using the provided connection string
        /// </summary>
        /// <param name="connectionString">
        /// Connection string for connectin to an FDMS database
        /// </param>
        /// <returns>
        /// Indicates if the connection was successful, otherwise contains an
        /// error message indicating what problem occurred
        /// </returns>
        public DALResult Connect(string connectionString)
        {
            CloseConnectionIfOpen();

            connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                return new DALResult();
            }
            catch (Exception ex) // could not open connection
            {
                connection.Dispose();
                connection = null;
                return new DALResult(ex.Message);
            }
        }

        /// <summary>
        /// Disconnects from the connected FDMS database
        /// </summary>
        /// <returns>
        /// Indicates if the disconnecting was successful, otherwise contains an
        /// error message indicating what problem occurred
        /// </returns>
        public DALResult Disconnect()
        {
            if (CloseConnectionIfOpen())
            {
                return new DALResult();
            }
            else //connection not open
            {
                return new DALResult("Connection was not open.");
            }
        }

        /// <summary>
        /// Inserts a TelemetryRecordDAL into an FDMS database.
        /// </summary>
        /// <param name="record">
        /// Record to be inserted into an FDMS database
        /// </param>
        /// <returns>
        /// Indicates if the record was successfully inserted into an FDMS
        /// database, otherwise contains an error message indicating what
        /// problem occurred
        /// </returns>
        public DALResult Insert(TelemetryRecordDAL record)
        {
            if (connection != null)
            {
                try
                {
                    // Create and perform insert command
                    using (SqlCommand com = connection.CreateCommand())
                    {
                        com.CommandText = "InsertTelemetry";
                        com.CommandType = System.Data.CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@Aircraft_Tail_Num", record.AircraftTailNum);
                        com.Parameters.AddWithValue("@Timestamp", record.Timestamp);
                        com.Parameters.AddWithValue("@Accel_X", record.Accel_X);
                        com.Parameters.AddWithValue("@Accel_Y", record.Accel_Y);
                        com.Parameters.AddWithValue("@Accel_Z", record.Accel_Z);
                        com.Parameters.AddWithValue("@Weight", record.Weight);
                        com.Parameters.AddWithValue("@Altitude", record.Altitude);
                        com.Parameters.AddWithValue("@Pitch", record.Pitch);
                        com.Parameters.AddWithValue("@Bank", record.Bank);

                        return com.ExecuteNonQuery() > 0 ? new DALResult() : new DALResult("No rows affected.");
                    }
                }
                catch (Exception ex) //command failed
                {
                    return new DALResult(ex.Message);
                }
            }

            return new DALResult("Connection is not open.");
        }

        /// <summary>
        /// Selects <=n of the most recent records from the FDMS database with
        /// the given Aircraft Tail Number
        /// </summary>
        /// <param name="aircraftTailNum">
        /// Identifies which aircraft to retrieve the records of
        /// </param>
        /// <param name="n">
        /// Max amount of records to return
        /// </param>
        /// <returns>
        /// Indicates if the select operation was successful and contains a
        /// list of the retrieved records, otherwise inidicates failure and
        /// provides an error message
        /// </returns>
        public DALSelectResult Select(string aircraftTailNum, int n = 100)
        {
            if (connection != null)
            {
                try
                {
                    List<TelemetryRecordDAL> records = new List<TelemetryRecordDAL>();
                    using (SqlCommand com = connection.CreateCommand())
                    {
                        // create and execute command to select records from database
                        // NEED TO Interpolate parameter n into command string, it will not work as a parameter
                        com.CommandText =
                            $"SELECT TOP {n} * FROM Telemetry_View WHERE (@tailNum) = Aircraft_Tail_Num ORDER BY Telemetry_ID Desc";
                        com.Parameters.AddWithValue("@tailNum", aircraftTailNum);

                        using (SqlDataReader reader = com.ExecuteReader())
                        {
                            /*
                             * iterate through returned data, convert to
                             * TelemetryalRecordDALs, and add to a list to
                             * return
                             */
                            while(reader.Read())
                            {
                                TelemetryRecordDAL record = new TelemetryRecordDAL(
                                    reader.GetString(1),
                                    reader.GetDateTime(2),
                                    reader.GetDateTime(3),
                                    (float)reader.GetDouble(4),
                                    (float)reader.GetDouble(5),
                                    (float)reader.GetDouble(6),
                                    (float)reader.GetDouble(7),
                                    (float)reader.GetDouble(8),
                                    (float)reader.GetDouble(9),
                                    (float)reader.GetDouble(10)
                                );

                                records.Add(record);
                            }
                        }
                    }

                    return new DALSelectResult(records);
                }
                catch (Exception ex) // command failed
                {
                    return new DALSelectResult(ex.Message);
                }
            }

            return new DALSelectResult("Connection is not open.");
        }

        /// <summary>
        /// Closes the SQLConnection to an FDMS database, if it is connected
        /// </summary>
        /// <returns>
        /// Indicates if the connection was open prior to attempting to close
        /// it
        /// </returns>
        private bool CloseConnectionIfOpen()
        {
            if (connection != null)
            {
                try
                {
                    connection.Close();
                }
                catch { }

                connection.Dispose();
                connection = null;
                return true;
            }

            return false;
        }

    }
}
