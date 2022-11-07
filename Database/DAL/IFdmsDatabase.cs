using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FDMS.DAL
{
    /// <summary>
    /// Interface for an object that communicates with an FDMS database
    /// </summary>
    public interface IFdmsDatabase
    {
        DALResult Connect(IPAddress ip, ushort port, string username, string password);
        DALResult Connect(string connectionString);

        DALResult Insert(TelemetryRecordDAL record);

        DALSelectResult Select(string aircraftTailNum, int n = 100);

        DALResult Disconnect();
    }


    /// <summary>
    /// Encapsulates the result of a Select operation performed by the
    /// FdmsDatabase. Its Success field indicates if the Select operation was
    /// successful, and if so then the REcords field will be populated with the
    /// retrieved records. Otherwise, if the success field is fales, then the
    /// Failure message field will contain a description of the error that
    /// occurred
    /// </summary>
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

    /// <summary>
    /// Encapsulates the result of an operation performed by the FdmsDatabase.
    /// Its Success field indicates if a FdmsDatabase operation was successful.
    /// If false, then the FailureMessage field will contain a description of
    /// the error that occurred.
    /// </summary>
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
