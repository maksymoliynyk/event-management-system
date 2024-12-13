# Navigate to the Infrastructure folder
Set-Location -Path "..\Infrastructure"

# Run the dotnet-ef command
dotnet-ef migrations add "SetupEfWithBackingFields" -v

# Return to the original directory (optional)
Set-Location -Path "..\scripts"