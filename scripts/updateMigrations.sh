#!/bin/bash

# Set the environment variable for ASP.NET Core
export ASPNETCORE_ENVIRONMENT="Test"

# Navigate to the Infrastructure folder
cd ../Infrastructure

# Run the dotnet-ef command
dotnet-ef database update -s ../API/API.csproj --configuration Test -v

# Return to the original directory (optional)
cd ../scripts