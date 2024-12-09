# Set the environment variable for ASP.NET Core
$env:ASPNETCORE_ENVIRONMENT = "Test"

# Navigate to the Infrastructure folder
Set-Location -Path "..\Infrastructure"

# Run the dotnet-ef command
dotnet-ef database update -s ..\API\API.csproj --configuration Test -v

# Return to the original directory (optional)
Set-Location -Path "..\scripts"
