using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDMS.DAL
{
    public class FdmsDatabase : IFdmsDatabase
    {
        public bool Connected { get; private set; } = false;

        private SqlConnection connection = null;

        public FdmsDatabase()
        {
        }
        
        public DALResult Connect(string connectionString)
        {
            if (connection != null)
            {
                try
                {
                    connection.Close();
                } catch { }

                connection.Dispose();
                connection = null;
            }

            connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                Connected = true;
                return new DALResult();
            }
            catch (Exception ex)
            {
                connection.Dispose();
                connection = null;
                Connected = false;
                return new DALResult(ex.Message);
            }
        }

        public DALResult Disconnect()
        {
            if (connection != null)
            {
                try
                {
                    connection.Close();
                } catch { }

                connection.Dispose();
                connection = null;
                Connected = false;
                return new DALResult();
            }
            else
            {
                return new DALResult("Connection was not open.");
            }
        }

        public DALResult Insert(TelemetryRecordDAL record)
        {
            if (connection != null)
            {
                try
                {
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
                catch (Exception ex)
                {
                    return new DALResult(ex.Message);
                }
            }

            return new DALResult("Connection is not open.");
        }

        public DALSelectResult Select(string aircraftTailNum)
        {
            if (connection != null)
            {
                try
                {
                    List<TelemetryRecordDAL> records = new List<TelemetryRecordDAL>();
                    using (SqlCommand com = connection.CreateCommand())
                    {
                        com.CommandText = "SELECT * FROM Telemetry_View WHERE @tailNum = Aircraft_Tail_Num";
                        com.Parameters.AddWithValue("@tailNum", aircraftTailNum);

                        using (SqlDataReader reader = com.ExecuteReader())
                        {
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
                catch (Exception ex)
                {
                    return new DALSelectResult(ex.Message);
                }
            }

            return new DALSelectResult("Connection is not open.");
        }
    }
}
