using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GroundTerminalSystem;
using FDMS.DAL;
using System.IO;

namespace GroundTerminalSystem.classes
{
    internal class DatabaseSearch
    {
        FdmsDatabase database; 

        public DatabaseSearch(FdmsDatabase database)
        {
            this.database = database;
        }

        public List<TelemetryRecordDAL> dbaseSearch(string searchItem)
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

            return null;
        }
    }
}
