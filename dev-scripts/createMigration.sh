#!/bin/bash

# Navigate to the Infrastructure folder
cd ../Infrastructure

# Run the dotnet-ef command
dotnet ef migrations add "MigrationName" -v

# Return to the original directory (optional)
cd ../scripts