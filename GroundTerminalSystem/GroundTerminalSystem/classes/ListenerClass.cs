using System;
using System.Text;
using System.Threading;
using System.Net.Sockets;

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
        private readonly CancellationTokenSource stopTokenSource = new CancellationTokenSource(); 
        private readonly object recordReceivedEventLock = new object();

        public void ListenForConnection(Socket listenerSocket)
        {
            CancellationToken stopToken = stopTokenSource.Token;

            try
            {
                listenerSocket.Blocking = false; //don't block, so we can shutdown if needed
                listenerSocket.Listen(1);
                Console.WriteLine("listening");

                while (!stopToken.IsCancellationRequested) //listen for connections until program ends
                {
                    try
                    {
                        Socket handler = listenerSocket.Accept();
                        if (handler != null)
                        {
                            new Thread(() => RecievePacket(handler, InvokeRecordReceivedEvent, stopTokenSource.Token))
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
            catch(SocketException e) // failed to listen on socket
            {
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

        private static void RecievePacket(object obj, Action<TelemetryRecordDAL> onRecordReceived, CancellationToken stopToken)
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
