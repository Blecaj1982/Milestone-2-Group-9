using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDMS.DAL
{
    interface IFdmsDatabase
    {
        DALResult Connect(string connectionString);

        DALResult Insert(TelemetryRecordDAL record);

        DALSelectResult Select(string aircraftTailNum);

        DALResult Disconnect();
    }


    public class DALSelectResult : DALResult
    {
        public List<TelemetryRecordDAL> Records;
    }

    public class DALResult
    {
        public bool Success;
        public string FailureMessage;
    }
}
