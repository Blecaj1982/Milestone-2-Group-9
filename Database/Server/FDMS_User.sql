CREATE LOGIN FDMS_User WITH PASSWORD = 'FDMS_Password'
GO

CREATE USER [FDMS_User]
	FOR LOGIN FDMS_User
	WITH DEFAULT_SCHEMA = FDMS
GO

GRANT CONNECT, SELECT, INSERT, EXECUTE TO [FDMS_User]