# =========================
# Multi-stage Dockerfile for .NET 10 API
# =========================

# -------- Build Stage --------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only the API project first to speed up restore
COPY TeamIndia.TalentFlow.API/ ./TeamIndia.TalentFlow.API/

# If you have a solution file, copy it as well
# COPY TIms-Backend.sln ./

# Set working directory to the API project
WORKDIR /src/TeamIndia.TalentFlow.API

# Restore NuGet packages
RUN dotnet restore

# Publish the API
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# -------- Runtime Stage --------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Bind to PORT environment variable (Render provides $PORT)
ENV ASPNETCORE_URLS="http://0.0.0.0:${PORT:-5000}"
EXPOSE 5000

# Copy published output from build stage
COPY --from=build /app/publish .

# ---------- Entrypoint ----------
# Use inline entrypoint instead of separate file
ENTRYPOINT ["dotnet", "TeamIndia.TalentFlow.API.dll"]