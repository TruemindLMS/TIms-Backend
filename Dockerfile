# =========================
# Multi-stage Dockerfile for .NET 10 API (Render compatible)
# =========================

# -------- Build Stage --------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY TeamIndia.TalentFlow/TeamIndia.TalentFlow.API/*.csproj TeamIndia.TalentFlow.API/
COPY TeamIndia.TalentFlow/TeamIndia.TalentFlow.Domain/*.csproj TeamIndia.TalentFlow.Domain/
COPY TeamIndia.TalentFlow/TeamIndia.TalentFlow.Application/*.csproj TeamIndia.TalentFlow.Application/
COPY TeamIndia.TalentFlow/TeamIndia.TalentFlow.Infrastructure/*.csproj TeamIndia.TalentFlow.Infrastructure/

# Restore NuGet packages for the API project (no solution file present)
RUN dotnet restore TeamIndia.TalentFlow.API/TeamIndia.TalentFlow.API.csproj

# Copy full source code
COPY TeamIndia.TalentFlow/. ./

# Publish API project
RUN dotnet publish TeamIndia.TalentFlow.API/TeamIndia.TalentFlow.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# -------- Runtime Stage --------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Bind to PORT environment variable (Render provides $PORT)
ENV ASPNETCORE_URLS="http://0.0.0.0:${PORT:-5000}"
EXPOSE 5000

# Copy published output from build stage
COPY --from=build /app/publish .

# ---------- Entrypoint ----------
ENTRYPOINT ["dotnet", "TeamIndia.TalentFlow.API.dll"]