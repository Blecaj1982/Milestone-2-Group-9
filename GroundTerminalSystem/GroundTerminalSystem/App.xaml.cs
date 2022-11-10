using FDMS.DAL;
using GroundTerminalSystem.classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GroundTerminalSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static FdmsDatabase InsertionDatabase { get; private set; }
        public static FdmsDatabase SelectionDatabase { get; private set; }
        public static ListenerClass ServerListener { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // set up database connections
            InsertionDatabase = new FdmsDatabase();
            SelectionDatabase = new FdmsDatabase();
            var insertionDbConnectResult = InsertionDatabase.Connect(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);
            var selectionDbConnectResult = SelectionDatabase.Connect(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);
            bool connectedToDatabase = insertionDbConnectResult.Success && selectionDbConnectResult.Success;

            if (!connectedToDatabase)
            {
                MessageBox.Show($"Unable to establish connection with Database.\r\n" +
                    (!insertionDbConnectResult.Success ? $"{insertionDbConnectResult.FailureMessage}\r\n" : "") +
                    (!selectionDbConnectResult.Success ? $"{selectionDbConnectResult.FailureMessage}\r\n" : "") 
                );
            }

            // create listener
            ServerListener = new ListenerClass("127.0.0.1", 8989, InsertionDatabase); 
        }
    }
}
