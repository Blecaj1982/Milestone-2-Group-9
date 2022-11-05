CREATE VIEW [FDMS].[Telemetry_View]
	AS SELECT 
	Telemetry_Records.Telemetry_ID,
	Telemetry_Records.Aircraft_Tail_Num,
	Telemetry_Records.Timestamp,
	Telemetry_Records.Entry_Timestamp,
	G_Force_Parameters.Accel_X,
	G_Force_Parameters.Accel_Y,
	G_Force_Parameters.Accel_Z,
	G_Force_Parameters.Weight,
	Altitude_Parameters.Altitude,
	Altitude_Parameters.Pitch,
	Altitude_Parameters.Bank

	FROM Telemetry_Records, Altitude_Parameters, G_Force_Parameters
	WHERE (Altitude_Parameters.Telemetry_ID = Telemetry_Records.Telemetry_ID) 
	AND (G_Force_Parameters.Telemetry_ID = Telemetry_Records.Telemetry_ID)

