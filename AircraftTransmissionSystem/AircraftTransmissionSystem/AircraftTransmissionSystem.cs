/// 
/// FILE		    : AircraftTransmissionSystem.cs
/// PROJECT		    : SENG3020
/// PROGRAMMER		: SQTeam9
/// FIRST VERSION	: 11/06/2022
/// DESCRIPTION		:
///		This file is the Aircraft Transmission System, it's job is to read telemetry data, add a check sum and send it to the server over time of 1 message a second
///	

namespace AircraftTransmissionSystem
{
    using System;
    using System.Net.Sockets;
    using System.Net;
    using System.Threading;
    class AircraftTransmissionSystem
    {
        /// Aircraft Information
        const int SERVER_PORT = 8989;
        const string SERVER_ADDRESS = "127.0.0.1";
        static string AircraftName = "C-FGAX";

 
        /// Start Point
        static void Main(string[] args)
        {
            /// Checks if there is a specific aircraft carrier data to be sent, otherwise use C_FGAX as default via arguments
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("ERROR: Arguments of Aircraft name is not provided, using default: C_FGAX");

                if (SERVER_ADDRESS != null && SERVER_PORT != 0)
                {
                    SystemOnline(SERVER_PORT, SERVER_ADDRESS);
                }
            }
            else
            {
                if (args[0]=="C-FGAX" || args[0] == "C-GEFC" || args[0] == "C-QWWT")
                {
                    AircraftName = args[0];

                    if (SERVER_ADDRESS != null && SERVER_PORT != 0)
                    {
                        SystemOnline(SERVER_PORT, SERVER_ADDRESS);
                    }
                }
                else
                {
                    Console.WriteLine("ERROR: Cannot find aircraft telemtry data of name: " + args[0]);
                    return;
                }
            }
        }

        public AircraftTransmissionSystem(){}

        ///Function to calculate CheckSum
        ///Checksum = (ALT + Pitch + Bank) / 3;
        public static int CalculateCheckSum(float Alt, float Pitch, float Bank)
        {
            return (int)(Alt + Pitch + Bank) / 3;
        }

        /// <summary>
        /// This method establishes the connection and creates a thread otherwise known as a connection
        /// </summary>
        /// <param name="port">The port to listen on</param>
        /// <param name="pAddress">The IP Address to listen on</param>
        public static void SystemOnline(Int32 port, string pAddress)
        {
            TcpClient connection = null;

            try
            {
                connection = new TcpClient(pAddress, port);

                ParameterizedThreadStart ts = new ParameterizedThreadStart(Connection);
                Thread clientThread = new Thread(ts);
                clientThread.Start(connection);

            }
            catch (SocketException e)
            {
                Console.WriteLine("ERROR" + e.Message);
                if(connection != null)
                    connection.Close();
            }
        }

        /// The Connection method is a thread that sends messages to the ground control terminal
        public static void Connection(Object o)
        {
            Console.WriteLine("Thread Started");
            TcpClient client = (TcpClient)o;
            NetworkStream stream = null;

            /// Get a stream object for reading and writing
            stream = client.GetStream();

            Console.WriteLine("Connection Established");


            lock (lockObject)
            {
                string telemetryLine = null;
                int telemetryCheckSum = 0;

                Console.WriteLine("Attempting to write to server: "+AircraftName);

                try
                {
                    //Grabs log contents into a string
                    int counter = 0;
                    foreach (string line in System.IO.File.ReadLines("D:/School/Level5/Advanced Software Quality/AircraftTransmissionSystem/AircraftTransmissionSystem/AircraftTransmissionSystem/" + AircraftName + ".txt"))
                    {
                        Console.WriteLine("We can read lines");
                        string[] flightInformation = null;
                        char[] unwantedChar = { ',' };

                        telemetryLine = line.Replace(" ", "");
                        flightInformation = telemetryLine.Split(unwantedChar, StringSplitOptions.RemoveEmptyEntries);

                        telemetryCheckSum = CalculateCheckSum(float.Parse(flightInformation[5]), float.Parse(flightInformation[6]), float.Parse(flightInformation[7]));

                        telemetryLine += telemetryCheckSum.ToString() + ",";

                        Console.WriteLine(telemetryLine);


                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(telemetryLine);
                        stream.Write(msg, 0, msg.Length);

                        counter++;

                        System.Threading.Thread.Sleep(1000);
                    }
                }
                catch
                {
                    /// Unable to load telemetry data
                    System.Console.WriteLine("Error cannot read data");
                }
            }

            /// Shutdown and end connection
            stream.Close();
            client.Close();
        }

        public static bool Done = false;
        static readonly object lockObject = new object();
        static TcpListener server = null;
    }
}