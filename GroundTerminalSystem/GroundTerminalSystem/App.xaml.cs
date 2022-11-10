using FDMS.DAL;
using GroundTerminalSystem.classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GroundTerminalSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static FdmsDatabase InsertionDatabase { get; private set; } = new FdmsDatabase();
        public static FdmsDatabase SelectionDatabase { get; private set; } = new FdmsDatabase();
        public static ListenerClass ServerListener { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Connect to database
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

            // Create and start transmission system listener on its own thread 
            ServerListener = new ListenerClass("127.0.0.1", 8989, InsertionDatabase);
            ServerListener.RecordReceivedEvent += (record) => InsertionDatabase.Insert(record);
            new Thread(
                () => {
                   ServerListener.ListenForConnection(
                        (endPoint) => { MessageBox.Show($"Unable to begin listening for Aicraft Transmissions on {endPoint.Address},{endPoint.Port}."); }
                   );
                }
            ).Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            ServerListener.Stop();
            InsertionDatabase.Disconnect();
            SelectionDatabase.Disconnect();
        }
    }
}
