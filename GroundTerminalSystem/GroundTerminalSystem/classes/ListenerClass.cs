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

namespace GroundTerminalSystem.classes
{
    internal class ListenerClass
    {
        public string sIpAddress {private set; get; }
        public int Port { set; get; }
        AircraftPacket aircraftPacket;

        public ListenerClass(string ipAdress, int tmpPort)
        {
            sIpAddress = ipAdress;
            this.Port = tmpPort;
            aircraftPacket = new AircraftPacket();
        }

        public void ListenForConnection(Action<TelemetryRecordDAL> OnPacketRecieved)
        {
            Console.WriteLine("listening");
            IPAddress ipAddress = IPAddress.Parse(sIpAddress);
            IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, Port);

            Socket listenerSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket handler = null;

            try
            {
                listenerSocket.Bind(iPEndPoint);
                listenerSocket.Listen(1);

                while (true)
                {
                    handler = listenerSocket.Accept();
                    if(handler != null)
                    {
                        Thread packetThread = new Thread(() => RecievePacket(handler,OnPacketRecieved));
                        packetThread.Start();
                    }             
                }
            }
            catch(SocketException e)
            {
                Console.WriteLine("ERROR" + e.Message);
            }
            Console.WriteLine("done");
        }

        void RecievePacket(object obj, Action<TelemetryRecordDAL> OnPacketRecieved)
        {
            Socket tempHandler = (Socket)obj;
            byte[] bytes = new byte[1024];
            string data = null;
            

            while (true)
            {
                int bytesRec = tempHandler.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

                if (data.IndexOf(data) > -1)
                {
                    break;
                }
                // parse the packet and store the corresponding
                // information into ta packet object              
            }
            aircraftPacket.parsePackets(data);
            if (DeterminePacketEquality(CheckSumClac(aircraftPacket.Altitude, aircraftPacket.Pitch, aircraftPacket.Bank), aircraftPacket.Checksum))
            {
                PacketDatabaseInsertion(OnPacketRecieved);
            }
            data = "";
            //if (aircraftPacket.Checksum == CheckSumClac(aircraftPacket.Altitude, aircraftPacket.Pitch, aircraftPacket.Bank))
            //{
            // probably delete this line
            //}
        }

        public int CheckSumClac(float Alt, float Pitch, float Bank)
        {
            int ReturnedCalc = 0 ;
            ReturnedCalc = (int)((Alt + Pitch + Bank) / 3);
            return ReturnedCalc;
        }

        public bool DeterminePacketEquality(float groundCalculatedCheckSum, float recievedChecksum)
        {
            bool isEqual = false;
            if (groundCalculatedCheckSum == recievedChecksum)
            {
                isEqual = true; ;
            }
            return isEqual;
        }

        public void PacketDatabaseInsertion(Action<TelemetryRecordDAL> OnPacketRecieved)
        {
            // insert packet data into data base
            TelemetryRecordDAL record = new TelemetryRecordDAL(
                aircraftPacket.AircraftTailNum,
                aircraftPacket.Timestamp,
                aircraftPacket.Accel_X,
                aircraftPacket.Accel_Y,
                aircraftPacket.Accel_Z,
                aircraftPacket.Weight,
                aircraftPacket.Altitude,
                aircraftPacket.Pitch,
                aircraftPacket.Bank
            );

            db.Insert(record);
            OnPacketRecieved(record);
        }
    }


}
