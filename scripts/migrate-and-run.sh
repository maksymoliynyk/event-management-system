#!/bin/bash

# Navigate to the Infrastructure folder
cd /src/Infrastructure

# Run the dotnet-ef command
dotnet ef database update -s /src/API/API.csproj -v

# Return to the scripts folder (optional)
cd /src/scripts
