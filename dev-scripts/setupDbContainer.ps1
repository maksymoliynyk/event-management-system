docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Password1234" -e "MSSQL_PID=Express" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

docker exec -it <container name/id> /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Password1234 -C