# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy solution and csproj files
COPY TIms-Backend.sln ./
COPY TIms-Backend/TeamIndia.TalentFlow/TeamIndia.TalentFlow.API/*.csproj TeamIndia.TalentFlow.API/
COPY TIms-Backend/TeamIndia.TalentFlow/TeamIndia.TalentFlow.Domain/*.csproj TeamIndia.TalentFlow.Domain/
COPY TIms-Backend/TeamIndia.TalentFlow/TeamIndia.TalentFlow.Application/*.csproj TeamIndia.TalentFlow.Application/
COPY TIms-Backend/TeamIndia.TalentFlow/TeamIndia.TalentFlow.Infrastructure/*.csproj TeamIndia.TalentFlow.Infrastructure/

# Restore NuGet packages
RUN dotnet restore TeamIndia.TalentFlow.API/TeamIndia.TalentFlow.API.csproj

# Copy all project files
COPY TIms-Backend/TeamIndia.TalentFlow/. ./

# Publish API project
RUN dotnet publish TeamIndia.TalentFlow.API/TeamIndia.TalentFlow.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Bind to PORT from Render
ENV ASPNETCORE_URLS="http://0.0.0.0:${PORT:-5000}"
EXPOSE 5000

# Copy published output
COPY --from=build /app/publish .

# Start the application
ENTRYPOINT ["dotnet", "TeamIndia.TalentFlow.API.dll"]