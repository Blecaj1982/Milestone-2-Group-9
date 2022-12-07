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
        private readonly int AIRCRAFT_TAIL_NUM_MAX_LENGTH = 6;
        /// <summary>
        /// Indicates whether the object is connected to an FDMS database as of
        /// its last operation
        /// </summary>
        public bool Connected {
            get => connection != null && connection.State == System.Data.ConnectionState.Open;
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
        public DalResult Connect(IPAddress ip, ushort port, string username, string password)
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
        public DalResult Connect(string connectionString)
        {
            CloseConnectionIfOpen();

            connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                return new DalResult();
            }
            catch (Exception ex) // could not open connection
            {
                connection.Dispose();
                connection = null;
                return new DalResult(ex.Message);
            }
        }

        /// <summary>
        /// Disconnects from the connected FDMS database
        /// </summary>
        /// <returns>
        /// Indicates if the disconnecting was successful, otherwise contains an
        /// error message indicating what problem occurred
        /// </returns>
        public DalResult Disconnect()
        {
            if (CloseConnectionIfOpen())
            {
                return new DalResult();
            }
            else //connection not open
            {
                return new DalResult("Connection was not open.");
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
        public DalResult Insert(TelemetryRecordDal record)
        {
            if (record.AircraftTailNum.Length > AIRCRAFT_TAIL_NUM_MAX_LENGTH)
            {
                return new DalResult($"Aircraft Tail Number longer than max length {AIRCRAFT_TAIL_NUM_MAX_LENGTH}");
            }

            else if (connection != null)
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

                        return com.ExecuteNonQuery() > 0 ? new DalResult() : new DalResult("No rows affected.");
                    }
                }
                catch (Exception ex) //command failed
                {
                    return new DalResult(ex.Message);
                }
            }

            return new DalResult("Connection is not open.");
        }

        /// <summary>
        /// Selects <=n of the most recent records from the FDMS database with
        /// the given Aircraft Tail Number
        /// </summary>
        /// <param name="aircraftTailNum">
        /// Identifies which aircraft to retrieve the records of
        /// </param>
        /// 
        /// <returns>
        /// Indicates if the select operation was successful and contains a
        /// list of the retrieved records, otherwise inidicates failure and
        /// provides an error message
        /// </returns>
        public DalSelectResult Select(string aircraftTailNum)
        {
            if (connection != null)
            {
                try
                {
                    List<TelemetryRecordDal> records = new List<TelemetryRecordDal>();
                    using (SqlCommand com = connection.CreateCommand())
                    {
                        // create and execute command to select records from database
                        // NEED TO Interpolate parameter n into command string, it will not work as a parameter
                        com.CommandText =
                            $"SELECT TOP 1000 * FROM Telemetry_View WHERE (@tailNum) = Aircraft_Tail_Num ORDER BY Telemetry_ID Desc";
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
                                TelemetryRecordDal record = new TelemetryRecordDal(
                                    reader.GetString(1),
                                    reader.GetDateTime(2),
                                    reader.GetDateTime(3),
                                    new GForceParameters(
                                        (float)reader.GetDouble(4),
                                        (float)reader.GetDouble(5),
                                        (float)reader.GetDouble(6),
                                        (float)reader.GetDouble(7)
                                    ),
                                    new AltitudeParameters(
                                        (float)reader.GetDouble(8),
                                        (float)reader.GetDouble(9),
                                        (float)reader.GetDouble(10)
                                    )
                                );

                                records.Add(record);
                            }
                        }
                    }

                    return new DalSelectResult(records);
                }
                catch (Exception ex) // command failed
                {
                    return new DalSelectResult(ex.Message);
                }
            }

            return new DalSelectResult("Connection is not open.");
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                connection.Dispose();
                connection = null;
                return true;
            }

            return false;
        }

    }
}
