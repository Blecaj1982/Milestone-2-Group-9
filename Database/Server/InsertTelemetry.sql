CREATE PROCEDURE [FDMS].[InsertTelemetry]
	@Aircraft_Tail_Num varchar(6),
	@Timestamp datetime,
	@Accel_X float,
	@Accel_Y float,
	@Accel_Z float,
	@Weight float,
	@Altitude float,
	@Pitch float,
	@Bank float
AS
	DECLARE @Telemetry_ID int;

	INSERT INTO [Telemetry_Records] (Aircraft_Tail_Num, Timestamp)
	VALUES (@Aircraft_Tail_Num, @Timestamp);

	SET @Telemetry_ID = SCOPE_IDENTITY();

	INSERT INTO [G_Force_Parameters] VALUES(@Telemetry_ID, @Accel_X, @Accel_Y, @Accel_Z, @Weight);
	INSERT INTO [Altitude_Parameters] VALUES(@Telemetry_ID, @Altitude, @Pitch, @Bank);

RETURN 0
