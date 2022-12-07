using System;

using FDMS.DAL;

namespace GroundTerminalSystem.classes
{
    internal class AircraftPacket
    {
        public string AircraftTailNum { private set; get; }
        public DateTime Timestamp { private set; get; }
        public float Accel_X { private set; get; }
        public float Accel_Y { private set; get; }
        public float Accel_Z { private set; get; }
        public float Weight { private set; get; }
        public float Altitude { private set; get;} 
        public float Pitch { private set; get; }
        public float Bank { private set; get; }         
        public float Checksum { private set; get; }
        public int PacketNum { private set; get; }

        
        // Returns if the parse was successful or not
        public void parsePackets(string packetToBeParsed)
        {
            char[] unwantedChar = { ',' };
            string[] separatedFlightData = packetToBeParsed.Split(unwantedChar, StringSplitOptions.RemoveEmptyEntries);
            PacketBulder(separatedFlightData);
        }

        public TelemetryRecordDAL ConvertToTelemetryRecord()
        {
            return new TelemetryRecordDAL(
                AircraftTailNum,
                Timestamp,
                new GForceParameters(Accel_X, Accel_Y, Accel_Z, Weight),
                new AltitudeParameters(Altitude, Pitch, Bank )
            );
        }

        private void PacketBulder(string[] FlightInfo)
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
