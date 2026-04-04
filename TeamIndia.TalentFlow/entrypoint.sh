#!/bin/bash
set -e

# If RUN_MIGRATIONS=true, attempt to run EF Core migrations before starting the app
if [ "${RUN_MIGRATIONS}" = "true" ]; then
  echo "[entrypoint] RUN_MIGRATIONS=true -> applying EF Core migrations"
  if command -v dotnet >/dev/null 2>&1; then
    dotnet ef database update --project TeamIndia.TalentFlow.Infrastructure --startup-project TeamIndia.TalentFlow.API || true
  else
    echo "[entrypoint] dotnet not available in image; skipping migrations"
  fi
fi

echo "[entrypoint] Starting API"
exec dotnet TeamIndia.TalentFlow.API.dll
