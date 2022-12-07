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
        DalResult Connect(IPAddress ip, ushort port, string username, string password);
        DalResult Connect(string connectionString);

        DalResult Insert(TelemetryRecordDal record);

        DalSelectResult Select(string aircraftTailNum);

        DalResult Disconnect();
    }


    /// <summary>
    /// Encapsulates the result of a Select operation performed by the
    /// FdmsDatabase. Its Success field indicates if the Select operation was
    /// successful, and if so then the REcords field will be populated with the
    /// retrieved records. Otherwise, if the success field is fales, then the
    /// Failure message field will contain a description of the error that
    /// occurred
    /// </summary>
    public class DalSelectResult
    {
        private bool success;
        private string failureMessage;
        private List<TelemetryRecordDal> records;

        public bool Success { get => success; set => success = value; }
        public string FailureMessage { get => failureMessage; set => failureMessage = value; }
        public List<TelemetryRecordDal> Records { get => records; set => records = value; }

        public DalSelectResult(string failureMessage)
        {
            Success = false;
            Records = null;
            FailureMessage = failureMessage;
        }

        public DalSelectResult(List<TelemetryRecordDal> records)
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
    public class DalResult
    {
        private bool success;
        private string failureMessage;

        public bool Success { get => success; set => success = value; }
        public string FailureMessage { get => failureMessage; set => failureMessage = value; }

        public DalResult()
        {
            Success = true;
            FailureMessage = "";
        }

        public DalResult(string failureMessage)
        {
            Success = false;
            FailureMessage = failureMessage;
        }
    }
}
