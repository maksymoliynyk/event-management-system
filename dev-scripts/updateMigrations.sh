#!/bin/bash
# Navigate to the Infrastructure folder
cd ../Infrastructure

# Run the dotnet-ef command
dotnet-ef database update -s ../API/API.csproj -v

# Return to the original directory (optional)
cd ../scripts