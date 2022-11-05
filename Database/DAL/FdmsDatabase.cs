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
        private SqlConnection connection = null;
        
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
                return new DALResult();
            }
            catch (Exception ex)
            {
                connection.Dispose();
                connection = null;
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
                return new DALResult();
            }
            else
            {
                return new DALResult("Connection was not open.");
            }
        }

        public DALResult Insert(TelemetryRecordDAL record)
        {
            throw new NotImplementedException();
        }

        public DALSelectResult Select(string aircraftTailNum)
        {
            throw new NotImplementedException();
        }
    }
}
