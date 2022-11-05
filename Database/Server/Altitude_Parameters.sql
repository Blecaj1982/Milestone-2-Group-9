CREATE TABLE [FDMS].[Altitude_Parameters]
(
	[Telemetry_ID] BIGINT NOT NULL PRIMARY KEY, 
    [Altitude] DECIMAL NOT NULL, 
    [Pitch] DECIMAL NOT NULL, 
    [Bank] DECIMAL NOT NULL, 
    CONSTRAINT [FK_Altitude_Parameters_To_Telemetry_Records] FOREIGN KEY ([Telemetry_ID]) REFERENCES [FDMS].[Telemetry_Records]([Telemetry_ID])
)
