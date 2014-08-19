/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

IF (EXISTS(select * from sysusers where name = 'BENDAY\aspdotnetapppooluser'))
BEGIN
	DROP USER [BENDAY\aspdotnetapppooluser]	
END

CREATE USER [BENDAY\aspdotnetapppooluser] FOR LOGIN [BENDAY\aspdotnetapppooluser]
GO
EXEC sp_addrolemember N'db_datareader', N'BENDAY\aspdotnetapppooluser'
GO
EXEC sp_addrolemember N'db_datawriter', N'BENDAY\aspdotnetapppooluser'