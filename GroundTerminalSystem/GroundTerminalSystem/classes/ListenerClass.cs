using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading;
using System.Configuration;
using System.Net.Sockets;
using System.Net.Mail;
using System.Windows.Markup;
using FDMS.DAL;

namespace GroundTerminalSystem.classes
{
    public class ListenerClass
    {
        public delegate void RecordReceivedDelegate(TelemetryRecordDAL record);
        public event RecordReceivedDelegate RecordReceivedEvent
        {
            add
            {
                lock (recordReceivedEventLock)
                {
                    _recordReceivedEvent += value;
                }
            }
            remove
            {
                lock (recordReceivedEventLock)
                {
                    _recordReceivedEvent -= value;
                }
            }
        }

        private event RecordReceivedDelegate _recordReceivedEvent;

        public string sIpAddress {private set; get; }
        public ushort Port { set; get; }

        private FdmsDatabase insertionDatabase;
        private CancellationTokenSource stopTokenSource = new CancellationTokenSource(); 
        private object recordReceivedEventLock = new object();
        

        public ListenerClass(string ipAdress, ushort tmpPort, FdmsDatabase insertionDatabase)
        {
            sIpAddress = ipAdress;
            Port = tmpPort;
            this.insertionDatabase = insertionDatabase;
        }

        public void ListenForConnection(Action<IPEndPoint> OnConnectionError)
        {
            Console.WriteLine("listening");
            CancellationToken stopToken = stopTokenSource.Token;
            IPAddress ipAddress = IPAddress.Parse(sIpAddress);
            IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, Port);

            try
            {
                using (Socket listenerSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    listenerSocket.Blocking = false; //don't block, so we can shutdown if needed
                    listenerSocket.Bind(iPEndPoint);
                    listenerSocket.Listen(1);

                    while (!stopToken.IsCancellationRequested) //listen for connections until program ends
                    {
                        try
                        {
                            Socket handler = listenerSocket.Accept();
                            if (handler != null)
                            {
                                new Thread(() => RecievePacket(handler, InvokeRecordReceivedEvent, insertionDatabase, stopTokenSource.Token))
                                    .Start();
                            }
                        }
                        catch (SocketException e)
                        {
                            if (e.SocketErrorCode == SocketError.WouldBlock) //no incoming connection, sleep
                            {
                                Thread.Sleep(3000);
                            }
                            else
                            {
                                Console.WriteLine("ERROR" + e.Message);
                            }
                        }
                    }
                }
            }
            catch(SocketException e) // failed to create, bind, or listen on socket
            {
                OnConnectionError(iPEndPoint);
                Console.WriteLine("ERROR" + e.Message);
            }

            Console.WriteLine("done");
        }

        public void Stop()
        {
            stopTokenSource.Cancel();
        }

        private void InvokeRecordReceivedEvent(TelemetryRecordDAL record)
        {
            lock(recordReceivedEventLock)
            {
                _recordReceivedEvent?.Invoke(record);
            }
        }

        private static void RecievePacket(object obj, Action<TelemetryRecordDAL> onRecordReceived, FdmsDatabase insertionDatabase, CancellationToken stopToken)
        {
            Socket tempHandler = (Socket)obj;
            tempHandler.Blocking = true;
            AircraftPacket packet = new AircraftPacket();
            byte[] bytes = new byte[1024];
            
            while (!stopToken.IsCancellationRequested) //get data from socket until program ends
            {
                try
                {
                    string data = "";
                    int bytesRec = tempHandler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    if (bytesRec == -1 || data.IndexOf(data) == -1)
                    {
                        break;
                    }

                    packet.parsePackets(data);
                    if (DeterminePacketEquality(CheckSumClac(packet.Altitude, packet.Pitch, packet.Bank), packet.Checksum))
                    {
                        onRecordReceived(packet.ConvertToTelemetryRecord());
                    }

                    data = "";
                }
                catch (SocketException ex) //error receiving data, connection probably severed, stop receiving
                {
                    Console.WriteLine("ERROR" + ex.Message);
                    break;
                }
            }

            if (tempHandler.Connected)
            {
                tempHandler.Disconnect(false);
            }

            tempHandler.Dispose();
        }

        static private int CheckSumClac(float Alt, float Pitch, float Bank)
        {
            return (int)((Alt + Pitch + Bank) / 3);
        }

        static private bool DeterminePacketEquality(float groundCalculatedCheckSum, float recievedChecksum)
        {
            return groundCalculatedCheckSum == recievedChecksum;
        }
    }
}
