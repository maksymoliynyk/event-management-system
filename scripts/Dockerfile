# Use the .NET SDK image to run the EF Core commands
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS migrate

# Set the working directory inside the container
WORKDIR /src

# Copy the entire project into the container
COPY . .

# Set the entrypoint to your script
ENTRYPOINT ["bash", "/src/scripts/migrate-and-run.sh"]
