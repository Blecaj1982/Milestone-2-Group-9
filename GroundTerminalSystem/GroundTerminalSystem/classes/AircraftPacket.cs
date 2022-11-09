using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroundTerminalSystem.classes
{
    internal class AircraftPacket
    {
        public string AircraftTailNum { set; get; }
        public DateTime Timestamp { set; get; }
        public float Accel_X { set; get; }
        public float Accel_Y { set; get; }
        public float Accel_Z { set; get; }
        public float Weight { set; get; }
        public float Altitude {set;get;} 
        public float Pitch { set; get; }
        public float Bank { set; get; }         
        public float Checksum { set; get; }
        public int PacketNum { set; get; }

        

        // Returns if the parse was successful or not
        public bool parsePackets(string packetToBeParsed)
        {
            bool retCode = false;
            string[] separatedFlightData = null;
            char[] unwantedChar = { ',' };
            
            separatedFlightData = packetToBeParsed.Split(unwantedChar, StringSplitOptions.RemoveEmptyEntries);
            PacketBulder(separatedFlightData);
            return retCode;
        }

        public void PacketBulder(string[] FlightInfo)
        {
            float tempParse = 0;

            AircraftTailNum = FlightInfo[0];

            PacketNum = int.Parse(FlightInfo[1]);

            Timestamp = DateTime.Parse(FlightInfo[2].Replace('_', '-'));

            float.TryParse(FlightInfo[3], out tempParse);
            Accel_X = tempParse;

            float.TryParse(FlightInfo[4], out tempParse);
            Accel_Y = tempParse;

            float.TryParse(FlightInfo[5], out tempParse);
            Accel_Z = tempParse;

            float.TryParse(FlightInfo[6], out tempParse);
            Weight = tempParse;

            float.TryParse(FlightInfo[7], out tempParse);
            Altitude = tempParse;

            float.TryParse(FlightInfo[8], out tempParse);
            Pitch = tempParse;

            float.TryParse(FlightInfo[9], out tempParse);
            Bank = tempParse;

            float.TryParse(FlightInfo[10], out tempParse);
            Checksum = tempParse;
        }

    }     
}
