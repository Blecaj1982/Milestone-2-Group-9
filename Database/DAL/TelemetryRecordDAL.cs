using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDMS.DAL
{
    public class TelemetryRecordDAL
    {
        public string AircraftTailNum;
        public DateTime Timestamp;
        public float Accel_X;
        public float Accel_Y;
        public float Accel_Z;
        public float Weight;
        public float Altitude;
        public float Pitch;
        public float Bank;

        public TelemetryRecordDAL() { }

        public TelemetryRecordDAL(string aircraftTailNum, DateTime timestamp, float accel_X, float accel_Y, float accel_Z, float weight, float altitude, float pitch, float bank)
        {
            AircraftTailNum = aircraftTailNum;
            Timestamp = timestamp;
            Accel_X = accel_X;
            Accel_Y = accel_Y;
            Accel_Z = accel_Z;
            Weight = weight;
            Altitude = altitude;
            Pitch = pitch;
            Bank = bank;
        }
    }
}
