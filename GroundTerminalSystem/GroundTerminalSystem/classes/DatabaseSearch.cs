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

namespace GroundTerminalSystem.classes
{
    internal class DatabaseSearch
    {
        public DataTable dbaseSearch(string searchItem)
        {
            //MainWindow mw = new MainWindow();

            try
            {
                using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["cn"].ConnectionString))
                {
                    if (cn.State==System.Data.ConnectionState.Closed)
                        cn.Open();
                    using (DataTable dt = new DataTable("FDMS_Server"))
                    {
                        using (SqlCommand cmd = new SqlCommand("Select *from FDMS_Server where Aircraft_Tail_Num=@Aircraft_Tail_Num", cn))
                        {
                            cmd.Parameters.AddWithValue("Aircraft_Tail_Num", searchItem);
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            adapter.Fill(dt);
                            return dt;
                            //mw.DataShow.ItemsSource = dt.DefaultView;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }
    }
}
