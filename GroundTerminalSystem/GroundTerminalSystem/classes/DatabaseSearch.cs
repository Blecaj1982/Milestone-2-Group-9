using System.Collections.Generic;
using System.Windows;
using System.IO;

using FDMS.DAL;

namespace GroundTerminalSystem.classes
{
    internal static class DatabaseSearch
    {
        public static List<TelemetryRecordDAL> dbaseSearch(FdmsDatabase database, string searchItem)
        {
            var selectResult = database.Select(searchItem);

            if (selectResult.Success)
            {
                using (StreamWriter logTele = new StreamWriter("TelemetryLog.txt", true))
                {
                    foreach (var record in selectResult.Records)
                    {
                        logTele.WriteLine(record);
                    }
                }
                return selectResult.Records;
            }
            else
            {
                MessageBox.Show(selectResult.FailureMessage);
            }

            return new List<TelemetryRecordDAL>();
        }
    }
}
