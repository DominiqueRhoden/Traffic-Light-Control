# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj(s) and restore dependencies
COPY *.sln .
COPY TrafficLightControl/*.csproj ./TrafficLightControl/
COPY TrafficLightControl.Tests/*.csproj ./TrafficLightControl.Tests/
RUN dotnet restore

# Copy the rest of the code and publish
COPY . .
RUN dotnet publish ./TrafficLightControl/ -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TrafficLightControl.dll"]



