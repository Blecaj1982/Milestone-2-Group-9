CREATE TABLE [FDMS].[Telemetry_Records]
(
	[Telemetry_ID] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [Aircraft_Tail_Num] VARCHAR(6) NOT NULL, 
    [Timestamp] DATETIME NOT NULL , 
    [Entry_Timestamp] DATETIME NOT NULL DEFAULT getdate()
)
