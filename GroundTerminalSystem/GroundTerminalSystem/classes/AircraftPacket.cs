using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroundTerminalSystem.classes
{
    internal class AircraftPacket
    {
        public string Timestamp { set; get; }
        public string Accel_X { set; get; }
        public string Accel_Y { set; get; }
        public string Accel_Z { set; get; }
        public float Weight { set; get; }
        public float Altitude {set;get;} 
        public float Pitch { set; get; }
        public float Bank { set; get; }         
        public float Checksum { set; get; }

        

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
            Timestamp = FlightInfo[0];
            Accel_X = FlightInfo[1];
            Accel_Y = FlightInfo[2];
            Accel_Z = FlightInfo[3];
            float.TryParse(FlightInfo[4], out tempParse);
            Weight = tempParse;
            float.TryParse(FlightInfo[5], out tempParse);
            Altitude = tempParse;
            float.TryParse(FlightInfo[6], out tempParse);
            Pitch = tempParse;
            float.TryParse(FlightInfo[7], out tempParse);
            Bank = tempParse;
            float.TryParse(FlightInfo[8], out tempParse);
            Checksum = tempParse;
        }

    }     
}
