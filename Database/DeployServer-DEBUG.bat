@ECHO OFF

set /P ip=SQL Server IP :
set /P port=SQL Server Port :
set /P id=SQL Server User ID : 
set /P pass=Sql SErver Password : 

".\sqlPackage\sqlpackage.exe" /Action:Publish /SourceFile:"./Server/bin/Debug/FDMS_Server.dacpac" /TargetConnectionString:"Server=tcp:%ip%,%port%;User ID=%id%; Password=%pass%;Initial Catalog=FDMS_Server;"
