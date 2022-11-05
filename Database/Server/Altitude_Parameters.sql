CREATE TABLE [FDMS].[Altitude_Parameters]
(
	[Telemetry_ID] BIGINT NOT NULL PRIMARY KEY, 
    [Altitude] FLOAT NOT NULL, 
    [Pitch] FLOAT NOT NULL, 
    [Bank] FLOAT NOT NULL, 
    CONSTRAINT [FK_Altitude_Parameters_To_Telemetry_Records] FOREIGN KEY ([Telemetry_ID]) REFERENCES [FDMS].[Telemetry_Records]([Telemetry_ID])
)
