#!/usr/bin/env bash
set -euo pipefail

MIGRATION_NAME="${1:-}"
PROJECT="${PROJECT:-Backend/Invoice_Generator/InvoiceGenerator.csproj}"
STARTUP_PROJECT="${STARTUP_PROJECT:-Backend/Invoice_Generator/InvoiceGenerator.csproj}"
CONTEXT="${CONTEXT:-InvoiceDbContext}"

if [[ -z "$MIGRATION_NAME" ]]; then
  echo "Usage: ./new-migration.sh <MigrationName>"
  exit 1
fi

if ! command -v dotnet >/dev/null 2>&1; then
  echo "dotnet SDK is required but was not found in PATH."
  exit 1
fi

if ! dotnet tool list -g | grep -q "dotnet-ef"; then
  echo "dotnet-ef not found. Installing globally..."
  dotnet tool install --global dotnet-ef
fi

echo "Creating EF Core migration '$MIGRATION_NAME'..."
dotnet ef migrations add "$MIGRATION_NAME" \
  --project "$PROJECT" \
  --startup-project "$STARTUP_PROJECT" \
  --context "$CONTEXT"

echo "Migration created successfully."
