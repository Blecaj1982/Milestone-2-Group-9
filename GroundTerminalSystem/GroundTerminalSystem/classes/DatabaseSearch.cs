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

namespace GroundTerminalSystem.classes
{
    internal class DatabaseSearch
    {
        public DataTable dbaseSearch(string searchItem)
        {
            //MainWindow mw = new MainWindow();
            FdmsDatabase db = new FdmsDatabase();

            var connectResult = db.Connect(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);

            if (connectResult.Success)
            {
                var selectResult = db.Select(searchItem);
                if (selectResult.Success)
                {
                    DataTable dt = new DataTable("FDMS_Server");

                    dt.Columns.Add("Aircraft Tail #", typeof(string));
                    dt.Columns.Add("Timestamp", typeof(DateTime));
                    dt.Columns.Add("Accel_X", typeof(float));
                    dt.Columns.Add("Accel_Y", typeof(float));
                    dt.Columns.Add("Accel_Z", typeof(float));
                    dt.Columns.Add("Weight", typeof(float));
                    dt.Columns.Add("Altitude", typeof(float));
                    dt.Columns.Add("Pitch", typeof(float));
                    dt.Columns.Add("Bank", typeof(float));

                    foreach (var record in selectResult.Records)
                    {
                        var row = dt.NewRow();
                        row.SetField("Aircraft Tail #", record.AircraftTailNum);
                        row.SetField("Timestamp", record.Timestamp);
                        row.SetField("Accel_X", record.Accel_X);
                        row.SetField("Accel_Y", record.Accel_Y);
                        row.SetField("Accel_Z", record.Accel_Z);
                        row.SetField("Weight", record.Weight);
                        row.SetField("Altitude", record.Altitude);
                        row.SetField("Pitch", record.Pitch);
                        row.SetField("Bank", record.Bank);
                        dt.Rows.Add(row);
                    }

                    return dt;
                }
                else
                {
                    MessageBox.Show(selectResult.FailureMessage);
                }
            }
            else
            {
                MessageBox.Show(connectResult.FailureMessage);
            }

            return null;
        }
    }
}
