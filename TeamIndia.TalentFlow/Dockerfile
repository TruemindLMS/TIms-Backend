# Multi-stage Dockerfile for .NET 7 LTS
# Builds the solution and publishes the API into a lean runtime image.

# 1) Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy solution and project files for restore
COPY *.sln ./
COPY TeamIndia.TalentFlow.API/TeamIndia.TalentFlow.API.csproj TeamIndia.TalentFlow.API/
COPY TeamIndia.TalentFlow.Infrastructure/TeamIndia.TalentFlow.Infrastructure.csproj TeamIndia.TalentFlow.Infrastructure/
COPY TeamIndia.TalentFlow.Application/TeamIndia.TalentFlow.Application.csproj TeamIndia.TalentFlow.Application/
COPY TeamIndia.TalentFlow.Domain/TeamIndia.TalentFlow.Domain.csproj TeamIndia.TalentFlow.Domain/

# Restore NuGet packages
RUN dotnet restore TeamIndia.TalentFlow.API/TeamIndia.TalentFlow.API.csproj

# Copy everything and publish
COPY . .
RUN dotnet publish TeamIndia.TalentFlow.API/TeamIndia.TalentFlow.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# 2) Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Bind to PORT from Render environment
ENV ASPNETCORE_URLS="http://0.0.0.0:${PORT:-5000}"
EXPOSE 5000

# Copy published output
COPY --from=build /app/publish .

# Optional healthcheck
HEALTHCHECK --interval=30s --timeout=5s --start-period=5s --retries=3 CMD curl -f http://localhost:5000/ || exit 1

# Start the application
ENTRYPOINT ["dotnet", "TeamIndia.TalentFlow.API.dll"]