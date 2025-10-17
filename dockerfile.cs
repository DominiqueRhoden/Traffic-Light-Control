# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set working directory
WORKDIR /src

# Copy the solution file and restore dependencies
COPY *.sln .
COPY TrafficLightControl/*.csproj ./TrafficLightControl/
RUN dotnet restore

# Copy the full source code
COPY TrafficLightControl/. ./TrafficLightControl/

# Publish the app to the /app folder
WORKDIR /src/TrafficLightControl
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port if the app serves HTTP (change if needed)
EXPOSE 5000

# Run the application
ENTRYPOINT ["dotnet", "TrafficLightControl.dll"]

