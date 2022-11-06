----------------------
DAL
----------------------
Library containing classes for communicating with an FDMS database


-----------------------
SERVER
-----------------------
The Server project builds to create a deployable SQL server database for the FDMS (in the form of a .dacpac file).

The output .dacpac file can be deployed to a SQL Server instance through the Microsoft SQL Server Management Studio or by running one of the DeployServer scripts.

The database is deployed with built-in credentials for an account to be used by the FDMS application. This account has Insert, Select, and Execute privileges
The account credentials are as follows - 
	User ID : FDMS_User
	Password : FDMS_Password

	----------------------------------------------------------------------------------------
	INSTRUCTION TO BUILD AND DEPLOY DATABASE TO A SQL SERVER INSTANCE
	----------------------------------------------------------------------------------------
	- have Microsoft SQL Server installed, running, and configured to run on some ip address and port
	- If you don't already have one, create an admin-type login/user account on the sql server (an account with a name, password, and credentials for creating and altering databases)
	- Build the Server project with Visual Studio
	- Run the appropriate DeployServer script (depending on if you built the DEBUG or RELEASE server)
	- Answer the scripts prompts for the ip and port of a sql server as well as a user id and password
		- After answering the prompts, the script will deploy the .dacpac file which was built by the Server project (hopefully all goes well...)



----------------------
DATABASE TEST CLIENT
----------------------
The database test client program lets one communicate with a deployed FDMS database.
Use the credentials from above for connecting to the database in the test client.
Textboxes are filled in for convenience.
The Program also providees an example of how to use the classes from the DAL library
!! The formats accepted by the Timestamp Textbox are finnicky


