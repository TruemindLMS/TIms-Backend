# Multi-stage Dockerfile tuned for .NET 10
# Builds the API and publishes into a runtime image. Build context must be repository root.

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy entire repo into build context to ensure project files are available
COPY . .

# Restore and publish the API project directly (avoid relying on .sln name)
RUN dotnet restore "TeamIndia.TalentFlow.API/TeamIndia.TalentFlow.API.csproj"
RUN dotnet publish "TeamIndia.TalentFlow.API/TeamIndia.TalentFlow.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Bind to PORT from environment (Render provides $PORT)
ENV ASPNETCORE_URLS="http://0.0.0.0:${PORT:-5000}"
EXPOSE 5000

# Copy published output
COPY --from=build /app/publish .

# Add a simple entrypoint script (container will start the API)
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]
