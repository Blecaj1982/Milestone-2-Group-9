using FDMS.DAL;
using GroundTerminalSystem.classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
        public static bool ConnectedToDatabase { get; private set; } = false;
        public static bool ListeningForTransmission { get; private set; } = false;

        private Socket listenerSocket;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Connect to database
            var insertionDbConnectResult = InsertionDatabase.Connect(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);
            var selectionDbConnectResult = SelectionDatabase.Connect(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);
            ConnectedToDatabase = insertionDbConnectResult.Success && selectionDbConnectResult.Success;

            if (!ConnectedToDatabase)
            {
                MessageBox.Show($"Unable to establish connection with Database.\r\n\r\n" +
                    (!insertionDbConnectResult.Success ? $"{insertionDbConnectResult.FailureMessage}\r\n\r\n" : "") +
                    (!selectionDbConnectResult.Success ? $"{selectionDbConnectResult.FailureMessage}\r\n" : "") 
                );
            }


            ServerListener = new ListenerClass();

            // Parse transmission information then start the listener
            if (!IPAddress.TryParse(ConfigurationManager.AppSettings.Get("TransmissionIP"), out IPAddress ipAddress))
            {
                MessageBox.Show($"Unable to parse Transmission IP Address {ConfigurationManager.AppSettings.Get("TransmissionIP")}");
            }
            else if (!ushort.TryParse(ConfigurationManager.AppSettings.Get("TransmissionPort"), out ushort port))
            {
                MessageBox.Show($"Unable to parse Transmission Port {ConfigurationManager.AppSettings.Get("TransmissionIP")}");
            }
            else
            {
                try
                {
                    listenerSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    listenerSocket.Bind(new IPEndPoint(ipAddress, port));
                    ServerListener.RecordReceivedEvent += (record) => InsertionDatabase.Insert(record);
                    new Thread(() => { ServerListener.ListenForConnection(listenerSocket); })
                        .Start();
                    ListeningForTransmission = true;
                }
                catch
                {
                    MessageBox.Show($"Unable to begin listening for Aicraft Transmissions on {ipAddress},{port}.");
                    listenerSocket = null;
                }
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            ServerListener.Stop();
            InsertionDatabase.Disconnect();
            SelectionDatabase.Disconnect();

            if (listenerSocket != null)
            {
                if (listenerSocket.Connected)
                {
                    listenerSocket.Disconnect(false);
                }
                listenerSocket.Dispose();
            }
        }
    }
}
