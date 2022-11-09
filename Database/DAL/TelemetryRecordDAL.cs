﻿using System;

namespace FDMS.DAL
{
    /// <summary>
    /// Simple object representing Telemetry data for an aircraft. Used for
    /// transferring Telemetry information into and out of a database. One if
    /// constructors is for use by the FdmsDatabase and another is for use by
    /// client code
    /// </summary>
    public class TelemetryRecordDAL
    {
        public string AircraftTailNum { get; set; }
        public DateTime Timestamp { get; set; } // Time that the Telemetry data was generated by the aicraft
        public DateTime EntryTimestamp { get; set; } // Time at which the Telemetry data was inserted into the FDMS database
        public float Accel_X { get; set; }
        public float Accel_Y { get; set; }
        public float Accel_Z { get; set; }
        public float Weight { get; set; }
        public float Altitude { get; set; }
        public float Pitch { get; set; }
        public float Bank { get; set; }

        public TelemetryRecordDAL() { }

        /// <summary>
        /// Client code constructor for TelemetryRecordDAL. Lacks an EntryTimestamp parameter.
        /// </summary>
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

        /// <summary>
        /// Database constructor for TelemetryRecordDAL. 
        /// </summary>
        public TelemetryRecordDAL(string aircraftTailNum, DateTime timestamp, DateTime entryTimestamp, float accel_X, float accel_Y, float accel_Z, float weight, float altitude, float pitch, float bank)
        {
            AircraftTailNum = aircraftTailNum;
            Timestamp = timestamp;
            EntryTimestamp = entryTimestamp;
            Accel_X = accel_X;
            Accel_Y = accel_Y;
            Accel_Z = accel_Z;
            Weight = weight;
            Altitude = altitude;
            Pitch = pitch;
            Bank = bank;
        }

        public override string ToString()
        {
            return $"{AircraftTailNum},{Timestamp},{Accel_X:N6},{Accel_Y:N6},{Accel_Z:N6},{Weight:N6},{Altitude:N6},{Pitch:N6},{Bank:N6}";
        }
    }
}
