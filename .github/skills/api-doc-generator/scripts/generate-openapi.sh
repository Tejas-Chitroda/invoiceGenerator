#!/usr/bin/env bash
set -euo pipefail

PROJECT="${PROJECT:-Backend/Invoice_Generator/InvoiceGenerator.csproj}"
OUTPUT="${OUTPUT:-Backend/Invoice_Generator/docs/openapi.json}"
SWAGGER_URL="${SWAGGER_URL:-http://localhost:5000/swagger/v1/swagger.json}"
APP_URL="${APP_URL:-http://localhost:5000}"
STARTUP_WAIT_SECONDS="${STARTUP_WAIT_SECONDS:-12}"
LOG_FILE="${LOG_FILE:-/tmp/invoice-api-doc-generator.log}"

if ! command -v dotnet >/dev/null 2>&1; then
  echo "dotnet SDK is required but was not found in PATH."
  exit 1
fi

mkdir -p "$(dirname "$OUTPUT")"

echo "Starting API on $APP_URL..."
dotnet run --project "$PROJECT" --urls "$APP_URL" >"$LOG_FILE" 2>&1 &
APP_PID=$!

cleanup() {
  if kill -0 "$APP_PID" >/dev/null 2>&1; then
    kill "$APP_PID" >/dev/null 2>&1 || true
  fi
}
trap cleanup EXIT

sleep "$STARTUP_WAIT_SECONDS"

echo "Fetching OpenAPI from $SWAGGER_URL"
if command -v curl >/dev/null 2>&1; then
  curl -fsSL "$SWAGGER_URL" -o "$OUTPUT"
elif command -v wget >/dev/null 2>&1; then
  wget -qO "$OUTPUT" "$SWAGGER_URL"
else
  echo "Neither curl nor wget is available to download OpenAPI output."
  exit 1
fi

if [[ ! -s "$OUTPUT" ]]; then
  echo "OpenAPI file was not generated at $OUTPUT"
  echo "See logs at $LOG_FILE"
  exit 1
fi

echo "OpenAPI generated successfully at $OUTPUT"
