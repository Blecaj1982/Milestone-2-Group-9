using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FDMS.DAL
{
    public interface IFdmsDatabase
    {
        DALResult Connect(IPAddress ip, ushort port, string username, string password);
        DALResult Connect(string connectionString);

        DALResult Insert(TelemetryRecordDAL record);

        DALSelectResult Select(string aircraftTailNum, int n = 100);

        DALResult Disconnect();
    }


    public class DALSelectResult
    {
        public bool Success;
        public string FailureMessage;
        public List<TelemetryRecordDAL> Records;

        public DALSelectResult(string failureMessage)
        {
            Success = false;
            Records = null;
            FailureMessage = failureMessage;
        }

        public DALSelectResult(List<TelemetryRecordDAL> records)
        {
            Success = true;
            Records = records;
            FailureMessage = "";
        }

    }

    public class DALResult
    {
        public bool Success;
        public string FailureMessage;

        public DALResult()
        {
            Success = true;
            FailureMessage = "";
        }

        public DALResult(string failureMessage)
        {
            Success = false;
            FailureMessage = failureMessage;
        }
    }
}
