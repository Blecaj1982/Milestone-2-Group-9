CREATE TABLE [FDMS].[G_Force_Parameters]
(
	[Telemetry_ID] BIGINT NOT NULL PRIMARY KEY, 
    [Accel_X] DECIMAL NOT NULL, 
    [Accel_Y] DECIMAL NOT NULL, 
    [Accel_Z] DECIMAL NOT NULL, 
    CONSTRAINT [FK_G_Force_Parameters_To_Telemetry_Records] FOREIGN KEY ([Telemetry_ID]) REFERENCES [FDMS].[Telemetry_Records]([Telemetry_ID])
)
